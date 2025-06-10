using QueTalMiAFPAPI.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Models {
	public class EntActualizacionMasivaComision {
		[Required]
		public required List<Comision> Comisiones { get; set; }
	}
}
