﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Entities {
	public class Comision {
		public const byte TIPO_VALOR_PORCENTAJE = 1;
		public const byte TIPO_VALOR_PESO_CHILENO = 2;

		public const byte TIPO_COMIS_DEPOS_COTIZ_OBLIG = 1; // Por deposito de la cotización obligatoria
		public const byte TIPO_COMIS_ADMIN_CTA_AHO_VOL = 2; // Por administración de la cuenta de ahorro voluntaria
		public const byte TIPO_COMIS_ADMIN_AHO_PRE_VOL = 3; // Por administración del ahorro previsional voluntaria
		public const byte TIPO_COMIS_TRANS_AHO_PRE_VOL = 4; // Por transferencia de depósitos del ahorro previsional voluntario

		public long Id { get; set; }

		public required string Afp { get; set; }

		public DateTime Fecha { get; set; }

		public decimal Valor { get; set; }

		public byte TipoComision { get; set; }

		public byte TipoValor { get; set; }
	}
}
