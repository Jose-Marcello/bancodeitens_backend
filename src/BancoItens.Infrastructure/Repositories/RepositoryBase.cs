using BancoItens.Domain.Interfaces;
using BancoItens.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace BancoItens.Infrastructure.Data.Repositories
{
    // A restrição (where) é necessária tanto na interface quanto na implementação.
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : Entity, new()
    {
        protected readonly BancoDeItensContext Db;
        protected readonly DbSet<TEntity> DbSet;
        protected readonly ILogger<TEntity> Logger;

        // Construtor com DbContext e ILogger
        protected RepositoryBase(BancoDeItensContext db, ILogger<TEntity> logger)
        {
            Db = db;
            DbSet = db.Set<TEntity>();
            Logger = logger;
            // Logger.LogInformation("Instância de RepositoryBase criada para {EntityType}", typeof(TEntity).Name);
        }

        // ----------------------------------------------------------------------
        // COMANDOS (Escrita)
        // ----------------------------------------------------------------------

        public virtual async Task Adicionar(TEntity entity)
        {
            await DbSet.AddAsync(entity);
            // SaveChanges é responsabilidade do UnitOfWork
        }

        public virtual Task Atualizar(TEntity entity)
        {
            DbSet.Update(entity);
            return Task.CompletedTask;
        }

        public virtual Task Remover(TEntity entity)
        {
            DbSet.Remove(entity);
            return Task.CompletedTask;
        }

        // ----------------------------------------------------------------------
        // CONSULTAS (Leitura)
        // ----------------------------------------------------------------------

        // Este é o método que estava faltando!
        public virtual async Task<IEnumerable<TEntity>> ObterTodos()
        {
            return await DbSet.AsNoTracking().ToListAsync();
        }

        public virtual async Task<TEntity> ObterPorId(Guid id)
        {
            return await DbSet.FindAsync(id);
        }

        public IQueryable<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate)
        {
            // Retorna IQueryable para que a expressão seja aplicada antes da execução (AsNoTracking para leitura)
            return DbSet.AsNoTracking().Where(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.CountAsync(predicate);
        }

        // ----------------------------------------------------------------------
        // DISPOSE
        // ----------------------------------------------------------------------

        public void Dispose()
        {
            // A disposição do contexto geralmente é controlada pelo UnitOfWork e pelo container DI (Scoped).
            // Deixamos a implementação vazia ou delegamos, dependendo da sua gestão no UoW.
        }
    }
}