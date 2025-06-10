namespace QueTalMiAFPAPI.Helpers {
    public class ConnectionString {

        private string? _connectionString = null;

        public async Task<string> GetValue() {
            if (_connectionString != null) {
                return _connectionString;
            }

            string secretArnConnectionString = Environment.GetEnvironmentVariable("SECRET_ARN_CONNECTION_STRING") ?? throw new ArgumentNullException("SECRET_ARN_CONNECTION_STRING");
            dynamic connectionString = await SecretManager.ObtenerSecreto(secretArnConnectionString);

            _connectionString = $"Server={connectionString.Host};Port={connectionString.Port};SslMode=prefer;" +
                $"Database={connectionString.QueTalMiAFPDatabase};User Id={connectionString.QueTalMiAFPAppUsername};Password='{connectionString.QueTalMiAFPAppPassword}';";
            return _connectionString;
        }
    }
}
