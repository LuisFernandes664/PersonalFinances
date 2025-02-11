
using PersonalFinances.BLL.Entities.DB;
using PersonalFinances.DAL.Helpers.FocusTrack.DAL.Helpers;
using System.Data.SqlClient;

namespace PersonalFinances.DAL.Helpers
{
    public static class DatabaseHelper
    { 

        private static string connectionString;

        public static async Task CreateTablesAsync()
        {
            connectionString = ConfigManager.GetConnectionString();

            var tables = new List<TableDefinition>
            {
                new TableDefinition
                {
                    TableName = "Users",
                    Columns = new Dictionary<string, ColumnDefinition>
                    {
                        { "stamp_entity", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = true, IsNullable = false, DefaultValue = "NEWID()" } },
                        { "user_name", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = false, IsNullable = false } },
                        { "password", new ColumnDefinition { DataType = "NVARCHAR(255)", IsPrimaryKey = false, IsNullable = false } },
                        { "email", new ColumnDefinition { DataType = "NVARCHAR(100)", IsPrimaryKey = false, IsNullable = true } },
                        { "phone_number", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = false, IsNullable = true } },
                        { "created_at", new ColumnDefinition { DataType = "DATETIME", IsPrimaryKey = false, IsNullable = false, DefaultValue = "GETDATE()" } },
                        { "updated_at", new ColumnDefinition { DataType = "DATETIME", IsPrimaryKey = false, IsNullable = false, DefaultValue = "GETDATE()" } },
                    }
                },
                new TableDefinition
                {
                    TableName = "Notifications",
                    Columns = new Dictionary<string, ColumnDefinition>
                    {
                        { "stamp_entity", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = true, IsNullable = false, DefaultValue = "NEWID()" } },
                        { "user_stamp", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = false, IsNullable = false, IsForeignKey = true, ForeignKeyTable = "Users", ForeignKeyColumn = "stamp_entity" } },
                        { "type", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = false, IsNullable = false } },
                        { "message", new ColumnDefinition { DataType = "NVARCHAR(255)", IsPrimaryKey = false, IsNullable = false } },
                        { "created_at", new ColumnDefinition { DataType = "DATETIME", IsPrimaryKey = false, IsNullable = false, DefaultValue = "GETDATE()" } },
                        { "updated_at", new ColumnDefinition { DataType = "DATETIME", IsPrimaryKey = false, IsNullable = false, DefaultValue = "GETDATE()" } },
                    }
                },
                new TableDefinition
                {
                    TableName = "PasswordResetRequests",
                    Columns = new Dictionary<string, ColumnDefinition>
                    {
                        { "stamp_entity", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = true, IsNullable = false, DefaultValue = "NEWID()" } },
                        { "user_stamp", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = false, IsNullable = false, IsForeignKey = true, ForeignKeyTable = "Users", ForeignKeyColumn = "stamp_entity" } },
                        { "token", new ColumnDefinition { DataType = "NVARCHAR(200)", IsPrimaryKey = false, IsNullable = false } },
                        { "created_at", new ColumnDefinition { DataType = "DATETIME", IsPrimaryKey = false, IsNullable = false, DefaultValue = "GETDATE()" } },
                        { "expires_at", new ColumnDefinition { DataType = "DATETIME", IsPrimaryKey = false, IsNullable = false } },
                        { "used", new ColumnDefinition { DataType = "BIT", IsPrimaryKey = false, IsNullable = false } }
                    }
                }

            };


            if (!string.IsNullOrEmpty(connectionString))
            {
                using (var dbContext = new DatabaseContext())
                {

                    try
                    {
                        var successLogs = new List<string>();
                        foreach (var table in tables)
                        {
                            var createTableQuery = GenerateCreateTableQuery(table);
                            await SQLHelper.ExecuteNonQueryAsync(createTableQuery, transaction: dbContext.Transaction);
                            await EnsureColumnsExistAsync(dbContext.Connection, table.TableName, table.Columns, dbContext.Transaction);
                            successLogs.Add($"Consitencia da tabela: {table.TableName} verificada com sucesso!");
                        }

                        dbContext.Commit();
                        successLogs.ForEach(logMessage => Logger.WriteLog(logMessage, LogStatus.Success));
                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        Logger.WriteLog(string.Concat("Erro ao executar CreateTablesAsync(): ", ex), LogStatus.Error);
                        throw;
                    }
                }
            }
        }

        private static async Task EnsureColumnsExistAsync(SqlConnection connection, string tableName, Dictionary<string, ColumnDefinition> columns, SqlTransaction transaction)
        {
            foreach (var column in columns)
            {
                var columnDef = column.Value.DataType + (column.Value.IsNullable ? " NULL" : " NOT NULL");
                await AddColumnIfNotExistsAsync(
                    connection,
                    tableName,
                    column.Key,
                    columnDef,
                    transaction,
                    column.Value.DefaultValue
                );
            }
        }
         
        private static async Task AddColumnIfNotExistsAsync(SqlConnection connection, string tableName, string columnName, string columnDefinition, SqlTransaction transaction, string defaultValue = null)
        {
            string defaultClause = !string.IsNullOrEmpty(defaultValue) ? $"DEFAULT {defaultValue}" : "";

            string query = $@"
                IF NOT EXISTS (
                    SELECT * FROM sys.columns 
                    WHERE Name = N'{columnName}' 
                    AND Object_ID = Object_ID(N'{tableName}')
                )
                BEGIN
                    -- Adiciona a coluna com valor padrão se necessário
                    IF EXISTS (SELECT 1 FROM {tableName})
                    BEGIN
                        ALTER TABLE {tableName} ADD {columnName} {columnDefinition} {defaultClause};
                    END
                    ELSE
                    BEGIN
                        ALTER TABLE {tableName} ADD {columnName} {columnDefinition};
                    END
                END";

            using (var command = new SqlCommand(query, connection, transaction))
            {
                await command.ExecuteNonQueryAsync();
            }
        }



        private static string GenerateCreateTableQuery(TableDefinition table)
        {
            // Definir as colunas
            var columnDefinitions = table.Columns.Select(col =>
                $"[{col.Key}] {col.Value.DataType} {(col.Value.IsNullable ? "NULL" : "NOT NULL")}"
            ).ToArray();

            // Definir as primaryKeys
            var primaryKeys = table.Columns.Where(col => col.Value.IsPrimaryKey).Select(col => $"[{col.Key}]").ToArray();
            var pkConstraint = primaryKeys.Any() ? $", PRIMARY KEY ({string.Join(", ", primaryKeys)})" : string.Empty;

            // Definir as ForeignKeys
            var foreignKeys = table.Columns.Where(col => col.Value.IsForeignKey).Select(col =>
                $"FOREIGN KEY ({col.Key}) REFERENCES {col.Value.ForeignKeyTable}({col.Value.ForeignKeyColumn})"
            ).ToArray();
            var fkConstraint = foreignKeys.Any() ? $", {string.Join(", ", foreignKeys)}" : string.Empty;

            // Criar índice, se necessário
            var indexes = table.Columns.Where(col => col.Value.HasIndex).Select(col =>
                $"CREATE INDEX IX_{table.TableName}_{col.Key} ON {table.TableName}({col.Key})"
            ).ToArray();

            var indexStatements = indexes.Any() ? string.Join(";", indexes) : string.Empty;

            // Gerar a consulta SQL para criação da tabela
            return $@"
                IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{table.TableName}]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[{table.TableName}] (
                        {string.Join(", ", columnDefinitions)}{pkConstraint}{fkConstraint}
                    );
                END;

                {indexStatements}
                ";
        }


    }
}
