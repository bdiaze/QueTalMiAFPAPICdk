using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Models {
	public class SalObtenerUltimaCuota {
		public required string Afp { get; set; }
		public DateTime Fecha { get; set; }
		public required string Fondo { get; set; }
		public decimal Valor { get; set; }
		public decimal? Comision { get; set; }
	}
}
