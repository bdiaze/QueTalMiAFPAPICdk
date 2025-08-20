namespace QueTalMiAFPAPI.Helpers {
    public class EnvironmentVariable(IConfiguration config) {
        public string GetValue(string nombre) {
#if DEBUG
            return config[$"Develop:EnvironmentVariable:{nombre}"]!;
#else
            return Environment.GetEnvironmentVariable(nombre) ?? throw new ArgumentNullException(nombre);
#endif
        }
    }
}
