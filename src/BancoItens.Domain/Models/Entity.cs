using System.ComponentModel.DataAnnotations;

namespace BancoItens.Domain.Models
{
    // Classe base que define a identidade única (Guid) para todas as entidades de domínio.
    // Isso é um padrão comum em Domain-Driven Design (DDD).
    public abstract class Entity
    {
        [Key]
        public Guid Id { get; set; }
        //public Guid Id { get; protected set; }

        protected Entity()
        {
            // O ID é gerado no momento da criação da entidade, e não pelo banco de dados.
            Id = Guid.NewGuid();
        }
    }
}