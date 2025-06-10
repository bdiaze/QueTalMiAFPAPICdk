using QueTalMiAFPAPI.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Interfaces {
    public interface ITipoMensajeDAO {
        Task<List<TipoMensaje>> ObtenerTiposMensaje(byte vigencia = 1);
        Task<TipoMensaje?> ObtenerTipoMensaje(short idTipoMensaje);
    }
}
