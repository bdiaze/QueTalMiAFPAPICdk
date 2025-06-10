using Microsoft.AspNetCore.Mvc;
using QueTalMiAFPAPI.Entities;
using QueTalMiAFPAPI.Interfaces;
using QueTalMiAFPAPI.Models;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Controllers {
	[Route("[controller]")]
	[ApiController]
	public class CuotaController(ICuotaDAO cuotaDAO) : ControllerBase {

		/// <summary>
		/// Actualiza los valores cuotas que se encuentran registrados según lo informado.
		/// </summary>
		/// <param name="cuotasExtraidas"></param>
		/// <returns>Cantidad de registros insertados y actualizados.</returns>
		[Route("[action]")]
		[HttpPost]
		public async Task<ActionResult<SalActualizacionMasivaCuota>> ActualizacionMasiva(EntActualizacionMasivaCuota cuotasExtraidas) {
			SalActualizacionMasivaCuota salida = new() {
				CantCuotasInsertadas = 0,
				CantCuotasActualizadas = 0
			};

			foreach (Cuota cuota in cuotasExtraidas.Cuotas) {
				Cuota? cuotaExistente = await cuotaDAO.ObtenerCuota(cuota.Afp, cuota.Fecha, cuota.Fondo);

				if (cuotaExistente == null) {
					await cuotaDAO.InsertarCuota(cuota);
					salida.CantCuotasInsertadas++;
				} else if (cuotaExistente.Valor != cuota.Valor) {
					cuotaExistente.Valor = cuota.Valor;
					await cuotaDAO.ActualizarCuota(cuotaExistente);
					salida.CantCuotasActualizadas++;
				}
			}

			return salida;
		}
	}
}
