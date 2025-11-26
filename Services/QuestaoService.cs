using BancoDeItensWebApi.Interfaces;
using BancoDeItensWebApi.Models;

namespace BancoDeItensWebApi.Services
{
    // Implementação do Serviço de Questões.
    // Esta camada é responsável por coordenar operações, aplicar regras de negócio
    // e usar o Repositório para persistência.
    public class QuestaoService : IQuestaoService
    {
        private readonly IQuestaoRepository _questaoRepository;

        // Injeção de Dependência: O Serviço recebe o Repositório.
        public QuestaoService(IQuestaoRepository questaoRepository)
        {
            _questaoRepository = questaoRepository;
        }

        // Lógica para obter todas as questões.
        public async Task<IEnumerable<Questao>> GetAllQuestoesAsync()
        {
            // Poderia haver lógica de filtragem ou cache aqui, mas por enquanto, apenas repassa.
            return await _questaoRepository.GetAllAsync();
        }

        // Lógica para adicionar uma nova questão.
        public async Task AddQuestaoAsync(Questao questao)
        {
            // Exemplo de REGRA DE NEGÓCIO: Validação extra antes de persistir
            if (string.IsNullOrWhiteSpace(questao.Descricao) || questao.Descricao.Length < 5)
            {
                throw new ArgumentException("A descrição da questão é muito curta ou inválida.");
            }

            // Chama o Repositório para persistir os dados.
            await _questaoRepository.AddAsync(questao);
        }
    }
}