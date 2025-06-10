using Microsoft.AspNetCore.Mvc;
using QueTalMiAFPAPI.Entities;
using QueTalMiAFPAPI.Interfaces;
using QueTalMiAFPAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Controllers {
	[Route("[controller]")]
	[ApiController]
	public class ComisionController(IComisionDAO comisionDAO) : ControllerBase {

		/// <summary>
		/// Actualiza las comisiones que se encuentran registrados según lo informado.
		/// </summary>
		/// <param name="comisionesExtraidas"></param>
		/// <returns>Cantidad de registros insertados y actualizados.</returns>
		[Route("[action]")]
		[HttpPost]
		public async Task<ActionResult<SalActualizacionMasivaComision>> ActualizacionMasiva(EntActualizacionMasivaComision comisionesExtraidas) {
			SalActualizacionMasivaComision salida = new() {
				CantComisionesInsertadas = 0,
				CantComisionesActualizadas = 0
			};

			foreach (Comision comision in comisionesExtraidas.Comisiones) {
				Comision? comisionExistente = await comisionDAO.ObtenerComision(comision.TipoComision, comision.Afp, comision.Fecha);

				if (comisionExistente == null) {
					await comisionDAO.InsertarComision(comision);
					salida.CantComisionesInsertadas++;
				} else if (comisionExistente.Valor != comision.Valor || comisionExistente.TipoValor != comision.TipoValor) {
					comisionExistente.Valor = comision.Valor;
					comisionExistente.TipoValor = comision.TipoValor;
					await comisionDAO.ActualizarComision(comisionExistente);
					salida.CantComisionesActualizadas++;
				}
			}

			return salida;
		}
	}
}
