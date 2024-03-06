using System;

namespace Questao5.Application.Commands
{
    public class SaldoDTO
    {
        public int NumeroContaCorrente { get; set; }
        public string NomeTitular { get; set; }
        public DateTime DataConsulta { get; set; }
        public double SaldoAtual { get; set; }
    }
}