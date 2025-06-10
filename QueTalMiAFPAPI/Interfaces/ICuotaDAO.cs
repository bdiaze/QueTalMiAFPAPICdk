using System.Threading.Tasks;
using System;
using QueTalMiAFPAPI.Entities;

namespace QueTalMiAFPAPI.Interfaces {
    public interface ICuotaDAO {
        Task<Cuota?> ObtenerCuota(string afp, DateTime fecha, string fondo);
        Task InsertarCuota(Cuota cuota);
        Task ActualizarCuota(Cuota cuota);
    }
}
