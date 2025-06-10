using Microsoft.Extensions.Configuration;
using Npgsql;
using QueTalMiAFPAPI.Entities;
using QueTalMiAFPAPI.Helpers;
using QueTalMiAFPAPI.Interfaces;
using QueTalMiAFPAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Repositories {

    [ExcludeFromCodeCoverage]
    public class CuotaUfComisionDAO(ConnectionString connectionString) : ICuotaUfComisionDAO {
        public async Task<List<CuotaUf>?> ObtenerCuotas(string[] afps, string[] fondos, DateTime dtFechaInicio, DateTime dtFechaFinal) {
            List<CuotaUf>? cuotas = [];

            using (NpgsqlConnection connection = new(await connectionString.GetValue())) {
                using NpgsqlCommand command = new();
                string[] parametrosAfp = new string[afps.Length];
                for (int i = 0; i < afps.Length; i++) {
                    parametrosAfp[i] = "@Afp" + i;
                    command.Parameters.AddWithValue(parametrosAfp[i], afps[i]);
                }

                string[] parametrosFondo = new string[fondos.Length];
                for (int i = 0; i < fondos.Length; i++) {
                    parametrosFondo[i] = "@Fondo" + i;
                    command.Parameters.AddWithValue(parametrosFondo[i], fondos[i]);
                }
                command.Parameters.AddWithValue("@FechaInicial", dtFechaInicio);
                command.Parameters.AddWithValue("@FechaFinal", dtFechaFinal);

                string queryString = string.Format("SELECT \"AFP\", \"FECHA\", \"FONDO\", \"VALOR\", \"VALOR_UF\" " +
                    "FROM \"QueTalMiAFP\".\"CUOTA_UF_COMISION\" " +
                    "WHERE \"AFP\" IN ({0}) " +
                    "AND \"FONDO\" IN ({1}) " +
                    "AND \"FECHA\" >= @FechaInicial " +
                    "AND \"FECHA\" <= @FechaFinal " +
                    "AND \"VALOR_UF\" IS NOT NULL " +
                    "ORDER BY \"FECHA\", \"FONDO\", \"AFP\";",
                    string.Join(", ", parametrosAfp),
                    string.Join(", ", parametrosFondo)
                );

                command.CommandText = queryString;
                command.Connection = connection;

                await connection.OpenAsync();
                DbDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync()) {
                    cuotas.Add(new CuotaUf() {
                        Afp = reader.GetString(0),
                        Fecha = reader.GetDateTime(1).ToString("yyyy-MM-dd"),
                        Fondo = reader.GetString(2),
                        Valor = Math.Round(reader.GetDecimal(3), 2),
                        ValorUf = await reader.IsDBNullAsync(4) ? null : Math.Round(reader.GetDecimal(4), 2)
                    });
                }
                await reader.CloseAsync();
            }

            return cuotas;
        }

        public async Task<CuotaUfComision?> ObtenerUltimaCuota(string afp, string fondo, DateTime dtFecha) {
            CuotaUfComision? cuota = null;

            using (NpgsqlConnection connection = new(await connectionString.GetValue())) {
                using NpgsqlCommand command = new();
                command.Parameters.AddWithValue("@Afp", afp);
                command.Parameters.AddWithValue("@Fondo", fondo);
                command.Parameters.AddWithValue("@Fecha", dtFecha);

                string queryString = "SELECT CUC.\"AFP\", CUC.\"FECHA\", CUC.\"FONDO\", CUC.\"VALOR\", CUC.\"VALOR_UF\", " +
                    "CUC.\"COMIS_DEPOS_COTIZ_OBLIG\", CUC.\"COMIS_ADMIN_CTA_AHO_VOL\" " +
                    "FROM \"QueTalMiAFP\".\"CUOTA_UF_COMISION\" CUC " +
                    "WHERE CUC.\"AFP\" = @Afp " +
                    "AND CUC.\"FONDO\" = @Fondo " +
                    "AND CUC.\"FECHA\" = (SELECT MAX(CU.\"FECHA\") " +
                    "FROM \"QueTalMiAFP\".\"CUOTA\" CU " +
                    "WHERE CU.\"AFP\" = @Afp " +
                    "AND CU.\"FONDO\" = @Fondo " +
                    "AND CU.\"FECHA\" <= @Fecha);";

                command.CommandText = queryString;
                command.Connection = connection;

                await connection.OpenAsync();
                DbDataReader reader = await command.ExecuteReaderAsync();
                bool existe = await reader.ReadAsync();
                if (existe) {
                    cuota = new CuotaUfComision() {
                        Afp = reader.GetString(0),
                        Fecha = reader.GetDateTime(1),
                        Fondo = reader.GetString(2),
                        Valor = Math.Round(reader.GetDecimal(3), 2),
                        ValorUf = await reader.IsDBNullAsync(4) ? null : Math.Round(reader.GetDecimal(4), 2),
                        ComisDeposCotizOblig = await reader.IsDBNullAsync(5) ? null : Math.Round(reader.GetDecimal(5), 2),
                        ComisAdminCtaAhoVol = await reader.IsDBNullAsync(6) ? null : Math.Round(reader.GetDecimal(6), 2)
                    };
                }
                await reader.CloseAsync();
            }

            return cuota;
        }

        public async Task<DateTime> ObtenerUltimaFechaTodas() {
            DateTime ultimaFecha = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneConverter.TZConvert.GetTimeZoneInfo("Pacific SA Standard Time"));

            using (NpgsqlConnection connection = new(await connectionString.GetValue())) {
                using NpgsqlCommand command = new();
                string queryString = "SELECT MIN(TMP.\"MAX_FECHA\") " +
                    "FROM ( " +
                    "SELECT \"AFP\", \"FONDO\", MAX(\"FECHA\") AS \"MAX_FECHA\" " +
                    "FROM \"QueTalMiAFP\".\"CUOTA\" " +
                    "GROUP BY \"AFP\", \"FONDO\" " +
                    ") TMP;";

                command.CommandText = queryString;
                command.Connection = connection;

                await connection.OpenAsync();
                DbDataReader reader = await command.ExecuteReaderAsync();
                bool existe = await reader.ReadAsync();
                if (existe) {
                    ultimaFecha = reader.GetDateTime(0);
                }
                await reader.CloseAsync();
            }

            return ultimaFecha;
        }

        public async Task<DateTime> ObtenerUltimaFechaAlguna() {
            DateTime ultimaFecha = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneConverter.TZConvert.GetTimeZoneInfo("Pacific SA Standard Time"));

            using (NpgsqlConnection connection = new(await connectionString.GetValue())) {
                using NpgsqlCommand command = new();
                string queryString = "SELECT MAX(\"FECHA\") " +
                    "FROM \"QueTalMiAFP\".\"CUOTA\";";

                command.CommandText = queryString;
                command.Connection = connection;

                await connection.OpenAsync();
                DbDataReader reader = await command.ExecuteReaderAsync();
                bool existe = await reader.ReadAsync();
                if (existe) {
                    ultimaFecha = reader.GetDateTime(0);
                }
                await reader.CloseAsync();
            }

            return ultimaFecha;
        }

    }
}
