using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Models {
	public class EntObtenerUltimaCuota {
		[Required]
		public required string ListaAFPs { get; set; }
		[Required]
		public required string ListaFondos { get; set; }
		[Required]
		public required string ListaFechas { get; set; }
		[Required]
		public int TipoComision { get; set; }
	}
}
