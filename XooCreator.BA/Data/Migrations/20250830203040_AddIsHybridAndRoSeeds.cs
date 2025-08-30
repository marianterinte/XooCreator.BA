using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace XooCreator.BA.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsHybridAndRoSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHybrid",
                table: "Animals",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Regions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), "Junglă" },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "Savana" },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "Stepă" },
                    { new Guid("20000000-0000-0000-0000-000000000004"), "Fermă" }
                });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "Label", "RegionId", "Src", "IsHybrid" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-00000000000f"), "Jaguar", new Guid("20000000-0000-0000-0000-000000000001"), "images/animals/base/jaguar.jpg", false },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "Tucan", new Guid("20000000-0000-0000-0000-000000000001"), "images/animals/base/tucan.jpg", false },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "Anaconda", new Guid("20000000-0000-0000-0000-000000000001"), "images/animals/base/anaconda.jpg", false },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "Maimuța Capucin", new Guid("20000000-0000-0000-0000-000000000001"), "images/animals/base/maimuta_capucin.jpg", false },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "Broasca Otrăvitoare", new Guid("20000000-0000-0000-0000-000000000001"), "images/animals/base/broasca_otravitoare.jpg", false },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "Leu", new Guid("20000000-0000-0000-0000-000000000002"), "images/animals/base/leu.jpg", false },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "Elefant African", new Guid("20000000-0000-0000-0000-000000000002"), "images/animals/base/elefant_african.jpg", false },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "Girafă", new Guid("20000000-0000-0000-0000-000000000002"), "images/animals/base/girafa.jpg", false },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "Zebra", new Guid("20000000-0000-0000-0000-000000000002"), "images/animals/base/zebra.jpg", false },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "Rinocer", new Guid("20000000-0000-0000-0000-000000000002"), "images/animals/base/rinocer.jpg", false },
                    { new Guid("00000000-0000-0000-0000-000000000019"), "Bizon (America de Nord)", new Guid("20000000-0000-0000-0000-000000000003"), "images/animals/base/bizon.jpg", false },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "Antilopa Saiga (Eurasia)", new Guid("20000000-0000-0000-0000-000000000003"), "images/animals/base/saiga.jpg", false },
                    { new Guid("00000000-0000-0000-0000-00000000001b"), "Lup cenușiu", new Guid("20000000-0000-0000-0000-000000000003"), "images/animals/base/lup_cenusiu.jpg", false },
                    { new Guid("00000000-0000-0000-0000-00000000001c"), "Cal sălbatic (Przewalski)", new Guid("20000000-0000-0000-0000-000000000003"), "images/animals/base/cal_przewalski.jpg", false },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "Vultur de stepă", new Guid("20000000-0000-0000-0000-000000000003"), "images/animals/base/vultur_de_stepa.jpg", false },
                    { new Guid("00000000-0000-0000-0000-00000000001e"), "Vaca", new Guid("20000000-0000-0000-0000-000000000004"), "images/animals/base/vaca.jpg", false },
                    { new Guid("00000000-0000-0000-0000-00000000001f"), "Oaia", new Guid("20000000-0000-0000-0000-000000000004"), "images/animals/base/oaia.jpg", false },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "Calul", new Guid("20000000-0000-0000-0000-000000000004"), "images/animals/base/calul.jpg", false },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "Găina", new Guid("20000000-0000-0000-0000-000000000004"), "images/animals/base/gaina.jpg", false },
                    { new Guid("00000000-0000-0000-0000-000000000022"), "Porcul", new Guid("20000000-0000-0000-0000-000000000004"), "images/animals/base/porc.jpg", false }
                });

            migrationBuilder.InsertData(
                table: "AnimalPartSupports",
                columns: new[] { "AnimalId", "PartKey" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-00000000000f"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000000f"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000000f"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "wings" },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "horn" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000019"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000019"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000019"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "horns" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "legs" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "tail" },
                    { new Guid("00000000-0000-0000-0000-00000000001b"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000001b"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000001b"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000001c"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000001c"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000001c"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000001c"), "legs" },
                    { new Guid("00000000-0000-0000-0000-00000000001c"), "tail" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "legs" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "tail" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "wings" },
                    { new Guid("00000000-0000-0000-0000-00000000001e"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000001e"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000001e"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000001f"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000001f"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000001f"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "wings" },
                    { new Guid("00000000-0000-0000-0000-000000000022"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000022"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000022"), "head" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000f"), "arms" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000f"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000f"), "head" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000010"), "arms" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000010"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000010"), "head" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000010"), "legs" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000010"), "tail" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000010"), "wings" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000011"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000011"), "head" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000011"), "tail" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000012"), "arms" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000012"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000012"), "head" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000013"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000013"), "head" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000013"), "legs" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000013"), "tail" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000014"), "arms" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000014"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000014"), "head" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000015"), "arms" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000015"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000015"), "head" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000016"), "arms" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000016"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000016"), "head" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000016"), "legs" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000016"), "tail" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000017"), "arms" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000017"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000017"), "head" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000017"), "legs" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000017"), "tail" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000018"), "arms" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000018"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000018"), "head" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000018"), "horn" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000018"), "legs" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000018"), "tail" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000019"), "arms" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000019"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000019"), "head" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001a"), "arms" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001a"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001a"), "head" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001a"), "horns" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001a"), "legs" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001a"), "tail" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001b"), "arms" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001b"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001b"), "head" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001c"), "arms" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001c"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001c"), "head" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001c"), "legs" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001c"), "tail" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001d"), "arms" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001d"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001d"), "head" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001d"), "legs" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001d"), "tail" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001d"), "wings" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001e"), "arms" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001e"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001e"), "head" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001f"), "arms" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001f"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001f"), "head" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000020"), "arms" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000020"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000020"), "head" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000020"), "legs" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000020"), "tail" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000021"), "arms" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000021"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000021"), "head" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000021"), "legs" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000021"), "tail" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000021"), "wings" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000022"), "arms" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000022"), "body" });

            migrationBuilder.DeleteData(
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000022"), "head" });

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000000f"));
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"));
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000011"));
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000012"));
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000013"));
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000014"));
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000015"));
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000016"));
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000017"));
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000018"));
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000019"));
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000001a"));
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000001b"));
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000001c"));
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000001d"));
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000001e"));
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000001f"));
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000020"));
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000021"));
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000022"));

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"));
            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"));
            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"));
            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000004"));

            migrationBuilder.DropColumn(
                name: "IsHybrid",
                table: "Animals");
        }
    }
}
