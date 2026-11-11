using Microsoft.Data.Sqlite;
using SoloProjectManagement.UI.Persistence;

namespace SoloProjectManagement.Tests.Helper
{
	public sealed class SqliteTestDb : IDisposable
	{
		private readonly SqliteConnection _keeper;
		public string ConnectionString { get; }
		public SqliteTestDb()
		{
			ConnectionString = new SqliteConnectionStringBuilder
			{
				DataSource = $"p,-test-{Guid.NewGuid()}",
				Mode = SqliteOpenMode.Memory,
				Cache = SqliteCacheMode.Shared
			}.ToString();

			_keeper = new SqliteConnection(ConnectionString);
			_keeper.Open();
		}

		public void Dispose() => _keeper.Dispose();

		public ProjectDbContext CreateContext() => new(ConnectionString);
	}
}
