
using BancoItens.Domain.Interfaces;
using BancoItens.Domain.Models;

namespace BancoDeItens.Domain.Interfaces
{
    // Interface que define o contrato de acesso a dados para Questões.
    public interface IQuestaoRepository : IRepository<Questao>
    {
        // Obtém todas as questões.
        //Task<IEnumerable<Models.Questao>> GetAllAsync();

        // Adiciona uma nova questão.
        //Task AddAsync(Models.Questao questao);
    }
}