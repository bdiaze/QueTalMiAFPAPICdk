using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QueTalMiAFPAPI.Entities;
using QueTalMiAFPAPI.Interfaces;
using System;
using System.Collections.Generic;
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
			return await tipoMensajeDAO.ObtenerTipoMensaje(idTipoMensaje);
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
			return await tipoMensajeDAO.ObtenerTiposMensaje(1);
		}
	}
}
