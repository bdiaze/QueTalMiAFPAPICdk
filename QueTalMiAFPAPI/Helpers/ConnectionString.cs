namespace QueTalMiAFPAPI.Helpers {
    public class ConnectionString(EnvironmentVariable environmentVariable, IConfiguration config) {

        private string? _connectionString = null;

        public async Task<string> GetValue() {
            if (_connectionString != null) {
                return _connectionString;
            }

            string secretArnConnectionString = environmentVariable.GetValue("SECRET_ARN_CONNECTION_STRING");
            dynamic connectionString = await SecretManager.ObtenerSecreto(secretArnConnectionString);

#if DEBUG
            connectionString.Host = config["Develop:Database:Host"];
            connectionString.QueTalMiAFPAppUsername = config["Develop:Database:User"];
            connectionString.QueTalMiAFPAppPassword = config["Develop:Database:Pass"];
#endif

            _connectionString = $"Server={connectionString.Host};Port={connectionString.Port};SslMode=prefer;" +
                $"Database={connectionString.QueTalMiAFPDatabase};User Id={connectionString.QueTalMiAFPAppUsername};Password='{connectionString.QueTalMiAFPAppPassword}';";
            return _connectionString;
        }
    }
}
