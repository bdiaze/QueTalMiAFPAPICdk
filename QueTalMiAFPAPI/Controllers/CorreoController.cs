using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using QueTalMiAFPAPI.Models;
using QueTalMiAFPAPI.Helpers;
using System.Text;
using Newtonsoft.Json;

namespace QueTalMiAFPAPI.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class CorreoController : ControllerBase {

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<RespuestaCorreo>> Enviar(Correo correo) {
            Stopwatch stopwatch = Stopwatch.StartNew();

            if (correo.De == null) {
                string parameterArnSesDireccionDeDefecto = Environment.GetEnvironmentVariable("PARAMETER_ARN_SES_DIRECCION_DE_DEFECTO") ?? throw new ArgumentNullException("PARAMETER_ARN_SES_DIRECCION_DE_DEFECTO");
                string strDireccionDeDefecto = await ParameterStore.ObtenerParametro(parameterArnSesDireccionDeDefecto) ?? throw new ArgumentNullException("PARAMETER_SES_DIRECCION_DE_DEFECTO");
                dynamic? direccionDeDefecto = JsonConvert.DeserializeObject<dynamic>(strDireccionDeDefecto);

                correo.De = new DireccionCorreo { 
                    Nombre = direccionDeDefecto?.Nombre,
                    Correo = direccionDeDefecto?.Correo!,
                };
            }

            List<Attachment>? attachments = null;
            if (correo.Adjuntos != null && correo.Adjuntos.Count > 0) {
                attachments = [];
                foreach(Adjunto adjunto in correo.Adjuntos) {
                    attachments.Add(new Attachment {
                        FileName = adjunto.NombreArchivo,
                        ContentType = adjunto.TipoMime,
                        RawContent = new MemoryStream(Convert.FromBase64String(adjunto.ContenidoBase64))
                    });
                }
            }

            try {
                using AmazonSimpleEmailServiceV2Client client = new();
                SendEmailRequest request = new() {
                    FromEmailAddress = correo.De.ToString(),
                    Destination = new Destination {
                        ToAddresses = [.. correo.Para.Select(c => c.ToString())],
                        CcAddresses = correo.Cc?.Select(c => c.ToString()).ToList(),
                        BccAddresses = correo.Cco?.Select(c => c.ToString()).ToList(),
                    },
                    ReplyToAddresses = correo.ResponderA?.Select(c => c.ToString()).ToList(),
                    Content = new EmailContent {
                        Simple = new Message {
                            Subject = new Content {
                                Charset = "UTF-8",
                                Data = correo.Asunto
                            },
                            Body = new Body {
                                Html = new Content {
                                    Charset = "UTF-8",
                                    Data = correo.Cuerpo
                                }
                            },
                            Attachments = attachments,
                        }
                    }
                };

                SendEmailResponse response = await client.SendEmailAsync(request);
                RespuestaCorreo salida = new() {
                    IdMensaje = response.MessageId,
                    CodigoEstado = (int)response.HttpStatusCode
                };

                LambdaLogger.Log(
                    $"[POST] - [CorreoController] - [Enviar] - [{stopwatch.ElapsedMilliseconds} ms] - [{StatusCodes.Status200OK}] - " +
                    $"Envío de correo exitoso - ID Mensaje: {salida.IdMensaje}.");

                return salida;
            } catch (Exception ex) {
                LambdaLogger.Log(
                    $"[POST] - [CorreoController] - [Enviar] - [{stopwatch.ElapsedMilliseconds} ms] - [{StatusCodes.Status500InternalServerError}] - " +
                    $"Ocurrió un error al enviar correo. " +
                    $"{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
