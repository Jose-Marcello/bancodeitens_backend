using BancoDeItens.Application.Interfaces;
using BancoDeItens.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BancoDeItensWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class QuestoesController : ControllerBase
    {
        // 🛑 MUDANÇA CRÍTICA: Não injetamos mais o Repositório, mas sim a Interface do Serviço.
        private readonly IQuestaoService _questaoService;

        // Injeção de Dependência: O Controller recebe o Serviço.
        public QuestoesController(IQuestaoService questaoService)
        {
            _questaoService = questaoService;
        }

        // GET: api/Questoes
        // Retorna a lista de todas as questões
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Questao>>> GetQuestoes()
        {
            // O Controller apenas delega a chamada para a camada de Serviço.
            var questoes = await _questaoService.GetAllQuestoesAsync();
            return Ok(questoes);
        }

        // POST: api/Questoes
        // Adiciona uma nova questão
        [HttpPost]
        public async Task<ActionResult<Questao>> PostQuestao(Questao questao)
        {
            try
            {
                // O Controller delega a chamada para a camada de Serviço, que contém a lógica de negócio.
                await _questaoService.AddQuestaoAsync(questao);

                // Retorna 201 Created com a questão salva.
                return CreatedAtAction(nameof(GetQuestoes), new { id = questao.Id }, questao);
            }
            catch (ArgumentException ex)
            {
                // Se o Serviço lançar uma exceção de regra de negócio, retorna 400 Bad Request.
                return BadRequest(ex.Message);
            }
        }
    }
}