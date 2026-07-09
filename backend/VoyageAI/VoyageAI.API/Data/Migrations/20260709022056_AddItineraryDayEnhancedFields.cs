using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoyageAI.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddItineraryDayEnhancedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Summary",
                table: "ItineraryDays",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<decimal>(
                name: "ActualBudget",
                table: "ItineraryDays",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ItineraryDays",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "ItineraryDays",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ItineraryDays",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedBudget",
                table: "ItineraryDays",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ItineraryDays",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "ItineraryDays",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ItineraryDays",
                type: "character varying(3000)",
                maxLength: 3000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ItineraryDays",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "WeatherSummary",
                table: "ItineraryDays",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryDays_TripId_Date",
                table: "ItineraryDays",
                columns: new[] { "TripId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryDays_TripId_DayNumber",
                table: "ItineraryDays",
                columns: new[] { "TripId", "DayNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryDays_TripId_IsDeleted",
                table: "ItineraryDays",
                columns: new[] { "TripId", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ItineraryDays_TripId_Date",
                table: "ItineraryDays");

            migrationBuilder.DropIndex(
                name: "IX_ItineraryDays_TripId_DayNumber",
                table: "ItineraryDays");

            migrationBuilder.DropIndex(
                name: "IX_ItineraryDays_TripId_IsDeleted",
                table: "ItineraryDays");

            migrationBuilder.DropColumn(
                name: "ActualBudget",
                table: "ItineraryDays");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ItineraryDays");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ItineraryDays");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ItineraryDays");

            migrationBuilder.DropColumn(
                name: "EstimatedBudget",
                table: "ItineraryDays");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ItineraryDays");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "ItineraryDays");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ItineraryDays");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ItineraryDays");

            migrationBuilder.DropColumn(
                name: "WeatherSummary",
                table: "ItineraryDays");

            migrationBuilder.AlterColumn<string>(
                name: "Summary",
                table: "ItineraryDays",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);
        }
    }
}
