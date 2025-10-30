using BancoDeItensWebApi.Data;
using BancoDeItensWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BancoDeItensWebApi.Controllers
{
    // A rota /api/[controller] usará o nome da classe, resultando em /api/Questoes
    [ApiController]
    [Route("api/[controller]")]
    public class QuestoesController : ControllerBase
    {
        private readonly BancoDeItensContext _context; // Usando o seu DbContext: BancoDeItensContext

        public QuestoesController(BancoDeItensContext context)
        {
            _context = context;
        }

        // GET: api/Questoes
        [HttpGet]
        // O tipo de retorno 'IActionResult' é usado para dar flexibilidade em retornar diferentes status HTTP (Ok, NotFound, etc.)
        public async Task<IActionResult> GetQuestoes()
        {
            var questoes = await _context.Questoes.ToListAsync();

            // Retornar 'NotFound' se a lista for nula é uma opção de tratamento de erro.
            if (questoes == null)
            {
                // Este cenário é raro com ToListAsync(), mas é uma boa prática de segurança.
                return NotFound("Nenhuma questão encontrada no banco de dados.");
            }

            // CORREÇÃO APLICADA: 
            // Retorna 200 OK e o objeto 'questoes' (que será serializado como JSON, 
            // sendo uma lista vazia '[]' se não houver dados).
            return Ok(questoes);
        }

        // POST: api/Questoes
        [HttpPost]
        public async Task<ActionResult<Questao>> PostQuestao(Questao questao)
        {
            // Adiciona uma nova questão ao banco de dados
            _context.Questoes.Add(questao);
            await _context.SaveChangesAsync();

            // Retorna a questão criada e o código 201 Created, seguindo a convenção REST.
            return CreatedAtAction(nameof(GetQuestoes), new { id = questao.Id }, questao);
        }

        // Outros métodos (PUT, DELETE, etc.) viriam aqui
    }
}
