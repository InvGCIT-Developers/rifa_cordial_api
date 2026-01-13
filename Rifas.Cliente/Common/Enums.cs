using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Rifas.Client.Common
{
    public enum RifaCategoriaEnum
    {
        Sorteos = 0,
        Premios = 1,
        Beneficencia = 2,
        Promociones = 3
    }

    public enum TicketStatusEnum
    {
        Confirmado = 0,
        Donacion = 1,
        Entregado = 2,
        Cancelado = 3,
        EnProceso = 4

    }

    public enum TicketStateEnum
    {
        [Display(Name = "Sin resultado")] SinResultado = 0,
        [Display(Name = "Ganador")] Ganador = 1,
        [Display(Name = "Primer lugar")] PrimerLugar = 2,
        [Display(Name = "Segundo lugar")] SegundoLugar = 3,
        [Display(Name = "Tercer lugar")] TercerLugar = 4,
        [Display(Name = "Perdedor")] Perdedor = 5
    }

}
