using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StructProject.Database.Migrations
{
    /// <inheritdoc />
    public partial class PersistenceS7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "players_x",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    coins = table.Column<int>(type: "INTEGER", nullable: false),
                    best_wave = table.Column<int>(type: "INTEGER", nullable: false),
                    best_score = table.Column<int>(type: "INTEGER", nullable: false),
                    extra_max_hp = table.Column<int>(type: "INTEGER", nullable: false),
                    damage_level = table.Column<int>(type: "INTEGER", nullable: false),
                    speed_level = table.Column<int>(type: "INTEGER", nullable: false),
                    last_played_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players_x", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "save_slots",
                columns: table => new
                {
                    slot = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    json_state = table.Column<string>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_save_slots", x => x.slot);
                });

            migrationBuilder.CreateTable(
                name: "score_entries_x",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    score = table.Column<int>(type: "INTEGER", nullable: false),
                    wave = table.Column<int>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_score_entries_x", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "players_x");

            migrationBuilder.DropTable(
                name: "save_slots");

            migrationBuilder.DropTable(
                name: "score_entries_x");
        }
    }
}
