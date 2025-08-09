using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Mvc;
using QueTalMiAFPAPI.Entities;
using QueTalMiAFPAPI.Interfaces;
using QueTalMiAFPAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Controllers {
	[Route("[controller]")]
	[ApiController]
	public class MensajeUsuarioController(IMensajeUsuarioDAO mensajeUsuarioDAO, ITipoMensajeDAO tipoMensajeDAO) : ControllerBase {

		/// <summary>
		/// Obtiene los mensajes ingresados en el formulario de contacto, según rango de fechas.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// 
		///     GET MensajeUsuario/ObtenerMensajes
		///     {
		///         "fechaDesde": "2020-01-01T00:00:00.000Z",
		///         "fechaHasta": "2020-01-31T00:00:00.000Z"
		///     }
		///     
		/// </remarks>
		/// <param name="fechaDesde"></param>
		/// <param name="fechaHasta"></param>
		/// <returns>Los mensajes ingresados en el rango de fechas.</returns>
		[Route("[action]")]
		[HttpGet]
		public async Task<ActionResult<List<MensajeUsuario>>> ObtenerMensajes(DateTime fechaDesde, DateTime fechaHasta) {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try {
				List<MensajeUsuario> salida = await mensajeUsuarioDAO.ObtenerMensajesUsuarios(fechaDesde, fechaHasta);

                LambdaLogger.Log(
                    $"[GET] - [MensajeUsuarioController] - [ObtenerMensajes] - [{stopwatch.ElapsedMilliseconds} ms] - [{StatusCodes.Status200OK}] - " +
                    $"Se obtuvo los mensajes exitosamente - Desde {fechaDesde:yyyy-MM-dd HH:mm:ss} - Hasta {fechaHasta:yyyy-MM-dd HH:mm:ss}: {salida.Count} registros.");

                return salida;
            } catch (Exception ex) {
                LambdaLogger.Log(
                    $"[GET] - [MensajeUsuarioController] - [ObtenerMensajes] - [{stopwatch.ElapsedMilliseconds} ms] - [{StatusCodes.Status500InternalServerError}] - " +
                    $"Ocurrió un error al obtener los mensajes - Desde {fechaDesde:yyyy-MM-dd HH:mm:ss} - Hasta {fechaHasta:yyyy-MM-dd HH:mm:ss}. " +
                    $"{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

		/// <summary>
		/// Ingresar un mensaje completado en el formulario de contacto.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// 
		///     POST MensajeUsuario/IngresarMensaje
		///     {
		///         "idTipoMensaje": 4,
		///         "nombre": "Juan Pérez",
		///         "correo": "juan.perez@ejemplo.cl",
		///         "mensaje": "Quiero felicitar al equipo de QueTalMiAFP.",
		///     }
		///     
		/// </remarks>
		/// <param name="mensaje"></param>
		/// <returns>El mensaje ingresado.</returns>
		[Route("[action]")]
		[HttpPost]
		public async Task<ActionResult<MensajeUsuario>> IngresarMensaje(EntIngresarMensaje mensaje) {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try {
                DateTime fechaActual = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneConverter.TZConvert.GetTimeZoneInfo("Pacific SA Standard Time"));
				MensajeUsuario mensajeIngresado = await mensajeUsuarioDAO.IngresarMensajeUsuario(mensaje.IdTipoMensaje, fechaActual, mensaje.Nombre, mensaje.Correo, mensaje.Mensaje);
				mensajeIngresado.TipoMensaje = await tipoMensajeDAO.ObtenerTipoMensaje(mensajeIngresado.IdTipoMensaje);

                LambdaLogger.Log(
                    $"[POST] - [MensajeUsuarioController] - [IngresarMensaje] - [{stopwatch.ElapsedMilliseconds} ms] - [{StatusCodes.Status200OK}] - " +
                    $"Se insertó el mensaje exitosamente - ID Mensaje: {mensajeIngresado.IdMensaje}.");

                return mensajeIngresado;
            } catch (Exception ex) {
                LambdaLogger.Log(
                    $"[POST] - [MensajeUsuarioController] - [IngresarMensaje] - [{stopwatch.ElapsedMilliseconds} ms] - [{StatusCodes.Status500InternalServerError}] - " +
                    $"Ocurrió un error al insertar el mensaje. " +
                    $"{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
	}
}
