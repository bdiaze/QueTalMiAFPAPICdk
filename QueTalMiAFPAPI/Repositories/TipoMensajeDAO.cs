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
    public class TipoMensajeDAO(ConnectionString connectionString) : ITipoMensajeDAO {
        public async Task<List<TipoMensaje>> ObtenerTiposMensaje(byte vigencia = 1) {
            List<TipoMensaje> tiposMensaje = [];

            using (NpgsqlConnection connection = new(await connectionString.GetValue())) {
                using NpgsqlCommand command = new();
                command.Parameters.AddWithValue("@Vigencia", vigencia);

                string queryString = "SELECT TM.\"ID_TIPO_MENSAJE\", TM.\"DESCRIPCION_CORTA\", TM.\"DESCRIPCION_LARGA\", TM.\"VIGENCIA\" " +
                    "FROM \"QueTalMiAFP\".\"TIPO_MENSAJE\" TM " +
                    "WHERE TM.\"VIGENCIA\" = @Vigencia " +
                    "ORDER BY TM.\"ID_TIPO_MENSAJE\" DESC;";

                command.CommandText = queryString;
                command.Connection = connection;

                await connection.OpenAsync();
                DbDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync()) {
                    tiposMensaje.Add(new TipoMensaje() {
                        IdTipoMensaje = reader.GetInt16(0),
                        DescripcionCorta = reader.GetString(1),
                        DescripcionLarga = reader.GetString(2),
                        Vigencia = reader.GetByte(3)
                    });
                }
                await reader.CloseAsync();
            }

            return tiposMensaje;
        }

        public async Task<TipoMensaje?> ObtenerTipoMensaje(short idTipoMensaje) {
            TipoMensaje? tipoMensaje = null;

            using (NpgsqlConnection connection = new(await connectionString.GetValue())) {
                using NpgsqlCommand command = new();
                command.Parameters.AddWithValue("@IdTipoMensaje", idTipoMensaje);

                string queryString = "SELECT TM.\"ID_TIPO_MENSAJE\", TM.\"DESCRIPCION_CORTA\", TM.\"DESCRIPCION_LARGA\", TM.\"VIGENCIA\" " +
                    "FROM \"QueTalMiAFP\".\"TIPO_MENSAJE\" TM " +
                    "WHERE TM.\"ID_TIPO_MENSAJE\" = @IdTipoMensaje;";

                command.CommandText = queryString;
                command.Connection = connection;

                await connection.OpenAsync();
                DbDataReader reader = await command.ExecuteReaderAsync();
                bool existe = await reader.ReadAsync();
                if (existe) {
                    tipoMensaje = new TipoMensaje() {
                        IdTipoMensaje = reader.GetInt16(0),
                        DescripcionCorta = reader.GetString(1),
                        DescripcionLarga = reader.GetString(2),
                        Vigencia = reader.GetByte(3)
                    };
                }
                await reader.CloseAsync();
            }

            return tipoMensaje;
        }
    }
}
