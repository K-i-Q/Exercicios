using Questao5.Domain.Entities;

namespace Questao5.Domain.Interfaces
{
    public interface IContaCorrenteRepositorio
    {
        Task<ContaCorrente> ObterContaCorrentePorId(string idContaCorrente);
    }

}
