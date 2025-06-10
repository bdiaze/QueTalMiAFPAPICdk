using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Entities {
	public class Cuota {
		public long Id { get; set; }

		public required string Afp { get; set; }

		public DateTime Fecha { get; set; }

		public required string Fondo { get; set; }

		public decimal Valor { get; set; }
	}
}
