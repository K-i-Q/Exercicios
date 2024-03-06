using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ContaCorrenteController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContaCorrenteController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("saldo/{idContaCorrente}")]
    public async Task<ActionResult<SaldoDTO>> GetSaldo(string idContaCorrente)
    {
        var result = await _mediator.Send(new ConsultaSaldoCommand { IdContaCorrente = idContaCorrente });

        if (result == null) return NotFound("Conta Corrente não encontrada.");

        return Ok(result);
    }
}