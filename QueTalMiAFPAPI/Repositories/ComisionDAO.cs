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
    public class ComisionDAO(ConnectionString connectionString) : IComisionDAO {
        public async Task<Comision?> ObtenerComision(byte tipoComision, string afp, DateTime fecha) {
            Comision? comision = null;

            using (NpgsqlConnection connection = new(await connectionString.GetValue())) {
                using NpgsqlCommand command = new();
                command.Parameters.AddWithValue("@TipoComision", tipoComision);
                command.Parameters.AddWithValue("@Afp", afp);
                command.Parameters.AddWithValue("@Fecha", fecha);

                string queryString = "SELECT CO.\"ID\", CO.\"AFP\", CO.\"FECHA\", CO.\"VALOR\", CO.\"TIPO_COMISION\", CO.\"TIPO_VALOR\" " +
                    "FROM \"QueTalMiAFP\".\"COMISION\" CO " +
                    "WHERE CO.\"TIPO_COMISION\" = @TipoComision " +
                    "AND CO.\"AFP\" = @Afp " +
                    "AND CO.\"FECHA\" = @Fecha;";

                command.CommandText = queryString;
                command.Connection = connection;

                await connection.OpenAsync();
                DbDataReader reader = await command.ExecuteReaderAsync();
                bool existe = await reader.ReadAsync();
                if (existe) {
                    comision = new Comision() {
                        Id = reader.GetInt64(0),
                        Afp = reader.GetString(1),
                        Fecha = reader.GetDateTime(2),
                        Valor = reader.GetDecimal(3),
                        TipoComision = reader.GetByte(4),
                        TipoValor = reader.GetByte(5)
                    };
                }
                await reader.CloseAsync();
            }

            return comision;
        }
        public async Task InsertarComision(Comision comision) {
            using NpgsqlConnection connection = new(await connectionString.GetValue());
            using NpgsqlCommand command = new();
            command.Parameters.AddWithValue("@Afp", comision.Afp);
            command.Parameters.AddWithValue("@Fecha", comision.Fecha);
            command.Parameters.AddWithValue("@Valor", comision.Valor);
            command.Parameters.AddWithValue("@TipoComision", comision.TipoComision);
            command.Parameters.AddWithValue("@TipoValor", comision.TipoValor);

            string queryString = "INSERT INTO \"QueTalMiAFP\".\"COMISION\"(" +
                "\"AFP\", " +
                "\"FECHA\", " +
                "\"VALOR\", " +
                "\"TIPO_COMISION\", " +
                "\"TIPO_VALOR\"" +
                ") VALUES (" +
                "@Afp, " +
                "@Fecha, " +
                "@Valor, " +
                "@TipoComision, " +
                "@TipoValor" +
                ") " +
                "RETURNING \"ID\";";

            command.CommandText = queryString;
            command.Connection = connection;

            await connection.OpenAsync();
            comision.Id = (long)(await command.ExecuteScalarAsync())!;
        }

        public async Task ActualizarComision(Comision comision) {
            using NpgsqlConnection connection = new(await connectionString.GetValue());
            using NpgsqlCommand command = new();
            command.Parameters.AddWithValue("@Id", comision.Id);
            command.Parameters.AddWithValue("@Afp", comision.Afp);
            command.Parameters.AddWithValue("@Fecha", comision.Fecha);
            command.Parameters.AddWithValue("@Valor", comision.Valor);
            command.Parameters.AddWithValue("@TipoComision", comision.TipoComision);
            command.Parameters.AddWithValue("@TipoValor", comision.TipoValor);

            string queryString = "UPDATE \"QueTalMiAFP\".\"COMISION\" " +
                "SET \"AFP\" = @Afp, " +
                "\"FECHA\" = @Fecha, " +
                "\"VALOR\" = @Valor, " +
                "\"TIPO_COMISION\" = @TipoComision, " +
                "\"TIPO_VALOR\" = @TipoValor " +
                "WHERE \"ID\" = @Id;";

            command.CommandText = queryString;
            command.Connection = connection;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}
