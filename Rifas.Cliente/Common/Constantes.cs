using System;
using System.Collections.Generic;
using System.Text;

namespace Rifas.Client.Common
{
    public static class Constantes
    {
        public const string MSG_400_INVALID_AUTH = "Invalid authentication signature";
        public const string MSG_400_INVALID_TOKEN = "Invalid session token";
        public const string MSG_400_INVALID_REQUEST = "Invalid request";
        public const string MSG_400_INVALID_TRANS = "Invalid transaction";
        public const string MSG_400_INVALID_CURR = "Invalid player currency";
        public const string MSG_401_EXPIRED_SESSION = "Session token expired";
        public const string MSG_404_PLATFORM_NOTFOUND = "Platform not found";
        public const string MSG_404_PRODUCT_NOTFOUND = "Product not found";
        public const string MSG_404_PLAYER_NOTFOUND = "Player not found";
        public const string MSG_404_BET_NOTFOUND = "Bet not found";
        public const string MSG_403_PLAYER_BANNED = "Player banned";
        public const string MSG_403_INSUFICIENT_BALANCE = "Insufficient balance";
        public const string MSG_403_BET_EXCEEDED = "Bet limit exceeded";
        public const string MSG_429_SERVICE_OVERLOADED = "Service overloaded";
        public const string MSG_501_METHOD_NOT_SUPPORTED = "Method not supported";
        public const string MSG_503_SERVICE_UNAVIABLE = "Service unavailable";


        public const int HTTP_STATUS_400 = 400;
        public const int HTTP_STATUS_401 = 401;
        public const int HTTP_STATUS_404 = 404;
        public const int HTTP_STATUS_403 = 403;
        public const int HTTP_STATUS_429 = 429;
        public const int HTTP_STATUS_500 = 500;
        public const int HTTP_STATUS_501 = 501;
        public const int HTTP_STATUS_503 = 503;
        public const string MONEDA_USD = "USD";
        public const string MONEDA_VES = "VES";
        public const string MONEDA_EUR = "EUR";
        public const string MONEDA_COP = "COP";
        public const string MONEDA_MXN = "MXN";
        public const string MONEDA_GBP = "GBP";
        public const string MONEDA_BRL = "BRL";
        public const string MONEDA_ARS = "ARS";
        public const string MONEDA_CLP = "CLP";
    }
}
