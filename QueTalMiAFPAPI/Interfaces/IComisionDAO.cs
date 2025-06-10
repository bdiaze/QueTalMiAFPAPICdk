using QueTalMiAFPAPI.Entities;
using System;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Interfaces {
    public interface IComisionDAO {
        Task<Comision?> ObtenerComision(byte tipoComision, string afp, DateTime fecha);
        Task InsertarComision(Comision comision);
        Task ActualizarComision(Comision comision);
    }
}
