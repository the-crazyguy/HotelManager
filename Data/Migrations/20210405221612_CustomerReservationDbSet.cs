using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class CustomerReservationDbSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerReservation_Customers_CustomerId",
                table: "CustomerReservation");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerReservation_Reservations_ReservationId",
                table: "CustomerReservation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerReservation",
                table: "CustomerReservation");

            migrationBuilder.RenameTable(
                name: "CustomerReservation",
                newName: "CustomerReservations");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerReservation_ReservationId",
                table: "CustomerReservations",
                newName: "IX_CustomerReservations_ReservationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerReservations",
                table: "CustomerReservations",
                columns: new[] { "CustomerId", "ReservationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerReservations_Customers_CustomerId",
                table: "CustomerReservations",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerReservations_Reservations_ReservationId",
                table: "CustomerReservations",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerReservations_Customers_CustomerId",
                table: "CustomerReservations");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerReservations_Reservations_ReservationId",
                table: "CustomerReservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerReservations",
                table: "CustomerReservations");

            migrationBuilder.RenameTable(
                name: "CustomerReservations",
                newName: "CustomerReservation");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerReservations_ReservationId",
                table: "CustomerReservation",
                newName: "IX_CustomerReservation_ReservationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerReservation",
                table: "CustomerReservation",
                columns: new[] { "CustomerId", "ReservationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerReservation_Customers_CustomerId",
                table: "CustomerReservation",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerReservation_Reservations_ReservationId",
                table: "CustomerReservation",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
