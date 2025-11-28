using AutoMapper;
using BancoDeItens.Domain.Models;
using BancoDeItensWebApi.Dtos;


namespace BancoDeItensWebApi.Profiles
{
    // Define os mapeamentos para o AutoMapper
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Mapeamento de Entrada: DTO (o que vem do formulário) -> Entidade (o que vai para o banco)
            // O AutoMapper cuida de transferir Descricao e DisciplinaId
            CreateMap<QuestaoCadastroDto, Questao>();

            // Adicione aqui outros mapeamentos conforme o projeto evolui.
        }
    }
}