using System.Threading.Tasks;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Questao5.Infrastructure.Repositories
{
    public class ContaCorrenteRepositorio : IContaCorrenteRepositorio
    {
        private readonly string _connectionString;

        public ContaCorrenteRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<ContaCorrente> ObterContaCorrentePorId(string idContaCorrente)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM contacorrente WHERE idcontacorrente = @Id";
            return await connection.QuerySingleOrDefaultAsync<ContaCorrente>(query, new { Id = idContaCorrente });
        }
    }
}