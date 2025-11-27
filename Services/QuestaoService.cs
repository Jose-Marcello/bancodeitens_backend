using BancoDeItensWebApi.Interfaces;
using BancoDeItensWebApi.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BancoDeItensWebApi.Services
{
    // Implementação do Serviço de Questões.
    public class QuestaoService : IQuestaoService
    {
        private readonly IQuestaoRepository _questaoRepository;
        // 🛑 NOVO: Injeção de IDisciplinaRepository para validação
        private readonly IDisciplinaRepository _disciplinaRepository;

        // 🛑 CORREÇÃO: Construtor deve receber IDisciplinaRepository.
        public QuestaoService(IQuestaoRepository questaoRepository, IDisciplinaRepository disciplinaRepository)
        {
            _questaoRepository = questaoRepository;
            _disciplinaRepository = disciplinaRepository;
        }

        public async Task<IEnumerable<Questao>> GetAllQuestoesAsync()
        {
            // O ideal aqui seria incluir o Eager Loading da Disciplina, mas o Repositório se encarrega.
            return await _questaoRepository.GetAllAsync();
        }

        public async Task AddQuestaoAsync(Questao questao)
        {
            // Exemplo de REGRA DE NEGÓCIO 1: Validação simples.
            if (string.IsNullOrWhiteSpace(questao.Descricao) || questao.Descricao.Length < 10)
            {
                // A validação de < 10 caracteres será capturada pelo FluentValidation no Controller,
                // mas mantemos a regra de negócio aqui como backup.
                throw new ArgumentException("A descrição da questão é muito curta ou inválida.");
            }

            // 🛑 REGRA DE NEGÓCIO 2: Validação se a DisciplinaId existe no banco.
            if (questao.DisciplinaId == Guid.Empty)
            {
                throw new ArgumentException("A questão deve estar associada a uma Disciplina válida.");
            }

            var areaExiste = (await _disciplinaRepository.GetAllAsync())
                                .Any(d => d.Id == questao.DisciplinaId);

            if (!areaExiste)
            {
                throw new ArgumentException($"A Disciplina com o ID {questao.DisciplinaId} não foi encontrada.");
            }

            // Chama o Repositório para persistir os dados.
            await _questaoRepository.AddAsync(questao);
        }
    }
}