using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using QueTalMiAFPAPI.Entities;
using QueTalMiAFPAPI.Models;

namespace QueTalMiAFPAPI.Interfaces {
    public interface ICuotaUfComisionDAO {
        Task<List<CuotaUf>?> ObtenerCuotas(string[] afps, string[] fondos, DateTime dtFechaInicio, DateTime dtFechaFinal);
        Task<CuotaUfComision?> ObtenerUltimaCuota(string afp, string fondo, DateTime dtFecha);
        Task<DateTime> ObtenerUltimaFechaTodas();
        Task<DateTime> ObtenerUltimaFechaAlguna();
    }
}
