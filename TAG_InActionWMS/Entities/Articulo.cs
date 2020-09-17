using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TREMEC_EtiquetaInventario_WS.Entities
{
    public class Articulo
    {
        //public string Clave { get; set; }
        //public string Descripcion { get; set; }
        //public int? DiasMinimoRechazo { get; set; }
        //public int? MultiploCaja { get; set; }
        //public string Categoria { get; set; }
        //public string TipoClasificacion { get; set; }
        //public string Fabricante  { get; set; }
        //public string Unidad { get; set; }
        //public double? Monto { get; set; }
        //public double? Monetario1 { get; set; }
        //public double? PorcentajeReabastoPrimario { get; set; }
        //public double? PorcentajeReabastoSecundario  { get; set; }
        //public string LeyendaSurtido { get; set; }
        //public int IdUsuarioActualiza { get; set; }
        //public int? SinCodigoBarras { get; set; }//0,1
        //public int? ForzarLeyendaSurtido { get; set; }
        //public int? TieneCaducidad { get; set; }
        //public int? Tipo_Caducidad  { get; set; }
        //public string SKU { get; set; }
        //public int? IdLinea { get; set; }
        //public int? IdClasificacion { get; set; }
        //public int? ManejoLote { get; set; }
        //public int? ManejoSerie { get; set; }
        //public int IdEstilo { get; set; }

        public int IdNumParte { get; set; }
        //public string? NumParte { get; set; }
        public int Lote { get; set; }
        //public string? NumSerie { get; set; }
        public int CantidadTotal { get; set; }
        //public string? FechaFabricacion { get; set; }
        public int IdUsuarioActualiza { get; set; }
        //public string? FechaActualiza { get; set; }

        public Articulo()
        {

        }
    }

}