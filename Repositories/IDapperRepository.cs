using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace MinimalApi.Repositories
{
    public interface IDapperRepository
    {
        Task<IEnumerable<T>> QueryAsync<T>(string sql, DynamicParameters dp, CommandType commandType = CommandType.Text);

        Task<int> ExecuteAsync<T>(string sql, T dp, CommandType commandType = CommandType.Text) where T : new();

        int Execute<T>(string sql, T dp, CommandType commandType = CommandType.Text) where T : new();
    }
}
