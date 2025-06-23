
using KeycloakAuthDemo.Options;
using KeycloakAuthDemo.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace KeycloakAuthDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "http://localhost:8080/realms/DemoRealm";
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });

            builder.Services.Configure<KeycloakOptions>(
                builder.Configuration.GetSection("Keycloak"));

            builder.Services.AddHttpClient<KeycloakAdminService>();

            builder.Services.AddTransient<IClaimsTransformation, KeycloakRealmRoleClaimsTransformation>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", policy =>
                    policy.RequireRole("admin"));
            });


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
