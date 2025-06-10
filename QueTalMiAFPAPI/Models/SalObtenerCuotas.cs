using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Models {
	public class SalObtenerCuotas {
		public string? S3Url { get; set; }
		public List<CuotaUf>? ListaCuotas { get; set; }
	}
}
