using Microsoft.EntityFrameworkCore.Migrations;

namespace ModelBattleBoardGames.Migrations
{
    public partial class AdicionandoOCampoImagem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Imagem",
                table: "ElementosDoExercitos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "ElementosDoExercitos");
        }
    }
}
