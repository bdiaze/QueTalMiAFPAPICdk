using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using QueTalMiAFPAPI.Entities;

namespace QueTalMiAFPAPI.Interfaces {
    public interface IMensajeUsuarioDAO {
        Task<List<MensajeUsuario>> ObtenerMensajesUsuarios(DateTime fechaDesde, DateTime fechaHasta);
        Task<MensajeUsuario> IngresarMensajeUsuario(short idTipoMensaje, DateTime fechaIngreso, string nombre, string correo, string mensaje);
    }
}
