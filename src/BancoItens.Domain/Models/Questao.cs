// Nome do arquivo: Models/Questao.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancoItens.Domain.Models
{
    // Questao herda de Entity (agora usa Guid como PK)
    public class Questao : Entity
    {
        // O Id (Guid) é herdado de Entity.

        [Required]
        [StringLength(2000)]
        public string Descricao { get; set; } = string.Empty;

        // 🛑 MUDANÇA CRÍTICA: A chave estrangeira agora é do tipo Guid.
        [ForeignKey("Disciplina")]
        public Guid DisciplinaId { get; set; }

        // Propriedade de navegação para o objeto Disciplina
        public Disciplina Disciplina { get; set; } = null!;
    }
}