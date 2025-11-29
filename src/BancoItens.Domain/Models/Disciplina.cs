// Nome do arquivo: Models/Disciplina.cs
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BancoItens.Domain.Models
{
    // Disciplina herda de Entity (agora usa Guid como PK)
    public class Disciplina : Entity
    {
        // O Id (Guid) é herdado de Entity.

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        // Propriedade de navegação
        public ICollection<Questao> Questoes { get; set; } = new List<Questao>();
    }
}