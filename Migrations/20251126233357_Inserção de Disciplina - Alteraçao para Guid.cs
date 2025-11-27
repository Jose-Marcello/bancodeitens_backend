using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BancoDeItensWebApi.Migrations
{
    /// <inheritdoc />
    public partial class InserçãodeDisciplinaAlteraçaoparaGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Altera a Descrição (ajuste de tamanho, não afeta o erro de PK)
            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Questoes",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            // 🛑 CORREÇÃO CRÍTICA (Passo 1): Retira a estratégia de geração de valor (IDENTITY)
            // Isso previne o erro 22023 do PostgreSQL antes de tentar trocar o tipo.
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Questoes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None) // Remove o Identity
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);


            // 🛑 CORREÇÃO CRÍTICA (Passo 2): Agora que o Id é um int "normal", ele pode ser alterado para Guid (uuid).
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Questoes",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            // 4. Adiciona a coluna DisciplinaId (FK)
            migrationBuilder.AddColumn<Guid>(
                name: "DisciplinaId",
                table: "Questoes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // 5. Cria a tabela Disciplinas
            migrationBuilder.CreateTable(
                name: "Disciplinas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disciplinas", x => x.Id);
                });

            // 6. Insere os dados iniciais
            migrationBuilder.InsertData(
                table: "Disciplinas",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Matemática" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "História" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "Geografia" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "Português" }
                });

            // 7. Cria o índice e a chave estrangeira
            migrationBuilder.CreateIndex(
                name: "IX_Questoes_DisciplinaId",
                table: "Questoes",
                column: "DisciplinaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questoes_Disciplinas_DisciplinaId",
                table: "Questoes",
                column: "DisciplinaId",
                principalTable: "Disciplinas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // O método Down reverte as alterações, voltando ao estado anterior
            migrationBuilder.DropForeignKey(
                name: "FK_Questoes_Disciplinas_DisciplinaId",
                table: "Questoes");

            migrationBuilder.DropTable(
                name: "Disciplinas");

            migrationBuilder.DropIndex(
                name: "IX_Questoes_DisciplinaId",
                table: "Questoes");

            migrationBuilder.DropColumn(
                name: "DisciplinaId",
                table: "Questoes");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Questoes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Questoes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}