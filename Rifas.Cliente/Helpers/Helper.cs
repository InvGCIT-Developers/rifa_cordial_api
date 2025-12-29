using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Globalization;

namespace Rifas.Client.Helpers
{
    public static class Helper
    {
        /// <summary>
        /// Obtiene la IP remota del cliente que consume la API.
        /// Puede recibir el HttpContext directamente o un IHttpContextAccessor.
        /// Si ambos son null se retorna "unknown".
        /// </summary>
        public static string GetIPAddress(HttpContext? httpContext = null, IHttpContextAccessor? accessor = null)
        {
            var context = httpContext ?? accessor?.HttpContext;
            if (context == null)
                return "unknown";

            // Primero revisar cabeceras que suelen establecer proxies / balanceadores
            // X-Forwarded-For puede contener una lista separada por comas, tomar la primera
            string? forwarded = null;
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var xff) && !string.IsNullOrWhiteSpace(xff))
            {
                forwarded = xff.ToString();
            }
            else if (context.Request.Headers.TryGetValue("X-Real-IP", out var xrip) && !string.IsNullOrWhiteSpace(xrip))
            {
                forwarded = xrip.ToString();
            }

            if (!string.IsNullOrWhiteSpace(forwarded))
            {
                // tomar la primera IP de la lista
                var first = forwarded.Split(',')[0].Trim();
                if (!string.IsNullOrWhiteSpace(first))
                    return NormalizeIp(first);
            }

            // Fallback: usar RemoteIpAddress de la conexión
            var remoteIp = context.Connection.RemoteIpAddress?.ToString();
            if (!string.IsNullOrWhiteSpace(remoteIp))
                return NormalizeIp(remoteIp!);

            return "unknown";
        }

        private static string NormalizeIp(string ip)
        {
            // Normalizar direcciones IPv6 que contienen IPv4 mapeada, ej: ::ffff:192.168.0.1
            if (ip.StartsWith("::ffff:", StringComparison.OrdinalIgnoreCase))
            {
                return ip.Substring(7);
            }

            return ip;
        }

        /// <summary>
        /// Convierte una cantidad entre dos monedas usando un diccionario de tasas opcional.
        /// El diccionario 'rates' debe contener pares [CURRENCY_CODE] => amountPerUSD (ej: EUR => 0.92 significa 1 USD = 0.92 EUR).
        /// Si 'rates' es null se usan tasas por defecto (aproximadas).
        /// Fórmula: amountInTarget = amount * (rateTarget / rateSource)
        /// </summary>
        public static decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency, IDictionary<string, decimal>? rates = null)
        {
            if (string.Equals(fromCurrency, toCurrency, StringComparison.OrdinalIgnoreCase))
                return amount;

            var from = fromCurrency?.ToUpperInvariant() ?? "";
            var to = toCurrency?.ToUpperInvariant() ?? "";

            var effectiveRates = rates ?? GetDefaultRates();

            if (!effectiveRates.TryGetValue(from, out var rateFrom))
                throw new ArgumentException($"Tasa no encontrada para la moneda origen '{fromCurrency}'", nameof(fromCurrency));

            if (!effectiveRates.TryGetValue(to, out var rateTo))
                throw new ArgumentException($"Tasa no encontrada para la moneda destino '{toCurrency}'", nameof(toCurrency));

            // rate values: amount of currency units per 1 USD (1 USD = rate[currency])
            // Convert: amount (in 'from') -> USD = amount / rateFrom ; USD -> to = USD * rateTo
            if (rateFrom == 0)
                throw new DivideByZeroException($"Rate for currency {from} is zero.");

            var amountInUsd = amount / rateFrom;
            var result = amountInUsd * rateTo;
            return Math.Round(result, 2);
        }

        /// <summary>
        /// Convierte usando un servicio externo (exchangerate.host). Si falla, usa la versión sincrónica con tasas por defecto.
        /// </summary>
        public static async Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency, HttpClient? httpClient = null)
        {
            if (string.Equals(fromCurrency, toCurrency, StringComparison.OrdinalIgnoreCase))
                return amount;

            var client = httpClient ?? new HttpClient();
            try
            {
                var key = "6RIJMizNJKpZMBt2NS2Gljy8D2ws8gkw";
                client.DefaultRequestHeaders.Add("apikey", key);
                var url = $"https://api.exchangerate.host/convert?from={Uri.EscapeDataString(fromCurrency)}&to={Uri.EscapeDataString(toCurrency)}&amount={amount.ToString(CultureInfo.InvariantCulture)}";
                using var resp = await client.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                using var stream = await resp.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);
                if (doc.RootElement.TryGetProperty("result", out var resultEl) && resultEl.ValueKind == JsonValueKind.Number)
                {
                    if (resultEl.TryGetDecimal(out var d))
                        return Math.Round(d, 2);
                }

                // fallback to local conversion if API response unexpected
                var fallback = ConvertCurrency(amount, fromCurrency, toCurrency);
                return fallback;
            }
            catch
            {
                // en caso de error de red / API, usar tasas por defecto
                return ConvertCurrency(amount, fromCurrency, toCurrency);
            }
            finally
            {
                if (httpClient == null)
                    client.Dispose();
            }
        }

        /// <summary>
        /// Convierte una cantidad desde la moneda dada a USD (conveniencia).
        /// </summary>
        public static decimal ToUsd(decimal amount, string fromCurrency, IDictionary<string, decimal>? rates = null)
            => ConvertCurrency(amount, fromCurrency, "USD", rates);

        /// <summary>
        /// Convierte una cantidad desde USD a la moneda objetivo (conveniencia).
        /// </summary>
        public static decimal FromUsd(decimal amountUsd, string toCurrency, IDictionary<string, decimal>? rates = null)
            => ConvertCurrency(amountUsd, "USD", toCurrency, rates);

        private static IDictionary<string, decimal> GetDefaultRates()
        {
            // Valores aproximados: 1 USD = X currency
            return new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
            {
                ["USD"] = 1m,
                ["EUR"] = 0.92m,
                ["GBP"] = 0.78m,
                ["MXN"] = 17.0m,
                ["ARS"] = 350.0m,
                ["BRL"] = 5.0m,
                ["CLP"] = 820.0m
            };
        }
    }
}
