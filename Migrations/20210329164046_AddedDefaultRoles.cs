using Microsoft.EntityFrameworkCore.Migrations;

namespace HotelListingAPI.Migrations
{
    public partial class AddedDefaultRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "d7f53b97-0af8-4279-8680-6b60b850c012", "72e69ad7-9f83-4af0-a74d-417b52ac4577", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1895622f-333c-4bf7-ab1d-1de852558c3d", "ad3e9512-25de-413f-839c-3e2264466435", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1895622f-333c-4bf7-ab1d-1de852558c3d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d7f53b97-0af8-4279-8680-6b60b850c012");
        }
    }
}
