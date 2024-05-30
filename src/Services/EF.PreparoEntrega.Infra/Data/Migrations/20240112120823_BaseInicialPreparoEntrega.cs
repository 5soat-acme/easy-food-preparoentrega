using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EF.PreparoEntrega.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class BaseInicialPreparoEntrega : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "CodigoPedidoSequence",
                startValue: 1000L);

            migrationBuilder.CreateTable(
                name: "PedidosPreparoEntrega",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PedidoCorrelacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidosPreparoEntrega", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItensPreparoEntrega",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uuid", nullable: false),
                    NomeProduto = table.Column<string>(type: "text", nullable: false),
                    TempoPreparoEstimado = table.Column<int>(type: "integer", nullable: false),
                    PedidoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensPreparoEntrega", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensPreparoEntrega_PedidosPreparoEntrega_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "PedidosPreparoEntrega",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItensPreparoEntrega_PedidoId",
                table: "ItensPreparoEntrega",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IDX_PreparoEntrega_PedidoCorrelacaoId",
                table: "PedidosPreparoEntrega",
                column: "PedidoCorrelacaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensPreparoEntrega");

            migrationBuilder.DropTable(
                name: "PedidosPreparoEntrega");

            migrationBuilder.DropSequence(
                name: "CodigoPedidoSequence");
        }
    }
}
