using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands;

namespace Questao5.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimentacaoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MovimentacaoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Movimentar([FromBody] MovimentacaoCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result); 
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
