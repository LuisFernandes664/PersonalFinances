
using PersonalFinances.BLL.Entities.DB;
using PersonalFinances.DAL.Helpers.FocusTrack.DAL.Helpers;
using System.Data.SqlClient;

namespace PersonalFinances.DAL.Helpers
{
    public static class DatabaseHelper
    { 

        private static string connectionString;

        /// <summary>
        /// Cria ou atualiza as tabelas do banco de dados conforme as definições
        /// </summary>
        public static async Task CreateTablesAsync()
        {
            connectionString = ConfigManager.GetConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
                Logger.WriteLog("String de conexão não configurada", LogStatus.Error);
                throw new InvalidOperationException("Connection string não configurada");
            }

            var tables = GetTableDefinitions();

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
                        successLogs.Add($"Consistência da tabela: {table.TableName} verificada com sucesso!");
                    }

                    await PopulateCategoriesAsync(dbContext.Connection, dbContext.Transaction);
                    dbContext.Commit();
                    successLogs.ForEach(logMessage => Logger.WriteLog(logMessage, LogStatus.Success));
                }
                catch (SqlException ex)
                {
                    dbContext.Rollback();
                    Logger.WriteLog($"Erro SQL ao executar CreateTablesAsync(): {ex.Message}", LogStatus.Error);
                    throw;
                }
                catch (Exception ex)
                {
                    dbContext.Rollback();
                    Logger.WriteLog($"Erro ao executar CreateTablesAsync(): {ex.Message}", LogStatus.Error);
                    throw;
                }
            }
        }

        /// <summary>
        /// Retorna as definições de todas as tabelas do sistema
        /// </summary>
        private static List<TableDefinition> GetTableDefinitions()
        {
            return new List<TableDefinition>
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
                },
                new TableDefinition
                {
                    TableName = "Transactions",
                    Columns = new Dictionary<string, ColumnDefinition>
                    {
                        { "stamp_entity", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = true, IsNullable = false, DefaultValue = "NEWID()" } },
                        { "user_stamp", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = true, IsNullable = false, DefaultValue = "NEWID()" } },
                        { "description", new ColumnDefinition { DataType = "NVARCHAR(255)", IsPrimaryKey = false, IsNullable = false } },
                        { "amount", new ColumnDefinition { DataType = "DECIMAL(18,2)", IsPrimaryKey = false, IsNullable = false } },
                        { "date", new ColumnDefinition { DataType = "DATE", IsPrimaryKey = false, IsNullable = false } },
                        { "category", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = false, IsNullable = false } },
                        { "paymentMethod", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = false, IsNullable = false } },
                        { "recipient", new ColumnDefinition { DataType = "NVARCHAR(255)", IsPrimaryKey = false, IsNullable = false } },
                        { "status", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = false, IsNullable = false } },
                        { "created_at", new ColumnDefinition { DataType = "DATETIME", IsPrimaryKey = false, IsNullable = false, DefaultValue = "GETDATE()" } },
                        { "updated_at", new ColumnDefinition { DataType = "DATETIME", IsPrimaryKey = false, IsNullable = false, DefaultValue = "GETDATE()" } },
                    }
                },
                // Tabela de Categorias compartilhada entre Budgets e Goals
                new TableDefinition
                {
                    TableName = "Categories",
                    Columns = new Dictionary<string, ColumnDefinition>
                    {
                        { "stamp_entity", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = true, IsNullable = false, DefaultValue = "NEWID()" } },
                        { "name", new ColumnDefinition { DataType = "NVARCHAR(100)", IsPrimaryKey = false, IsNullable = false } },
                        { "type", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = false, IsNullable = false } } // Ex: "budget", "goal"
                    }
                },

                // Tabela Budgets (Orçamentos)
                new TableDefinition
                {
                    TableName = "Budgets",
                    Columns = new Dictionary<string, ColumnDefinition>
                    {
                        { "stamp_entity", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = true, IsNullable = false, DefaultValue = "NEWID()" } },
                        { "user_id", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = false, IsNullable = false } },
                        { "category_id", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = false, IsNullable = false, IsForeignKey = true, ForeignKeyTable = "Categories", ForeignKeyColumn = "stamp_entity" } },
                        { "valor_orcado", new ColumnDefinition { DataType = "DECIMAL(18,2)", IsPrimaryKey = false, IsNullable = false } },
                        { "data_inicio", new ColumnDefinition { DataType = "DATE", IsPrimaryKey = false, IsNullable = false } },
                        { "data_fim", new ColumnDefinition { DataType = "DATE", IsPrimaryKey = false, IsNullable = false } },
                        { "created_at", new ColumnDefinition { DataType = "DATETIME", IsPrimaryKey = false, IsNullable = false, DefaultValue = "GETDATE()" } }
                    }
                },

                // Histórico de gastos para budgets
                new TableDefinition
                {
                    TableName = "BudgetHistory",
                    Columns = new Dictionary<string, ColumnDefinition>
                    {
                        { "stamp_entity", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = true, IsNullable = false, DefaultValue = "NEWID()" } },
                        { "budget_id", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = false, IsNullable = false, IsForeignKey = true, ForeignKeyTable = "Budgets", ForeignKeyColumn = "stamp_entity" } },
                        { "transaction_id", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = false, IsNullable = false, IsForeignKey = true, ForeignKeyTable = "Transactions", ForeignKeyColumn = "stamp_entity" } },
                        { "valor_gasto", new ColumnDefinition { DataType = "DECIMAL(18,2)", IsPrimaryKey = false, IsNullable = false } },
                        { "data_registro", new ColumnDefinition { DataType = "DATETIME", IsPrimaryKey = false, IsNullable = false, DefaultValue = "GETDATE()" } }
                    }
                },

                // Tabela Goals (Metas Financeiras)
                new TableDefinition
                {
                    TableName = "Goals",
                    Columns = new Dictionary<string, ColumnDefinition>
                    {
                        { "stamp_entity", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = true, IsNullable = false, DefaultValue = "NEWID()" } },
                        { "user_id", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = false, IsNullable = false } },
                        { "category_id", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = false, IsNullable = false, IsForeignKey = true, ForeignKeyTable = "Categories", ForeignKeyColumn = "stamp_entity" } },
                        { "descricao", new ColumnDefinition { DataType = "NVARCHAR(255)", IsPrimaryKey = false, IsNullable = false } },
                        { "valor_alvo", new ColumnDefinition { DataType = "DECIMAL(18,2)", IsPrimaryKey = false, IsNullable = false } },
                        { "data_limite", new ColumnDefinition { DataType = "DATE", IsPrimaryKey = false, IsNullable = false } },
                        { "created_at", new ColumnDefinition { DataType = "DATETIME", IsPrimaryKey = false, IsNullable = false, DefaultValue = "GETDATE()" } }
                    }
                },

                // Tabela de progresso das metas (Goal Progress)
                new TableDefinition
                {
                    TableName = "GoalProgress",
                    Columns = new Dictionary<string, ColumnDefinition>
                    {
                        { "stamp_entity", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = true, IsNullable = false, DefaultValue = "NEWID()" } },
                        { "goal_id", new ColumnDefinition { DataType = "NVARCHAR(50)", IsPrimaryKey = false, IsNullable = false, IsForeignKey = true, ForeignKeyTable = "Goals", ForeignKeyColumn = "stamp_entity" } },
                        { "valor_atual", new ColumnDefinition { DataType = "DECIMAL(18,2)", IsPrimaryKey = false, IsNullable = false } },
                        { "data_registro", new ColumnDefinition { DataType = "DATETIME", IsPrimaryKey = false, IsNullable = false, DefaultValue = "GETDATE()" } }
                    }
                }
            };
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

        private static async Task PopulateCategoriesAsync(SqlConnection connection, SqlTransaction transaction = null)
        {
            var categories = new List<(string, string)>
            {
                ("Alimentação", "budget"),
                ("Transporte", "budget"),
                ("Moradia", "budget"),
                ("Saúde", "budget"),
                ("Educação", "budget"),
                ("Lazer", "budget"),
                ("Compras", "budget"),
                ("Assinaturas", "budget"),
                ("Fundo de Emergência", "goal"),
                ("Viagem", "goal"),
                ("Compra de Carro", "goal"),
                ("Casa Própria", "goal"),
                ("Investimentos", "goal"),
                ("Casamento", "goal"),
                ("Educação dos Filhos", "goal"),
                ("Aposentadoria", "goal")
            };

            foreach (var (name, type) in categories)
            {
                var query = @"IF NOT EXISTS (SELECT 1 FROM Categories WHERE name = @name AND type = @type)
                      INSERT INTO Categories (stamp_entity, name, type) VALUES (NEWID(), @name, @type);";

                using (var command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@type", type);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }



    }
}
