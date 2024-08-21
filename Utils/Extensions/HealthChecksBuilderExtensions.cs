using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MinimalApi.Utils.Extensions
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddSqlServerQueryHealthCheck(this IHealthChecksBuilder builder,
            string connectionString, HealthStatus failureStatus = HealthStatus.Degraded, IEnumerable<string> tags = default, TimeSpan? timeout = default)
        {
            return builder.AddCheck("Sql Server (read): ", new SqlServerHealthCheck(connectionString), failureStatus, tags, timeout);
        }
    }

    public class SqlServerHealthCheck(string connectionString) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync(cancellationToken);
                await connection.CloseAsync();
                return await Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception e)
            {
                return await Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, description: e.Message, exception: e));
            }
        }
    }
}
