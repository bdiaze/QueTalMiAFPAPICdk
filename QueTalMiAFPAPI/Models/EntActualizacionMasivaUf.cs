using QueTalMiAFPAPI.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Models {
	public class EntActualizacionMasivaUf {
		[Required]
		public required List<Uf> Ufs { get; set; }
	}
}
