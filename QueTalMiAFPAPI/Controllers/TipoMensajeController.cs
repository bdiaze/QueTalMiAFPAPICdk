using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QueTalMiAFPAPI.Entities;
using QueTalMiAFPAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Controllers {
	[Produces("application/json")]
	[Route("[controller]")]
	[ApiController]
	public class TipoMensajeController(ITipoMensajeDAO tipoMensajeDAO) : ControllerBase {

		/// <summary>
		/// Obtiene un tipo de mensaje según identificador.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// 
		///     GET TipoMensaje/ObtenerTipoMensaje
		///     {
		///         "idTipoMensaje": 1
		///     }
		///     
		/// </remarks>
		/// <param name="idTipoMensaje"></param>
		/// <returns>Tipo de mensaje.</returns>
		[Route("[action]")]
		[HttpGet]
		public async Task<ActionResult<TipoMensaje?>> ObtenerTipoMensaje(short idTipoMensaje) {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try {
                TipoMensaje salida = await tipoMensajeDAO.ObtenerTipoMensaje(idTipoMensaje);

                LambdaLogger.Log(
                    $"[GET] - [TipoMensajeController] - [ObtenerTipoMensaje] - [{stopwatch.ElapsedMilliseconds} ms] - [{StatusCodes.Status200OK}] - " +
                    $"Se obtuvo el tipo de mensaje exitosamente - ID Tipo Mensaje: {idTipoMensaje}.");

                return salida;
			} catch (Exception ex) {
                LambdaLogger.Log(
                    $"[GET] - [TipoMensajeController] - [ObtenerTipoMensaje] - [{stopwatch.ElapsedMilliseconds} ms] - [{StatusCodes.Status500InternalServerError}] - " +
                    $"Ocurrió un error al obtener el tipo de mensaje - ID Tipo Mensaje: {idTipoMensaje}. " +
                    $"{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		/// <summary>
		/// Obtiene los tipos de mensajes vigentes.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// 
		///     GET TipoMensaje/ObtenerVigentes
		///     
		/// </remarks>
		/// <returns>Los tipos de mensajes vigentes.</returns>
		[Route("[action]")]
		[HttpGet]
		public async Task<ActionResult<List<TipoMensaje>>> ObtenerVigentes() {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try {
                List<TipoMensaje> salida = await tipoMensajeDAO.ObtenerTiposMensaje(1);

                LambdaLogger.Log(
                    $"[GET] - [TipoMensajeController] - [ObtenerVigentes] - [{stopwatch.ElapsedMilliseconds} ms] - [{StatusCodes.Status200OK}] - " +
                    $"Se obtuvo los tipos de mensaje vigentes exitosamente: {salida.Count} registros.");

                return salida;
            } catch (Exception ex) {
                LambdaLogger.Log(
                    $"[GET] - [TipoMensajeController] - [ObtenerVigentes] - [{stopwatch.ElapsedMilliseconds} ms] - [{StatusCodes.Status500InternalServerError}] - " +
                    $"Ocurrió un error al obtener los tipos de mensaje vigentes. " +
                    $"{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
	}
}
