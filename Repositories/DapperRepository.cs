using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;

namespace MinimalApi.Repositories
{
    public class DapperRepository(IOptions<RepositoryOptions> options) : IDapperRepository
    {
        private SqlConnection GetConnection()
        {
            var config = options.Value;

            if (config.Provider == "SqlServer")
                return new SqlConnection(config.ConnectionString);

            throw new NotSupportedException("Provider not supported.");
        }

        public async Task<int> ExecuteAsync<T>(string sql, T dp, CommandType commandType = CommandType.Text) where T : new()
        {
            int result;

            await using var connection = GetConnection();

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                await using var transaction = connection.BeginTransaction();

                try
                {
                    result = await connection.ExecuteAsync(sql, dp, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return result;
        }

        public int Execute<T>(string sql, T dp, CommandType commandType = CommandType.Text) where T : new()
        {
            int result;

            using var connection = GetConnection();

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                using var transaction = connection.BeginTransaction();

                try
                {
                    result = connection.Execute(sql, dp, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return result;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, DynamicParameters dp, CommandType commandType = CommandType.Text)
        {
            await using var db = GetConnection();

            if (db.State == ConnectionState.Closed)
                db.Open();

            try
            {
                return await db.QueryAsync<T>(sql, dp, commandType: commandType);
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }
        }
    }
}
