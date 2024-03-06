using Dapper;
using MediatR;
using Microsoft.Data.Sqlite;
using Questao5.Application.Commands;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Sqlite;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Questao5.Application.Handlers
{
    public class ConsultaSaldoHandler : IRequestHandler<ConsultaSaldoCommand, SaldoDTO>
    {
        private readonly DatabaseConfig _databaseConfig;

        public ConsultaSaldoHandler(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public async Task<SaldoDTO> Handle(ConsultaSaldoCommand request, CancellationToken cancellationToken)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            await connection.OpenAsync();

            var contaCorrente = await connection.QuerySingleOrDefaultAsync<ContaCorrente>(
                "SELECT * FROM contacorrente WHERE idcontacorrente = @Id",
                new { Id = request.IdContaCorrente });

            if (contaCorrente == null)
            {
                throw new Exception("INVALID_ACCOUNT");
            }
            if (contaCorrente.Ativo != 1)
            {
                throw new Exception("INACTIVE_ACCOUNT");
            }

            var saldoCreditos = await connection.ExecuteScalarAsync<double>(
                "SELECT COALESCE(SUM(valor), 0) FROM movimento WHERE idcontacorrente = @Id AND tipomovimento = 'C'",
                new { Id = request.IdContaCorrente });

            var saldoDebitos = await connection.ExecuteScalarAsync<double>(
                "SELECT COALESCE(SUM(valor), 0) FROM movimento WHERE idcontacorrente = @Id AND tipomovimento = 'D'",
                new { Id = request.IdContaCorrente });

            var saldoAtual = saldoCreditos - saldoDebitos;

            return new SaldoDTO
            {
                NumeroContaCorrente = contaCorrente.Numero,
                NomeTitular = contaCorrente.Nome,
                DataConsulta = DateTime.UtcNow, 
                SaldoAtual = saldoAtual
            };
        }
    }
}