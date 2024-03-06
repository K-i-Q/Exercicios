using MediatR;

namespace Questao5.Application.Commands
{
    public class MovimentacaoCommand : IRequest<string>
    {
        public string IdRequisicao { get; set; } 
        public string IdContaCorrente { get; set; }
        public double Valor { get; set; }
        public char TipoMovimento { get; set; } 
    }
}
