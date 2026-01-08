using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cofrinho.Infrastructure.Persistence.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "metas",
                columns: table => new
                {
                    nome = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metas", x => x.nome);
                });

            migrationBuilder.CreateTable(
                name: "transacoes",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    data = table.Column<DateTime>(type: "TEXT", nullable: false),
                    valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    descricao = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    MetaNome = table.Column<string>(type: "TEXT", nullable: true),
                    meta_nome = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transacoes", x => x.id);
                    table.ForeignKey(
                        name: "FK_transacoes_metas_meta_nome",
                        column: x => x.meta_nome,
                        principalTable: "metas",
                        principalColumn: "nome",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transacoes_metas_MetaNome",
                        column: x => x.MetaNome,
                        principalTable: "metas",
                        principalColumn: "nome");
                });

            migrationBuilder.CreateIndex(
                name: "IX_transacoes_meta_nome",
                table: "transacoes",
                column: "meta_nome");

            migrationBuilder.CreateIndex(
                name: "IX_transacoes_MetaNome",
                table: "transacoes",
                column: "MetaNome");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transacoes");

            migrationBuilder.DropTable(
                name: "metas");
        }
    }
}
