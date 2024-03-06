using Dapper;
using MediatR;
using Microsoft.Data.Sqlite;
using Questao5.Application.Commands;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;
using Questao5.Infrastructure.Sqlite;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Questao5.Application.Handlers
{
    public class MovimentacaoHandler : IRequestHandler<MovimentacaoCommand, string>
    {
        private readonly IContaCorrenteRepositorio _contaCorrenteRepositorio;
        private readonly IMovimentacaoRepositorio _movimentacaoRepositorio;

        public MovimentacaoHandler(IContaCorrenteRepositorio contaCorrenteRepositorio, IMovimentacaoRepositorio movimentacaoRepositorio)
        {
            _contaCorrenteRepositorio = contaCorrenteRepositorio;
            _movimentacaoRepositorio = movimentacaoRepositorio;
        }

        public async Task<string> Handle(MovimentacaoCommand request, CancellationToken cancellationToken)
        {
            var resultadoIdempotencia = await _movimentacaoRepositorio.ObterResultadoIdempotente(request.IdRequisicao);
            if (!string.IsNullOrEmpty(resultadoIdempotencia))
            {
                return resultadoIdempotencia;
            }

            var contaCorrente = await _contaCorrenteRepositorio.ObterContaCorrentePorId(request.IdContaCorrente);

            if (contaCorrente == null || contaCorrente.Ativo != 1)
            {
                throw new InvalidOperationException("INVALID_ACCOUNT or INACTIVE_ACCOUNT");
            }

            if (request.Valor <= 0)
            {
                throw new InvalidOperationException("INVALID_VALUE");
            }

            if (request.TipoMovimento != 'C' && request.TipoMovimento != 'D')
            {
                throw new InvalidOperationException("INVALID_TYPE");
            }

            var idMovimento = await _movimentacaoRepositorio.InserirMovimento(new Movimento
            {
                IdContaCorrente = request.IdContaCorrente,
                DataMovimento = DateTime.Now,
                TipoMovimento = request.TipoMovimento,
                Valor = request.Valor
            });

            return idMovimento;
        }
    }
}