namespace MinimalApi.Repositories
{
    public sealed class RepositoryOptions
    {
        public string ConnectionString { get; private set; }

        public string Provider { get; private set; }

        public void UserSqlServer(string connectionString)
        {
            ConnectionString = connectionString;
            Provider = "SqlServer";
        }
    }
}
