using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoyageAI.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTravelerEnhancedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsPrimaryTraveler",
                table: "Travelers",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Travelers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Travelers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Travelers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Travelers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DietaryPreference",
                table: "Travelers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactName",
                table: "Travelers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactPhone",
                table: "Travelers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FrequentFlyerNumber",
                table: "Travelers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Travelers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Travelers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "KnownTravelerNumber",
                table: "Travelers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "Travelers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "Travelers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PassportCountry",
                table: "Travelers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PassportExpiry",
                table: "Travelers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Relationship",
                table: "Travelers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SpecialRequirements",
                table: "Travelers",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Travelers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.CreateIndex(
                name: "IX_Travelers_Email",
                table: "Travelers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Travelers_PassportNumber",
                table: "Travelers",
                column: "PassportNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Travelers_TripId_IsDeleted",
                table: "Travelers",
                columns: new[] { "TripId", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Travelers_Email",
                table: "Travelers");

            migrationBuilder.DropIndex(
                name: "IX_Travelers_PassportNumber",
                table: "Travelers");

            migrationBuilder.DropIndex(
                name: "IX_Travelers_TripId_IsDeleted",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "DietaryPreference",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "EmergencyContactName",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "EmergencyContactPhone",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "FrequentFlyerNumber",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "KnownTravelerNumber",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "PassportCountry",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "PassportExpiry",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "Relationship",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "SpecialRequirements",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Travelers");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPrimaryTraveler",
                table: "Travelers",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);
        }
    }
}
