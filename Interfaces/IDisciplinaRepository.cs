using BancoDeItensWebApi.Models;

namespace BancoDeItensWebApi.Interfaces
{
    // Contrato para acesso a dados da entidade Disciplina.
    public interface IDisciplinaRepository
    {
        Task<IEnumerable<QuestaoCadastroDto>> GetAllAsync();
    }
}