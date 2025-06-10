using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Models {
	public class EntIngresarMensaje {
		[Required]
		public short IdTipoMensaje { get; set; }

		[Required]
		public required string Nombre { get; set; }

		[Required]
		public required string Correo { get; set; }

		[Required]
		public required string Mensaje { get; set; }
	}
}
