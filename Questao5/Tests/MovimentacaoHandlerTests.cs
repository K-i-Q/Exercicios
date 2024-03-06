using NSubstitute;
using Questao5.Application.Commands;
using Questao5.Application.Handlers;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;
using Questao5.Infrastructure.Sqlite;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Questao5.Tests
{
    public class MovimentacaoHandlerTests
    {
        private readonly IContaCorrenteRepositorio _contaCorrenteRepositorio;
        private readonly IMovimentacaoRepositorio _movimentacaoRepositorio;
        private readonly MovimentacaoHandler _handler;

        public MovimentacaoHandlerTests()
        {
            _contaCorrenteRepositorio = Substitute.For<IContaCorrenteRepositorio>();
            _movimentacaoRepositorio = Substitute.For<IMovimentacaoRepositorio>();
            _handler = new MovimentacaoHandler(_contaCorrenteRepositorio, _movimentacaoRepositorio);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsMovimentoId()
        {
            var request = new MovimentacaoCommand
            {
                IdRequisicao = Guid.NewGuid().ToString(),
                IdContaCorrente = "valid-account-id",
                Valor = 100.00,
                TipoMovimento = 'C'
            };

            var contaCorrenteValida = new ContaCorrente { IdContaCorrente = "B6BAFC09-6967-ED11-A567-055DFA4A16C9" };
            _contaCorrenteRepositorio.ObterContaCorrentePorId(Arg.Is<string>(s => s == request.IdContaCorrente)).Returns(contaCorrenteValida);
            _movimentacaoRepositorio.InserirMovimento(Arg.Any<Movimento>()).Returns(Guid.NewGuid().ToString());
            _movimentacaoRepositorio.ObterResultadoIdempotente(Arg.Is<string>(s => s == request.IdRequisicao)).Returns((string)null);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact]
        public async Task Handle_WithSameRequest_ReturnsSameResultIdempotente()
        {
            var requestId = Guid.NewGuid().ToString();
            var movimentoId = Guid.NewGuid().ToString();

            var request = new MovimentacaoCommand
            {
                IdRequisicao = requestId,
                IdContaCorrente = "valid-account-id",
                Valor = 100.00,
                TipoMovimento = 'C'
            };

            _movimentacaoRepositorio.ObterResultadoIdempotente(Arg.Is<string>(s => s == requestId)).Returns(movimentoId);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.Equal(movimentoId, result);
        }

        [Fact]
        public async Task Handle_ContaCorrenteInexistente_ThrowsException()
        {
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(
                new MovimentacaoCommand
                {
                    IdRequisicao = Guid.NewGuid().ToString(),
                    IdContaCorrente = "non-existent-account",
                    Valor = 50.00,
                    TipoMovimento = 'D'
                },
                default));
        }

        [Fact]
        public async Task Handle_ContaInativa_ThrowsException()
        {
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(
                new MovimentacaoCommand
                {
                    IdRequisicao = Guid.NewGuid().ToString(),
                    IdContaCorrente = "inactive-account",
                    Valor = 200.00,
                    TipoMovimento = 'C'
                },
                default));
        }

        [Fact]
        public async Task Handle_ValorInvalido_ThrowsException()
        {
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(
                new MovimentacaoCommand
                {
                    IdRequisicao = Guid.NewGuid().ToString(),
                    IdContaCorrente = "valid-account-id",
                    Valor = 0,
                    TipoMovimento = 'C'
                },
                default));
        }

        [Fact]
        public async Task Handle_TipoMovimentoInvalido_ThrowsException()
        {
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(
                new MovimentacaoCommand
                {
                    IdRequisicao = Guid.NewGuid().ToString(),
                    IdContaCorrente = "valid-account-id",
                    Valor = 100,
                    TipoMovimento = 'X' 
                },
                default));
        }

    }
}