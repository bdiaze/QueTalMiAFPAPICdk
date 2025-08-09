using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QueTalMiAFPAPI.Entities;
using QueTalMiAFPAPI.Interfaces;
using QueTalMiAFPAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Controllers {
	[Route("[controller]")]
	[ApiController]
	public class UfController(IUfDAO ufDAO) : ControllerBase {

		/// <summary>
		/// Actualiza los valores UF que se encuentran registrados según lo informado.
		/// </summary>
		/// <param name="ufsExtraidas"></param>
		/// <returns>Cantidad de registros insertados y actualizados.</returns>
		[Route("[action]")]
		[HttpPost]
		public async Task<ActionResult<SalActualizacionMasivaUf>> ActualizacionMasiva(EntActualizacionMasivaUf ufsExtraidas) {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try {
				SalActualizacionMasivaUf salida = new() {
					CantUfsInsertadas = 0,
					CantUfsActualizadas = 0
				};

				foreach (Uf uf in ufsExtraidas.Ufs) {
					Uf? ufExistente = await ufDAO.ObtenerUf(uf.Fecha);
					if (ufExistente == null) {
						await ufDAO.InsertarUf(uf);
						salida.CantUfsInsertadas++;
					} else if (ufExistente.Valor != uf.Valor) {
						ufExistente.Valor = uf.Valor;
						await ufDAO.ActualizarUf(ufExistente);
						salida.CantUfsActualizadas++;
					}
				}

                LambdaLogger.Log(
                    $"[POST] - [UfController] - [ActualizacionMasiva] - [{stopwatch.ElapsedMilliseconds} ms] - [{StatusCodes.Status200OK}] - " +
                    $"Actualización masiva de UF exitosa: {salida.CantUfsInsertadas} insertadas y {salida.CantUfsActualizadas} actualizadas.");

                return salida;
            } catch (Exception ex) {
                LambdaLogger.Log(
                    $"[POST] - [UfController] - [ActualizacionMasiva] - [{stopwatch.ElapsedMilliseconds} ms] - [{StatusCodes.Status500InternalServerError}] - " +
                    $"Ocurrió un error en la actualización masiva de UF. " +
                    $"{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
	}
}
