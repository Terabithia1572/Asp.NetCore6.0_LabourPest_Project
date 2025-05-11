using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mig_BlogComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentCommentID",
                table: "BlogsComments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogsComments_ParentCommentID",
                table: "BlogsComments",
                column: "ParentCommentID");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogsComments_BlogsComments_ParentCommentID",
                table: "BlogsComments",
                column: "ParentCommentID",
                principalTable: "BlogsComments",
                principalColumn: "BlogCommentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogsComments_BlogsComments_ParentCommentID",
                table: "BlogsComments");

            migrationBuilder.DropIndex(
                name: "IX_BlogsComments_ParentCommentID",
                table: "BlogsComments");

            migrationBuilder.DropColumn(
                name: "ParentCommentID",
                table: "BlogsComments");
        }
    }
}
