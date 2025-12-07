
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Rifas.Client.Services.Interfaces;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;
using Rifas.Client.Models.DTOs;

namespace Rifas.Client.Modulos.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<AuthResponse> AuthenticateAsync(AuthRequest request)
        {
            try
            {
                // Validación simple contra configuración.
                // Config keys esperadas:
                // Jwt:Key, Jwt:Issuer, Jwt:Audience, Jwt:ExpiresMinutes
                // Auth:Username, Auth:Password
                var configuredUser = _configuration["Auth:Username"];
                var configuredPass = _configuration["Auth:Password"];

                if (string.IsNullOrWhiteSpace(request?.Username) || string.IsNullOrWhiteSpace(request.Password)
                    || request.Username != configuredUser || request.Password != configuredPass)
                {
                    return Task.FromResult(new AuthResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Credenciales inválidas",
                        CodigoError = "AUTH_INVALID_CREDENTIALS",
                        Datos = null
                    });
                }

                var key = _configuration["Jwt:Key"];
                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];
                var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var em) ? em : 60;

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key ?? throw new InvalidOperationException("Jwt:Key no está configurado")));
                var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, request.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var expires = DateTime.UtcNow.AddMinutes(expiresMinutes);

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Task.FromResult(new AuthResponse
                {
                    EsExitoso = true,
                    Mensaje = "Autenticación exitosa",
                    Datos = new TokenDTO
                    {
                        Token = tokenString,
                        ExpiresAt = expires
                    }
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AuthResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al autenticar: {ex.Message}",
                    CodigoError = "AUTH_ERROR",
                    Datos = null
                });
            }
        }
    }
}