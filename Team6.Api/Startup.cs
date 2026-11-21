using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Team6.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Team6.Api.Security;
namespace Team6.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
           
           services.AddControllers();
           // Configure JWT authentication using Auth0.
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = Configuration["Auth0:Authority"];
    options.Audience = Configuration["Auth0:Audience"];
});

// Allow the React frontend to communicate with this API.
services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register the handler that checks Auth0 permissions.
services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

// Require the delete:catalog permission to delete products.
services.AddAuthorization(options =>
{
    options.AddPolicy("delete:catalog", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new HasScopeRequirements(
            "delete:catalog",
            Configuration["Auth0:Authority"]));
    });
});
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
            services.AddDbContext<StoreContext>(options =>
                options.UseSqlite("Data Source=../Registrar.sqlite", 
                b => b.MigrationsAssembly("Team6.Api")));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "jet.piranha.Api v1"));
            }

           app.UseHttpsRedirection();

app.UseRouting();

// Apply the CORS policy to requests from the React frontend.
app.UseCors();

// Authenticate requests before checking permissions.
app.UseAuthentication();

app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
