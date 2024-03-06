using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;

namespace Questao5.Infrastructure.Repositories
{
    public class MovimentacaoRepositorio : IMovimentacaoRepositorio
    {
        private readonly string _connectionString;

        public MovimentacaoRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<string> InserirMovimento(Movimento movimento)
        {

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            movimento.IdMovimento = Guid.NewGuid().ToString();

            var sql = @"
                INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor)
                VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)";

            await connection.ExecuteAsync(sql, movimento);

            return movimento.IdMovimento;

        }

        public async Task<string> ObterResultadoIdempotente(string idRequisicao)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT resultado FROM idempotencia WHERE chave_idempotencia = @IdRequisicao";
            return await connection.QuerySingleOrDefaultAsync<string>(query, new { IdRequisicao = idRequisicao });
        }
    }
}