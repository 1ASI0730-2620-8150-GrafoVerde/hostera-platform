using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HosteraBackend.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "booking_confirmations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    property_id = table.Column<int>(type: "int", nullable: false),
                    booking_id = table.Column<int>(type: "int", nullable: false),
                    booking_code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    recipient_email = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    language = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    provider_message_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    reason = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    processed_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_booking_confirmations", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "bookings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    property_id = table.Column<int>(type: "int", nullable: false),
                    version = table.Column<int>(type: "int", nullable: false),
                    code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    guest_name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    guest_email = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    guest_phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    preferred_language = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false),
                    check_in_date = table.Column<DateTime>(type: "date", nullable: false),
                    check_out_date = table.Column<DateTime>(type: "date", nullable: false),
                    guests = table.Column<int>(type: "int", nullable: false),
                    room_type_id = table.Column<int>(type: "int", nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    rate_plan_id = table.Column<int>(type: "int", nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    guest_request = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    confirmed_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    confirmed_by = table.Column<int>(type: "int", nullable: true),
                    cancelled_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    cancelled_by = table.Column<int>(type: "int", nullable: true),
                    cancellation_reason = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    cancellation_note = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    no_show_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    no_show_by = table.Column<int>(type: "int", nullable: true),
                    checked_in_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    checked_in_by = table.Column<int>(type: "int", nullable: true),
                    guest_document_type = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    guest_document_number = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    checked_out_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    checked_out_by = table.Column<int>(type: "int", nullable: true),
                    room_condition = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    departure_note = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_bookings", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "inventory_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    property_id = table.Column<int>(type: "int", nullable: false),
                    code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    category = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    unit = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    primary_location_id = table.Column<int>(type: "int", nullable: false),
                    low_stock_threshold = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_inventory_items", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "properties",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    organization_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    city = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    currency = table.Column<string>(type: "char(3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_properties", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rate_plans",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    property_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    refundable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cancellation_policy = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_rate_plans", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "room_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    property_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    capacity = table.Column<int>(type: "int", nullable: false),
                    bed_configuration = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    base_nightly_rate = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_room_types", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "staff_members",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    property_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    role = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_staff_members", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "storage_locations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    property_id = table.Column<int>(type: "int", nullable: false),
                    code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    area = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    responsible_team = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    description = table.Column<string>(type: "varchar(240)", maxLength: 240, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_storage_locations", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    organization_id = table.Column<int>(type: "int", nullable: false),
                    username = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_users", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    property_id = table.Column<int>(type: "int", nullable: false),
                    booking_id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    method = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    paid_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    reference = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true),
                    recorded_by = table.Column<int>(type: "int", nullable: false),
                    idempotency_key = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_payments", x => x.id);
                    table.ForeignKey(
                        name: "f_k_payments_bookings_booking_id",
                        column: x => x.booking_id,
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "inventory_stocks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    inventory_item_id = table.Column<int>(type: "int", nullable: false),
                    storage_location_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_inventory_stocks", x => x.id);
                    table.ForeignKey(
                        name: "f_k_inventory_stocks_inventory_items_inventory_item_id",
                        column: x => x.inventory_item_id,
                        principalTable: "inventory_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "stock_adjustments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    inventory_item_id = table.Column<int>(type: "int", nullable: false),
                    operation = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    quantity_change = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    storage_location_id = table.Column<int>(type: "int", nullable: false),
                    reason = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    note = table.Column<string>(type: "varchar(240)", maxLength: 240, nullable: true),
                    @operator = table.Column<string>(name: "operator", type: "varchar(80)", maxLength: 80, nullable: false),
                    recorded_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    transfer_id = table.Column<Guid>(type: "char(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_stock_adjustments", x => x.id);
                    table.ForeignKey(
                        name: "f_k_stock_adjustments_inventory_items_inventory_item_id",
                        column: x => x.inventory_item_id,
                        principalTable: "inventory_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "daily_rates",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    property_id = table.Column<int>(type: "int", nullable: false),
                    rate_plan_id = table.Column<int>(type: "int", nullable: false),
                    room_type_id = table.Column<int>(type: "int", nullable: false),
                    rate_date = table.Column<DateTime>(type: "date", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_daily_rates", x => x.id);
                    table.ForeignKey(
                        name: "f_k_daily_rates__rate_plan_rate_plan_id",
                        column: x => x.rate_plan_id,
                        principalTable: "rate_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rate_plan_room_types",
                columns: table => new
                {
                    rate_plan_id = table.Column<int>(type: "int", nullable: false),
                    room_type_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_rate_plan_room_types", x => new { x.rate_plan_id, x.room_type_id });
                    table.ForeignKey(
                        name: "f_k_rate_plan_room_types_rate_plans_rate_plan_id",
                        column: x => x.rate_plan_id,
                        principalTable: "rate_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rate_plan_services",
                columns: table => new
                {
                    rate_plan_id = table.Column<int>(type: "int", nullable: false),
                    service = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_rate_plan_services", x => new { x.rate_plan_id, x.service });
                    table.ForeignKey(
                        name: "f_k_rate_plan_services_rate_plans_rate_plan_id",
                        column: x => x.rate_plan_id,
                        principalTable: "rate_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    property_id = table.Column<int>(type: "int", nullable: false),
                    number = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    room_type_id = table.Column<int>(type: "int", nullable: false),
                    floor = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_rooms", x => x.id);
                    table.ForeignKey(
                        name: "f_k_rooms__room_type_room_type_id",
                        column: x => x.room_type_id,
                        principalTable: "room_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "credentials",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    property_id = table.Column<int>(type: "int", nullable: false),
                    card_id = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    holder_name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    booking_id = table.Column<int>(type: "int", nullable: true),
                    booking_code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    room_id = table.Column<int>(type: "int", nullable: true),
                    staff_member_id = table.Column<int>(type: "int", nullable: true),
                    scope = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    valid_from = table.Column<DateTimeOffset>(type: "datetime(6)", precision: 6, nullable: false),
                    valid_until = table.Column<DateTimeOffset>(type: "datetime(6)", precision: 6, nullable: true),
                    issued_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    issued_by = table.Column<int>(type: "int", nullable: false),
                    revocation_reason = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    revocation_note = table.Column<string>(type: "varchar(240)", maxLength: 240, nullable: true),
                    revoked_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    revoked_by = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_credentials", x => x.id);
                    table.ForeignKey(
                        name: "f_k_credentials__staff_member_staff_member_id",
                        column: x => x.staff_member_id,
                        principalTable: "staff_members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "status_periods",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    property_id = table.Column<int>(type: "int", nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: false),
                    reason = table.Column<string>(type: "varchar(240)", maxLength: 240, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_status_periods", x => x.id);
                    table.ForeignKey(
                        name: "f_k_status_periods_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "access_events",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    property_id = table.Column<int>(type: "int", nullable: false),
                    credential_id = table.Column<int>(type: "int", nullable: true),
                    card_id = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    holder_type = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    holder_name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: true),
                    access_point = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    result = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    denial_reason = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    occurred_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_access_events", x => x.id);
                    table.ForeignKey(
                        name: "f_k_access_events__credential_credential_id",
                        column: x => x.credential_id,
                        principalTable: "credentials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "properties",
                columns: new[] { "id", "city", "currency", "name", "organization_id" },
                values: new object[,]
                {
                    { 1, "Cartagena", "PEN", "Cartagena 04", 1 },
                    { 2, "Lima", "PEN", "Lima 01", 1 },
                    { 3, "Arequipa", "PEN", "Arequipa 01", 2 }
                });

            migrationBuilder.InsertData(
                table: "staff_members",
                columns: new[] { "id", "name", "property_id", "role" },
                values: new object[,]
                {
                    { 1, "Lucía Martín", 1, "Operations manager" },
                    { 2, "Andrés León", 1, "Maintenance technician" },
                    { 3, "Rosa Pérez", 1, "Housekeeping supervisor" },
                    { 4, "Jorge Salas", 1, "Front desk agent" },
                    { 5, "Diana Flores", 1, "Night auditor" },
                    { 6, "Pedro Rivas", 1, "Housekeeping attendant" },
                    { 7, "Carla Núñez", 2, "Front desk agent" },
                    { 8, "Miguel Ortiz", 2, "Housekeeping attendant" },
                    { 9, "Silvia Cárdenas", 3, "Operations manager" },
                    { 10, "Raúl Mamani", 3, "Front desk agent" },
                    { 11, "Teresa Apaza", 3, "Housekeeping attendant" }
                });

            migrationBuilder.CreateIndex(
                name: "i_x_access_events_credential_id",
                table: "access_events",
                column: "credential_id");

            migrationBuilder.CreateIndex(
                name: "i_x_access_events_property_id_occurred_at",
                table: "access_events",
                columns: new[] { "property_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "i_x_booking_confirmations_booking_id",
                table: "booking_confirmations",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "i_x_bookings_property_id_code",
                table: "bookings",
                columns: new[] { "property_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_bookings_room_id_check_in_date",
                table: "bookings",
                columns: new[] { "room_id", "check_in_date" });

            migrationBuilder.CreateIndex(
                name: "i_x_credentials_property_id_card_id",
                table: "credentials",
                columns: new[] { "property_id", "card_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_credentials_staff_member_id",
                table: "credentials",
                column: "staff_member_id");

            migrationBuilder.CreateIndex(
                name: "i_x_daily_rates_rate_plan_id_room_type_id_rate_date",
                table: "daily_rates",
                columns: new[] { "rate_plan_id", "room_type_id", "rate_date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_inventory_items_property_id_code",
                table: "inventory_items",
                columns: new[] { "property_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_inventory_stocks_inventory_item_id_storage_location_id",
                table: "inventory_stocks",
                columns: new[] { "inventory_item_id", "storage_location_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_payments_booking_id_idempotency_key",
                table: "payments",
                columns: new[] { "booking_id", "idempotency_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_payments_property_id",
                table: "payments",
                column: "property_id");

            migrationBuilder.CreateIndex(
                name: "i_x_rate_plans_property_id_name",
                table: "rate_plans",
                columns: new[] { "property_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_room_types_property_id_name",
                table: "room_types",
                columns: new[] { "property_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_rooms_property_id_number",
                table: "rooms",
                columns: new[] { "property_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_rooms_room_type_id",
                table: "rooms",
                column: "room_type_id");

            migrationBuilder.CreateIndex(
                name: "i_x_status_periods_room_id_start_date",
                table: "status_periods",
                columns: new[] { "room_id", "start_date" });

            migrationBuilder.CreateIndex(
                name: "i_x_stock_adjustments_inventory_item_id",
                table: "stock_adjustments",
                column: "inventory_item_id");

            migrationBuilder.CreateIndex(
                name: "i_x_storage_locations_property_id_code",
                table: "storage_locations",
                columns: new[] { "property_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_users_organization_id",
                table: "users",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "i_x_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "access_events");

            migrationBuilder.DropTable(
                name: "booking_confirmations");

            migrationBuilder.DropTable(
                name: "daily_rates");

            migrationBuilder.DropTable(
                name: "inventory_stocks");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "properties");

            migrationBuilder.DropTable(
                name: "rate_plan_room_types");

            migrationBuilder.DropTable(
                name: "rate_plan_services");

            migrationBuilder.DropTable(
                name: "status_periods");

            migrationBuilder.DropTable(
                name: "stock_adjustments");

            migrationBuilder.DropTable(
                name: "storage_locations");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "credentials");

            migrationBuilder.DropTable(
                name: "bookings");

            migrationBuilder.DropTable(
                name: "rate_plans");

            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "inventory_items");

            migrationBuilder.DropTable(
                name: "staff_members");

            migrationBuilder.DropTable(
                name: "room_types");
        }
    }
}
