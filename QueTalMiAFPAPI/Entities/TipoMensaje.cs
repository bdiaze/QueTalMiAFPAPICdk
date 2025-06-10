using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Entities {
	public class TipoMensaje {
		public short IdTipoMensaje { get; set; }

		public required string DescripcionCorta { get; set; }

		public required string DescripcionLarga { get; set; }

		public byte Vigencia { get; set; }

		public ICollection<MensajeUsuario>? MensajeUsuarios { get; set; }
	}
}
