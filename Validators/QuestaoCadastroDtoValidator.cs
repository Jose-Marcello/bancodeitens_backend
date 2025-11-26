//// Nome do arquivo: Validators/QuestaoCadastroDtoValidator.cs
using BancoDeItensWebApi.Dtos;
using BancoDeItensWebApi.Models;
using FluentValidation;

namespace BancoDeItensWebApi.Validators
{
    public class QuestaoCadastroDtoValidator : AbstractValidator<MigrationExtensions>
    {
        public QuestaoCadastroDtoValidator()
        {
            RuleFor(q => q.Descricao)
                .NotEmpty().WithMessage("A descrição da questão não pode ser vazia.")
                .Length(10, 2000).WithMessage("A descrição deve ter entre 10 e 2000 caracteres.");

            // 🛑 MUDANÇA CRÍTICA: A validação agora checa se o Guid não é vazio.
            RuleFor(q => q.DisciplinaId)
                .NotEmpty().WithMessage("É obrigatório informar uma Disciplina válida (Guid).");
        }
    }
}