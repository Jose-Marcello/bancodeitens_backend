namespace BancoDeItensWebApi.Interfaces
{
    // Interface que define o contrato de acesso a dados para Questões.
    public interface IQuestaoRepository
    {
        // Obtém todas as questões.
        Task<IEnumerable<Models.Questao>> GetAllAsync();

        // Adiciona uma nova questão.
        Task AddAsync(Models.Questao questao);
    }
}