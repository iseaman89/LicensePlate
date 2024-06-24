using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicensePlateServer.Migrations
{
    /// <inheritdoc />
    public partial class UpadateLicensePlate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CameraName",
                table: "LicensePlates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "30a24107-d279-4e37-96fd-01af5b38cb27",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7fc8adb0-6d57-4e94-a9c2-16a7a7938eb2", "AQAAAAIAAYagAAAAEI154EMIz1RH4cRv8RuCUk0HYD8uyrDUKyddxDYFXiaql+a5ahQr8gEEfUix7LCx3Q==", "cdabd538-d4a3-401b-b0a7-f9c0c9461a16" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e448afa-f008-446e-a52f-13c449803c2e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7dae029c-0d49-4125-84ab-b63f4ee1110f", "AQAAAAIAAYagAAAAELSq/C0h+mmeI/fNS3PgRc6LGFXK0upUmHuK+GelhjYJNWsm+r5OR2WfvDBGXCZnwQ==", "ec20b08d-5984-4867-a357-4a028944208a" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CameraName",
                table: "LicensePlates");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "30a24107-d279-4e37-96fd-01af5b38cb27",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "578da1d6-d9e0-4982-922c-be62b4674539", "AQAAAAIAAYagAAAAEPacupMsclQhBEcJEcIsxtCLDNNbEggbSbk2Yh1wzx9Q86LsTdVA1oXAApb+lPt2Ww==", "11900e63-5692-45d7-acae-f8c3f2fcb06d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e448afa-f008-446e-a52f-13c449803c2e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f2ce6dda-a7a4-4f19-be24-a8d7b890e390", "AQAAAAIAAYagAAAAEB1gu9BzPmvoeDnRs7agh/PqCtp62ma6to2S3VF3CgC+Yt/2IzgeR1a7+f8S94kOlQ==", "073b6874-2f2c-404f-a52f-66302d721be1" });
        }
    }
}
