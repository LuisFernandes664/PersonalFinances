
using PersonalFinances.DAL.Helpers.FocusTrack.DAL.Helpers;
using System.Data;
using System.Data.SqlClient;

namespace PersonalFinances.DAL.Helpers
{
    public class SQLHelper
    {

        public static async Task<int> ExecuteNonQueryAsync(string query, List<SqlParameter> parameters = null, SqlTransaction transaction = null)
        {
            return await ExecuteAsync(query, parameters, transaction, async command =>
            {
                return await command.ExecuteNonQueryAsync();
            });
        }

        public static async Task<DataTable> ExecuteQueryAsync(string query, List<SqlParameter> parameters = null, SqlTransaction transaction = null)
        {
            return await ExecuteAsync(query, parameters, transaction, async command =>
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    return dataTable;
                }
            });
        }

        public static async Task<DataRow> ExecuteScalarAsync(string query, List<SqlParameter> parameters = null, SqlTransaction transaction = null)
        {
            return await ExecuteAsync(query, parameters, transaction, async command =>
            {
                using (var adapter = new SqlDataAdapter(command))
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable.Rows.Count > 0 ? dataTable.Rows[0] : null;
                }
            });
        }

        private static async Task<T> ExecuteAsync<T>(string query, List<SqlParameter> parameters, SqlTransaction transaction, Func<SqlCommand, Task<T>> executeAction)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentException("A query não pode ser nula ou vazia.", nameof(query));

            SqlConnection connection = null;
            bool shouldCloseConnection = false;

            try
            {
                connection = transaction?.Connection ?? await OpenConnectionAsync();
                shouldCloseConnection = transaction == null;

                using (var command = new SqlCommand(query, connection, transaction))
                {
                    if (parameters != null && parameters.Count > 0)
                    {
                        command.Parameters.AddRange(parameters.ToArray());
                    }

                    return await executeAction(command);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog($"Erro ao executar operação na base de dados: {ex}", LogStatus.Error);
                throw;
            }
            finally
            {
                if (shouldCloseConnection && connection != null)
                {
                    await CloseConnectionAsync(connection);
                }
            }
        }

        private static async Task<SqlConnection> OpenConnectionAsync()
        {
            var connection = new SqlConnection(ConfigManager.GetConnectionString());
            await connection.OpenAsync();
            return connection;
        }

        private static async Task CloseConnectionAsync(SqlConnection connection)
        {
            if (connection?.State != ConnectionState.Closed)
            {
                await connection.CloseAsync();
            }
        }
    }
}
