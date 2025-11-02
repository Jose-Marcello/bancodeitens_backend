// Localização: Models/Questao.cs
// Modelo Mínimo Viável (MMV) para o teste de conectividade
namespace BancoDeItensWebApi.Models
{
    public class Questao
    {
        public int Id { get; set; }

        // A Pergunta / Enunciado
        public required string Descricao { get; set; }
    }

}
