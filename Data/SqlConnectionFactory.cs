using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AzureFunctionEsteban.Data
{
	public class SqlConnectionFactory
	{
		private readonly IConfiguration _configuration;

		public SqlConnectionFactory(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public IDbConnection CreateConnection()
		{
			var connectionString = _configuration.GetConnectionString("DefaultConnection");
			return new SqlConnection(connectionString);
		}
	}
}
