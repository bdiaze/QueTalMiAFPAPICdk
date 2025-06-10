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
    public class UfDAO(ConnectionString connectionString) : IUfDAO {
        public async Task<Uf?> ObtenerUf(DateTime fecha) {
            Uf? uf = null;

            using (NpgsqlConnection connection = new(await connectionString.GetValue())) {
                using NpgsqlCommand command = new();
                command.Parameters.AddWithValue("@Fecha", fecha);

                string queryString = "SELECT UF.\"ID\", UF.\"FECHA\", UF.\"VALOR\" " +
                    "FROM \"QueTalMiAFP\".\"UF\" UF " +
                    "WHERE UF.\"FECHA\" = @Fecha;";

                command.CommandText = queryString;
                command.Connection = connection;

                await connection.OpenAsync();
                DbDataReader reader = await command.ExecuteReaderAsync();
                bool existe = await reader.ReadAsync();
                if (existe) {
                    uf = new Uf() {
                        Id = reader.GetInt64(0),
                        Fecha = reader.GetDateTime(1),
                        Valor = reader.GetDecimal(2)
                    };
                }
                await reader.CloseAsync();
            }

            return uf;
        }

        public async Task InsertarUf(Uf uf) {
            using NpgsqlConnection connection = new(await connectionString.GetValue());
            using NpgsqlCommand command = new();
            command.Parameters.AddWithValue("@Fecha", uf.Fecha);
            command.Parameters.AddWithValue("@Valor", uf.Valor);

            string queryString = "INSERT INTO \"QueTalMiAFP\".\"UF\"(" +
                "\"FECHA\", " +
                "\"VALOR\"" +
                ") VALUES (" +
                "@Fecha, " +
                "@Valor" +
                ") " +
                "RETURNING \"ID\";";

            command.CommandText = queryString;
            command.Connection = connection;

            await connection.OpenAsync();
            uf.Id = (long)(await command.ExecuteScalarAsync())!;
        }

        public async Task ActualizarUf(Uf uf) {
            using NpgsqlConnection connection = new(await connectionString.GetValue());
            using NpgsqlCommand command = new();
            command.Parameters.AddWithValue("@Id", uf.Id);
            command.Parameters.AddWithValue("@Fecha", uf.Fecha);
            command.Parameters.AddWithValue("@Valor", uf.Valor);

            string queryString = "UPDATE \"QueTalMiAFP\".\"UF\" " +
                "SET \"FECHA\" = @Fecha, " +
                "\"VALOR\" = @Valor " +
                "WHERE \"ID\" = @Id;";

            command.CommandText = queryString;
            command.Connection = connection;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}
