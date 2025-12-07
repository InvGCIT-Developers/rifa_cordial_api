
using Dapper;
using GCIT.Core.Helpers;
using GCIT.Core.Models.DTOs.Response;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Rifas.Client.Common;
using Rifas.Client.Data;
using Rifas.Client.Models.DTOs;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;
using Rifas.Client.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace Rifas.Client.Modulos.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly RifasContext _context;
        public AuthService(IConfiguration configuration ,ILogger<AuthService> log, RifasContext context)
        {
            _configuration = configuration;
            _logger = log;
            _context = context;
        }

        public Task<AuthResponse> AuthenticateAsync(AuthRequest user)
        {
            try
            {
                // Validación simple contra configuración.
                // Config keys esperadas:
                // Jwt:Key, Jwt:Issuer, Jwt:Audience, Jwt:ExpiresMinutes
                // Auth:Username, Auth:Password

                var login = new LoginResponse();


                string sql = $"SELECT Clientes.id IDdeCliente, Clientes.dimCedula, Clientes.Cedula, Clientes.Nombre, Clientes.Apellido, Clientes.Usuario NombreUser, Clientes.clave ClaveDeCliente, Clientes.Pin, Clientes.tipoCuenta, Clientes.fnacimiento, Clientes.Pais, Clientes.Ciudad, " +
                             $"Clientes.Moneda, Agentes.defMoneda, Clientes.Direccion, Clientes.Email CorreoDeCliente, Clientes.Telefono, Clientes.Cpostal, Clientes.perfil, Clientes.validadoporFecha, Clientes.validohastaFec, Clientes.FechaIngreso, Clientes.esDemo," +
                             $"Clientes.Status, ClientesID.idcompania, ClientesID.compania, ClientesID.idPaisdependencia, ClientesID.Paisdependencia, ClientesID.idAgente IdAgente, ClientesID.NombreAgente Agente, ClientesID.idlocal, ClientesID.local," +
                             $"ClientesID.terminal, Clientes.logeado , Clientes.PorcComision,Clientes.is2Auth, (select top 1 Nombre from [dbo].[SiteEmpresa] where LOWER([Site]) = @webSite), EmailVerificado, SMSVerificado, Clientes.Idioma, '1' Existe  " +
                             $"FROM  Clientes INNER JOIN ClientesID ON Clientes.id = ClientesID.idCliente INNER JOIN Agentes ON Agentes.id=ClientesID.idAgente WHERE Clientes.Usuario = @UserName";

                var connectionString = _configuration.GetConnectionString("DefaultDB");
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    login = connection.Query<LoginResponse>(sql, new { UserName = user.UserName, webSite = user.WebSite }).FirstOrDefault();

                }

                if (login == null)
                {
                    throw new Exception($"{Constantes.HTTP_STATUS_500}:Client not found");
                }

                if (login.ClaveDeCliente != user.Password)
                {
                    _logger.LogError("No autorizado");
                    throw new Exception($"{Constantes.HTTP_STATUS_401}:Client Not authorized");
                }

                _logger.LogInformation($"Usuario: {login.NombreUser} logeado");


                var cliente = new ObtenerClienteResponse
                {
                    data = new DataDTO
                    {
                        cliente = new ClienteDTO
                        {
                            id = Convert.ToInt32(login.IDdeCliente),
                            usuario = login.NombreUser,
                            nombreAgente = login.Agente,
                            email = login.CorreoDeCliente,
                            idAgente = Convert.ToInt32(login.IdAgente),
                            moneda = login.Moneda,
                            clave = login.ClaveDeCliente
                        }
                    }
                };

                if (cliente == null)
                {
                    throw new Exception($"{Constantes.HTTP_STATUS_500}:Client not found");
                }

                if (cliente.data.cliente.clave != user.Password)
                {
                    _logger.LogError("No autorizado");
                    throw new Exception($"{Constantes.HTTP_STATUS_401}:Client Not authorized");
                }


                //var configuredUser = _configuration["Auth:Username"];
                //var configuredPass = _configuration["Auth:Password"];

                //if (string.IsNullOrWhiteSpace(user?.UserName) || string.IsNullOrWhiteSpace(user.Password)
                //    || user.UserName != configuredUser || user.Password != configuredPass)
                //{
                //    return Task.FromResult(new AuthResponse
                //    {
                //        EsExitoso = false,
                //        Mensaje = "Credenciales inválidas",
                //        CodigoError = "AUTH_INVALID_CREDENTIALS",
                //        Datos = null
                //    });
                //}

                var key = _configuration["Jwt:Key"];
                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];
                var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var em) ? em : 60;

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key ?? throw new InvalidOperationException("Jwt:Key no está configurado")));
                var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim("Id", cliente.data.cliente.id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Email, cliente.data.cliente.email),
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