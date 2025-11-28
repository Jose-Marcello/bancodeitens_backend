using BancoDeItens.Domain.Models;
using System.Threading.Tasks;

namespace BancoDeItens.Application.Interfaces
{
    public interface IDisciplinaService
    {
        Task AddDisciplinaAsync(Disciplina disciplina);
        Task<IEnumerable<Disciplina>> GetAllDisciplinasAsync();
        // Adicione outros métodos CRUD/Busca que o Controller precisará
    }
}