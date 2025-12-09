using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class voretest2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_bellies_profile_id_name_inner_description_digest_mode",
                table: "bellies");

            migrationBuilder.AddColumn<string>(
                name: "digest_desc_pred",
                table: "bellies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "digest_desc_prey",
                table: "bellies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "expell_desc",
                table: "bellies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ingest_desc",
                table: "bellies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_bellies_profile_id_name",
                table: "bellies",
                columns: new[] { "profile_id", "name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_bellies_profile_id_name",
                table: "bellies");

            migrationBuilder.DropColumn(
                name: "digest_desc_pred",
                table: "bellies");

            migrationBuilder.DropColumn(
                name: "digest_desc_prey",
                table: "bellies");

            migrationBuilder.DropColumn(
                name: "expell_desc",
                table: "bellies");

            migrationBuilder.DropColumn(
                name: "ingest_desc",
                table: "bellies");

            migrationBuilder.CreateIndex(
                name: "IX_bellies_profile_id_name_inner_description_digest_mode",
                table: "bellies",
                columns: new[] { "profile_id", "name", "inner_description", "digest_mode" },
                unique: true);
        }
    }
}
