// BancoItens.Infrastructure/UnitOfWork.cs

using BancoDeItens.Domain.Interfaces;
using BancoDeItens.Domain.Models;
using BancoDeItens.Infrastructure.Data;
using BancoDeItens.Infrastructure.Data.Repositories;
using BancoDeItens.Infrastructure.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Transactions; // Usar Transactions, se necessário

namespace BancoDeItens.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BancoDeItensContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private readonly ILoggerFactory _loggerFactory;

        // Adiciona ILoggerFactory para log nos repositórios, seguindo o Bolão
        public UnitOfWork(BancoDeItensContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
            _loggerFactory = loggerFactory;
        }

        // Método complexo do Bolão: Cria/retorna o repositório correto
        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity, new()
        {
            if (_repositories.ContainsKey(typeof(TEntity)))
            {
                return (IRepository<TEntity>)_repositories[typeof(TEntity)];
            }

            object repositoryInstance = null;

            if (typeof(TEntity) == typeof(Disciplina))
            {
                // CORRIGIDO: Usa a Entidade (Disciplina) para tipar o logger
                var disciplinaLogger = _loggerFactory.CreateLogger<Disciplina>();
                repositoryInstance = new DisciplinaRepository(_context, disciplinaLogger);
            }
            else if (typeof(TEntity) == typeof(Questao))
            {
                // CORRIGIDO: Usa a Entidade (Questao) para tipar o logger
                var questaoLogger = _loggerFactory.CreateLogger<Questao>();
                repositoryInstance = new QuestaoRepository(_context, questaoLogger);
            }
            // ... Adicione outras entidades conforme necessário ...
            else
            {
                // CRIA A INSTÂNCIA DO REPOSITÓRIO GENÉRICO CONCRETO
                // Usa TEntity para tipar o logger e o construtor
                var genericLogger = _loggerFactory.CreateLogger<TEntity>();
                repositoryInstance = new Repository<TEntity>(_context, genericLogger);
            }

            _repositories[typeof(TEntity)] = repositoryInstance;
            return (IRepository<TEntity>)repositoryInstance;
        }


        // Commit com Transação e Estratégia de Execução (Retries)
        public async Task<bool> CommitAsync()
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _context.Database.BeginTransactionAsync()) // Transação explícita
                {
                    try
                    {
                        int result = await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return result > 0;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw; // Propaga a exceção para a camada Application
                    }
                }
            });
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}