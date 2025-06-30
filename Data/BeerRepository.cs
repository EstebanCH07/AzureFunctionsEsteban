using Dapper;
using AzureFunctionEsteban.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureFunctionEsteban.Data
{
	public class BeerRepository
	{
		private readonly SqlConnectionFactory _connectionFactory;

		public BeerRepository(SqlConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public async Task<IEnumerable<Beer>> GetAllAsync()
		{
			using var connection = _connectionFactory.CreateConnection();
			return await connection.QueryAsync<Beer>("SELECT * FROM Beer");
		}

		public async Task<Beer?> GetByIdAsync(int id)
		{
			using var connection = _connectionFactory.CreateConnection();
			return await connection.QueryFirstOrDefaultAsync<Beer>("SELECT * FROM Beer WHERE BeerId = @Id", new { Id = id });
		}

		public async Task<int> CreateAsync(Beer beer)
		{
			var sql = "INSERT INTO Beer (Name, Style, PruebaPro) VALUES (@Name, @Style, @PruebaPro)";
			using var connection = _connectionFactory.CreateConnection();
			return await connection.ExecuteAsync(sql, beer);
		}

		public async Task<int> UpdateAsync(Beer beer)
		{
			var sql = "UPDATE Beer SET Name = @Name, Style = @Style, PruebaPro = @PruebaPro WHERE BeerId = @BeerId";
			using var connection = _connectionFactory.CreateConnection();
			return await connection.ExecuteAsync(sql, beer);
		}

		public async Task<int> DeleteAsync(int id)
		{
			using var connection = _connectionFactory.CreateConnection();
			return await connection.ExecuteAsync("DELETE FROM Beer WHERE BeerId = @Id", new { Id = id });
		}
	}
}
