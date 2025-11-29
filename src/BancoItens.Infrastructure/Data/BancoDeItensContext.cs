// Nome do arquivo: Data/BancoDeItensContext.cs
using Microsoft.EntityFrameworkCore;
using BancoItens.Domain.Models;
using System;

namespace BancoItens.Infrastructure.Data
{
    public class BancoDeItensContext : DbContext
    {
        public BancoDeItensContext(DbContextOptions<BancoDeItensContext> options) : base(options)
        {
        }

        public DbSet<Questao> Questoes { get; set; } = null!;
        public DbSet<Disciplina> Disciplinas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🛑 MUDANÇA CRÍTICA: Configuração de chaves Guid
            modelBuilder.Entity<Questao>()
                .HasKey(q => q.Id);

            modelBuilder.Entity<Disciplina>()
                .HasKey(d => d.Id);

            // Configuração do relacionamento One-to-Many
            modelBuilder.Entity<Questao>()
                .HasOne(q => q.Disciplina)
                .WithMany(d => d.Questoes)
                .HasForeignKey(q => q.DisciplinaId)
                .IsRequired();

            // 🛑 MUDANÇA CRÍTICA: Criação de Guids para os dados iniciais
            modelBuilder.Entity<Disciplina>().HasData(
                new Disciplina { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Nome = "Matemática" },
                new Disciplina { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Nome = "História" },
                new Disciplina { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Nome = "Geografia" },
                new Disciplina { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Nome = "Português" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}