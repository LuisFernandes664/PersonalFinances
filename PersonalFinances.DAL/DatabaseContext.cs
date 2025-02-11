using PersonalFinances.DAL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.DAL
{
    public class DatabaseContext : IDisposable
    {
        private readonly SqlConnection _connection;
        private SqlTransaction _transaction;

        public SqlConnection Connection => _connection;
        public SqlTransaction Transaction => _transaction;

        public DatabaseContext()
        {
            string connecionString = ConfigManager.GetConnectionString();
            _connection = new SqlConnection(connecionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }

}
