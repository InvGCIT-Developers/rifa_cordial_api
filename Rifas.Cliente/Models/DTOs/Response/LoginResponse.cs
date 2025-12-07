using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Rifas.Client.Models.DTOs.Response
{
    public class LoginResponse
    {
        
        public string Existe { get; set; }
        
        public string CuentaCliente { get; set; }
        
        public string IDdeCliente { get; set; }
        
        public string ClaveDeCliente { get; set; }
        
        public string CorreoDeCliente { get; set; }
        
        public string BalanceDeCliente { get; set; }
        
        public string BalancePendiente { get; set; }
        
        public string BalanceDisponible { get; set; }
        
        public string BalanceDisponibleConMatchPlay { get; set; }
        
        public string Credito { get; set; }
        
        public string CreditoTemporal { get; set; }
        
        public string SaldoOCredito { get; set; }
        
        public string UseKiosk { get; set; }
        
        public string PostUp { get; set; }
        
        public string Compania { get; set; }
        
        public string Paisdependencia { get; set; }
       
        public string IdPaisdependencia { get; set; }
        
        public string Agente { get; set; }
        
        public string IdAgente { get; set; }
       
        public string NombreUser { get; set; }
    
        public string Nombre { get; set; }
        
        public string Apellido { get; set; }
        
        public string Cedula { get; set; }
       
        public string FechaNacimiento { get; set; }
      
        public string Direccion { get; set; }
        
        public string Ciudad { get; set; }
        
        public string Ciudadcodigo { get; set; }
        
        public string IdCiudad { get; set; }
       
        public string Pais { get; set; }
        
        public string Moneda { get; set; }
        
        public string Monedadef { get; set; }
        
        public string Telefono { get; set; }
       
        public string Pin { get; set; }
       
        public string Activo { get; set; }
        
        public string Esdemo { get; set; }
      
        public string Logeado { get; set; }
        
        public string ConfigBotones { get; set; }
        
        public string Statuslog { get; set; }
    }
}
