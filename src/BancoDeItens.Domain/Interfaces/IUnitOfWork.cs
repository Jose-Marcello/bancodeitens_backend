using BancoDeItens.Domain.Models; // Necessário para a restrição 'TEntity : Entity'
using System;
using System.Threading.Tasks;

namespace BancoDeItens.Domain.Interfaces
{
    /// <summary>
    /// Define o contrato para a Unidade de Trabalho (Unit of Work), 
    /// responsável por encapsular operações em uma transação atômica.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Salva todas as alterações pendentes no contexto de forma assíncrona.
        /// </summary>
        /// <returns>True se as alterações foram salvas com sucesso, false caso contrário.</returns>
        Task<bool> CommitAsync();

        /// <summary>
        /// Obtém a instância do repositório específico para a entidade TEntity.
        /// </summary>
        /// <typeparam name="TEntity">A entidade para a qual o repositório é necessário.</typeparam>
        /// <returns>A instância do repositório da entidade.</returns>
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity, new();

        // O método Dispose() já está incluso na herança de IDisposable.
    }
}