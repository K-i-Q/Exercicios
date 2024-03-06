using Questao5.Domain.Entities;

namespace Questao5.Domain.Interfaces
{

    public interface IMovimentacaoRepositorio
    {
        Task<string> InserirMovimento(Movimento movimento);
        Task<string> ObterResultadoIdempotente(string idRequisicao);
    }
}
