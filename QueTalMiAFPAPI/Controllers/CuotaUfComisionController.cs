using Microsoft.AspNetCore.Mvc;
using QueTalMiAFPAPI.Entities;
using QueTalMiAFPAPI.Helpers;
using QueTalMiAFPAPI.Interfaces;
using QueTalMiAFPAPI.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Controllers {
    [Produces("application/json")]
    [Route("[controller]")]
	[ApiController]
	public class CuotaUfComisionController(S3BucketHelper s3BucketHelper, ICuotaUfComisionDAO cuotaUfComisionDAO) : ControllerBase {

        /// <summary>
        /// Obtiene los valores cuotas y UF para las AFPs y fondos indicados, según un rango de fecha.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET CuotaUfComision/ObtenerCuotas
        ///     {
        ///         "listaAFPs": "CAPITAL,CUPRUM",
        ///         "listaFondos": "A,B",
        ///         "fechaInicial": "01/01/2020",
        ///         "fechaFinal": "30/06/2020"
        ///     }
        ///     
        /// </remarks>
        /// <param name="listaAFPs"></param>
        /// <param name="listaFondos"></param>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns>Los valores cuotas y UF de las AFPs y fondos indicados.</returns>
        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<SalObtenerCuotas>> ObtenerCuotas(string listaAFPs, string listaFondos, string fechaInicial, string fechaFinal) {
            string[] afps = listaAFPs.ToUpper().Replace(" ", "").Split(",");
            string[] fondos = listaFondos.ToUpper().Replace(" ", "").Split(",");
            string[] diaMesAnnoInicio = fechaInicial.Split("/");
            string[] diaMesAnnoFinal = fechaFinal.Split("/");

            DateTime dtFechaInicio = new(
                int.Parse(diaMesAnnoInicio[2]),
                int.Parse(diaMesAnnoInicio[1]),
                int.Parse(diaMesAnnoInicio[0])
            );
            DateTime dtFechaFinal = new(
                int.Parse(diaMesAnnoFinal[2]),
                int.Parse(diaMesAnnoFinal[1]),
                int.Parse(diaMesAnnoFinal[0])
            );

            List<CuotaUf>? cuotas = await cuotaUfComisionDAO.ObtenerCuotas(afps, fondos, dtFechaInicio, dtFechaFinal);

            string jsonRetorno = JsonSerializer.Serialize(cuotas);
            int cantBytes = System.Text.Encoding.UTF8.GetByteCount(jsonRetorno);

            SalObtenerCuotas retorno = new();
            if (cantBytes > 5 * 1000 * 1000) {
                retorno.S3Url = await s3BucketHelper.UploadFile(jsonRetorno);
            } else {
                retorno.ListaCuotas = cuotas;
            }

            return retorno;
        }

        /// <summary>
        /// Obtiene el valor cuota y comisión para cada una de las fechas indicados según AFPs, fondos y tipo de comisión.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST CuotaUfComision/ObtenerUltimaCuota
        ///     {
        ///         "listaAFPs": "CAPITAL,CUPRUM",
        ///         "listaFondos": "A,B",
        ///         "listaFechas": "05/01/2020,05/02/2020",
        ///         "tipoComision": 1
        ///     }
        ///    
        /// </remarks>
        /// <param name="entrada"></param>
        /// <returns>Los valores cuotas y comisiones de las fechas indicadas.</returns>
        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<List<SalObtenerUltimaCuota>>> ObtenerUltimaCuota(EntObtenerUltimaCuota entrada) {
            string[] afps = entrada.ListaAFPs.ToUpper().Replace(" ", "").Split(",");
            string[] fondos = entrada.ListaFondos.ToUpper().Replace(" ", "").Split(",");
            string[] fechas = entrada.ListaFechas.Replace(" ", "").Split(",");

            List<SalObtenerUltimaCuota> retorno = [];
            foreach (string fecha in fechas) {
                string[] diaMesAnno = fecha.Split("/");

                DateTime dtFecha = new(
                        int.Parse(diaMesAnno[2]),
                        int.Parse(diaMesAnno[1]),
                        int.Parse(diaMesAnno[0]));

                foreach (string afp in afps) {
                    foreach (string fondo in fondos) {
                        CuotaUfComision? cuota = await cuotaUfComisionDAO.ObtenerUltimaCuota(afp, fondo, dtFecha);

                        if (cuota != null) {
                            decimal? comision;
                            if (entrada.TipoComision == Comision.TIPO_COMIS_DEPOS_COTIZ_OBLIG) {
                                comision = cuota.ComisDeposCotizOblig;
                            } else {
                                comision = cuota.ComisAdminCtaAhoVol;
                            }

                            retorno.Add(new SalObtenerUltimaCuota() {
                                Afp = cuota.Afp,
                                Fecha = cuota.Fecha,
                                Fondo = cuota.Fondo,
                                Valor = cuota.Valor,
                                Comision = comision
                            });
                        }
                    }
                }
            }

            return retorno;
        }

        /// <summary>
        /// Obtiene la rentabilidad real obtenida en un periodo de tiempo, para las AFPs y fondos indicados.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET CuotaUfComision/ObtenerRentabilidadReal
        ///     {
        ///         "listaAFPs": "CAPITAL,CUPRUM",
        ///         "listaFondos": "A,B",
        ///         "fechaInicial": "2020-01-01T00:00:00.000Z",
        ///         "fechaFinal": "2020-12-31T00:00:00.000Z"
        ///     }
        ///    
        /// </remarks>
        /// <param name="listaAFPs"></param>
        /// <param name="listaFondos"></param>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns>Los valores cuota y UF, inicial y final, junto a la rentabilidad real.</returns>
        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<List<RentabilidadReal>>> ObtenerRentabilidadReal(string listaAFPs, string listaFondos, DateTime fechaInicial, DateTime fechaFinal) {
            string[] afps = listaAFPs.ToUpper().Replace(" ", "").Split(",");
            string[] fondos = listaFondos.ToUpper().Replace(" ", "").Split(",");

            List<RentabilidadReal> retorno = [];
            foreach (string fondo in fondos) {
                foreach (string afp in afps) {
                    CuotaUfComision? cuotaInicial = await cuotaUfComisionDAO.ObtenerUltimaCuota(afp, fondo, fechaInicial);
                    CuotaUfComision? cuotaFinal = await cuotaUfComisionDAO.ObtenerUltimaCuota(afp, fondo, fechaFinal);

                    if (cuotaInicial?.ValorUf != null && cuotaFinal?.ValorUf != null) {
                        retorno.Add(new RentabilidadReal() { 
                            Afp = afp,
                            Fondo = fondo,
                            ValorCuotaInicial = cuotaInicial.Valor,
                            ValorUfInicial = cuotaInicial.ValorUf.Value,
                            ValorCuotaFinal = cuotaFinal.Valor,
                            ValorUfFinal = cuotaFinal.ValorUf.Value,
                            Rentabilidad = (cuotaFinal.Valor * cuotaInicial.ValorUf.Value / (cuotaInicial.Valor * cuotaFinal.ValorUf.Value) - 1) * 100
                        });
                    }
                }
			}
            return retorno;
        }

        /// <summary>
        /// Obtiene la última fecha para la cual se tienen registros de todos los valores cuotas para todas las AFPs y fondos.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET CuotaUfComision/UltimaFechaTodas
        ///     
        /// </remarks>
        /// <returns>Fecha de los últimos valores cuotas registrados.</returns>
        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<DateTime>> UltimaFechaTodas() {
            DateTime ultimaFecha = await cuotaUfComisionDAO.ObtenerUltimaFechaTodas();
            return ultimaFecha;
        }

        /// <summary>
        /// Obtiene la última fecha para la cual se tiene al menos un registro de valor cuota entre todas las AFPs y fondos.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET CuotaUfComision/UltimaFechaAlguna
        ///     
        /// </remarks>
        /// <returns>Fecha del último valor cuota registrado.</returns>
        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<DateTime>> UltimaFechaAlguna() {
            DateTime ultimaFecha = await cuotaUfComisionDAO.ObtenerUltimaFechaAlguna();
            return ultimaFecha;
        }
    }
}
