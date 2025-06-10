using Microsoft.Extensions.Configuration;
using Npgsql;
using QueTalMiAFPAPI.Entities;
using QueTalMiAFPAPI.Helpers;
using QueTalMiAFPAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace QueTalMiAFPAPI.Repositories {

    [ExcludeFromCodeCoverage]
    public class CuotaDAO(ConnectionString connectionString) : ICuotaDAO {
        public async Task<Cuota?> ObtenerCuota(string afp, DateTime fecha, string fondo) {
            Cuota? cuota = null;

            using (NpgsqlConnection connection = new(await connectionString.GetValue())) {
                using NpgsqlCommand command = new();
                command.Parameters.AddWithValue("@Afp", afp);
                command.Parameters.AddWithValue("@Fecha", fecha);
                command.Parameters.AddWithValue("@Fondo", fondo);

                string queryString = "SELECT CU.\"ID\", CU.\"AFP\", CU.\"FECHA\", CU.\"FONDO\", CU.\"VALOR\" " +
                    "FROM \"QueTalMiAFP\".\"CUOTA\" CU " +
                    "WHERE CU.\"AFP\" = @Afp " +
                    "AND CU.\"FECHA\" = @Fecha " +
                    "AND CU.\"FONDO\" = @Fondo;";

                command.CommandText = queryString;
                command.Connection = connection;

                await connection.OpenAsync();
                DbDataReader reader = await command.ExecuteReaderAsync();
                bool existe = await reader.ReadAsync();
                if (existe) {
                    cuota = new Cuota() {
                        Id = reader.GetInt64(0),
                        Afp = reader.GetString(1),
                        Fecha = reader.GetDateTime(2),
                        Fondo = reader.GetString(3),
                        Valor = reader.GetDecimal(4)
                    };
                }
                await reader.CloseAsync();
            }

            return cuota;
        }

        public async Task InsertarCuota(Cuota cuota) {
            using NpgsqlConnection connection = new(await connectionString.GetValue());
            using NpgsqlCommand command = new();
            command.Parameters.AddWithValue("@Afp", cuota.Afp);
            command.Parameters.AddWithValue("@Fecha", cuota.Fecha);
            command.Parameters.AddWithValue("@Fondo", cuota.Fondo);
            command.Parameters.AddWithValue("@Valor", cuota.Valor);

            string queryString = "INSERT INTO \"QueTalMiAFP\".\"CUOTA\"(\"AFP\", " +
                "\"FECHA\", " +
                "\"FONDO\", " +
                "\"VALOR\"" +
                ") VALUES (" +
                "@Afp, " +
                "@Fecha, " +
                "@Fondo, " +
                "@Valor" +
                ") " +
                "RETURNING \"ID\";";

            command.CommandText = queryString;
            command.Connection = connection;

            await connection.OpenAsync();
            cuota.Id = (long)(await command.ExecuteScalarAsync())!;
        }

        public async Task ActualizarCuota(Cuota cuota) {
            using NpgsqlConnection connection = new(await connectionString.GetValue());
            using NpgsqlCommand command = new();
            command.Parameters.AddWithValue("@Id", cuota.Id);
            command.Parameters.AddWithValue("@Afp", cuota.Afp);
            command.Parameters.AddWithValue("@Fecha", cuota.Fecha);
            command.Parameters.AddWithValue("@Fondo", cuota.Fondo);
            command.Parameters.AddWithValue("@Valor", cuota.Valor);

            string queryString = "UPDATE \"QueTalMiAFP\".\"CUOTA\" " +
                "SET \"AFP\" = @Afp, " +
                "\"FECHA\" = @Fecha, " +
                "\"FONDO\" = @Fondo, " +
                "\"VALOR\" = @Valor " +
                "WHERE \"ID\" = @Id;";

            command.CommandText = queryString;
            command.Connection = connection;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}
