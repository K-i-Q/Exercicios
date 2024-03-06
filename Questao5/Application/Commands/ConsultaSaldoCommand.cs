using MediatR;

namespace Questao5.Application.Commands
{
    public class ConsultaSaldoCommand : IRequest<SaldoDTO>
    {
        public string IdContaCorrente { get; set; }
    }
}