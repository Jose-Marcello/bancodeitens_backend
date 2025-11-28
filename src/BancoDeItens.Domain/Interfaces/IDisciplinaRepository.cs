using BancoDeItens.Domain.Models;
using System.Threading.Tasks;

namespace BancoDeItens.Domain.Interfaces
{
    // A interface específica deve herdar do IRepository<TEntity>
    public interface IDisciplinaRepository : IRepository<Disciplina>
    {
        // Aqui você adicionaria métodos específicos para Disciplina,
        // mas não métodos CRUD genéricos como GetAllAsync().
    }
}