using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Entities {
	public class Uf {
		public long Id { get; set; }

		public DateTime Fecha { get; set; }

		public decimal Valor { get; set; }
	}
}
