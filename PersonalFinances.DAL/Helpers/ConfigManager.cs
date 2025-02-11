
using DotNetEnv;
using Newtonsoft.Json;
using PersonalFinances.DAL.Helpers.FocusTrack.DAL.Helpers;

namespace PersonalFinances.DAL.Helpers
{
    public static class ConfigManager
    {
        private const string ConfigFilePath = "config.txt";

        public static string GetConnectionString()
        {
            try
            {
                if (!File.Exists(ConfigFilePath))
                {
                    CreateDefaultConfig();
                }

                var encryptedConfig = File.ReadAllText(ConfigFilePath);
                var decryptedConfig = Cryptography.Decrypt(encryptedConfig);
                var config = JsonConvert.DeserializeObject<Dictionary<string, string>>(decryptedConfig);

                if (config == null)
                    throw new InvalidOperationException("Falha ao desserializar o arquivo de configuração.");

                return $"Server={config["Server"]};Database={config["Database"]};User ID={config["User"]};Password={config["Password"]};TrustServerCertificate=True";
            }
            catch (Exception ex)
            {
                Logger.WriteLog($"Erro ao obter a string de conexão: {ex}", LogStatus.Error);
                throw;
            }
        }

        private static void CreateDefaultConfig()
        {
            Env.Load();

            var defaultConfig = new
            {
                Server = Environment.GetEnvironmentVariable("DB_SERVER"),
                Database = Environment.GetEnvironmentVariable("DB_DATABASE"),
                User = Environment.GetEnvironmentVariable("DB_USER"),
                Password = Environment.GetEnvironmentVariable("DB_PASSWORD"),
            };

            var json = JsonConvert.SerializeObject(defaultConfig);
            Cryptography.SaveEncryptedConfig(ConfigFilePath, json);
        }
    }

}
