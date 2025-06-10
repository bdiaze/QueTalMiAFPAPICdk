using System.Threading.Tasks;
using System;
using QueTalMiAFPAPI.Entities;

namespace QueTalMiAFPAPI.Interfaces {
    public interface IUfDAO {
        Task<Uf?> ObtenerUf(DateTime fecha);
        Task InsertarUf(Uf uf);
        Task ActualizarUf(Uf uf);
    }
}
