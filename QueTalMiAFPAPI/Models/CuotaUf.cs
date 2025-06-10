using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Models {
	public class CuotaUf {
		public required string Afp { get; set; }
		public required string Fecha { get; set; }
		public required string Fondo { get; set; }
		public decimal Valor { get; set; }
		public decimal? ValorUf { get; set; }
	}
}
