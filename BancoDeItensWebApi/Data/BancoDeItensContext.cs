// Localização: Data/CatalogoContext.cs
using BancoDeItensWebApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BancoDeItensWebApi.Data
{
    // O nome do projeto aqui deve ser ajustado para corresponder ao seu projeto (BancoDeItensWebApi)
    public class BancoDeItensContext : DbContext
    {
        public BancoDeItensContext(DbContextOptions<BancoDeItensContext> options) : base(options)
        {
        }

        // Define a tabela que o EF Core irá mapear
        public DbSet<Questao> Questoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuração mínima para a tabela Questao
            modelBuilder.Entity<Questao>().ToTable("Questoes");
            modelBuilder.Entity<Questao>().HasKey(q => q.Id);
            modelBuilder.Entity<Questao>().Property(q => q.Descricao).IsRequired();
        }
    }
}
