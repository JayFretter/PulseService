﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PulseService.Domain.Adapters;
using PulseService.Security.Models;
using System.Text;

namespace PulseService.Security
{
    public static class SecurityExtensions
    {
        public static void AddSecurity(this IServiceCollection services, IConfigurationRoot configuration)
        {
            var jwtOptionsUnbound = configuration.GetSection("JwtOptions");
            services.Configure<JwtOptions>(jwtOptionsUnbound);
            var jwtOptions = jwtOptionsUnbound.Get<JwtOptions>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

            services.AddAuthorization();

            services.AddSingleton<ITokenManager, JwtTokenManager>();
        }
    }
}