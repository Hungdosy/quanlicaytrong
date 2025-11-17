using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeGarden.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "email_templates",
                columns: table => new
                {
                    template_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    body_html = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__email_te__BE44E07974A8B025", x => x.template_id);
                });

            migrationBuilder.CreateTable(
                name: "health_definitions",
                columns: table => new
                {
                    health_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__health_d__189960E39E5FD07E", x => x.health_id);
                });

            migrationBuilder.CreateTable(
                name: "payment_providers",
                columns: table => new
                {
                    provider_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    api_key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    secret_key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    endpoint_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__payment___00E21310C9441459", x => x.provider_id);
                });

            migrationBuilder.CreateTable(
                name: "resource_types",
                columns: table => new
                {
                    type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__resource__2C0005985FC3A50F", x => x.type_id);
                });

            migrationBuilder.CreateTable(
                name: "status_definitions",
                columns: table => new
                {
                    status_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    entity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__status_d__3683B53120DA08E7", x => x.status_id);
                });

            migrationBuilder.CreateTable(
                name: "subscription_plans",
                columns: table => new
                {
                    plan_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    price = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    billing_cycle = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Monthly"),
                    features = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__subscrip__BE9F8F1D00E7ACF0", x => x.plan_id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    status_id = table.Column<int>(type: "int", nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__roles__760965CCBC9A0AB9", x => x.role_id);
                    table.ForeignKey(
                        name: "FK__roles__status_id__52593CB8",
                        column: x => x.status_id,
                        principalTable: "status_definitions",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    fullname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__users__B9BE370F5B3BA653", x => x.user_id);
                    table.ForeignKey(
                        name: "FK__users__role_id__5812160E",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "role_id");
                    table.ForeignKey(
                        name: "FK__users__status_id__59063A47",
                        column: x => x.status_id,
                        principalTable: "status_definitions",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "areas",
                columns: table => new
                {
                    area_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__areas__985D6D6B239D401E", x => x.area_id);
                    table.ForeignKey(
                        name: "FK__areas__status_id__5EBF139D",
                        column: x => x.status_id,
                        principalTable: "status_definitions",
                        principalColumn: "status_id");
                    table.ForeignKey(
                        name: "FK__areas__user_id__5DCAEF64",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "email_notifications",
                columns: table => new
                {
                    email_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sent = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    send_time = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    sent_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__email_no__3FEF87664E6515AC", x => x.email_id);
                    table.ForeignKey(
                        name: "FK__email_not__user___3F115E1A",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "media_files",
                columns: table => new
                {
                    file_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: true),
                    entity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    entity_id = table.Column<long>(type: "bigint", nullable: true),
                    file_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    file_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    mime_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    size_kb = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    uploaded_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__media_fi__07D884C6442BAF5D", x => x.file_id);
                    table.ForeignKey(
                        name: "FK__media_fil__user___503BEA1C",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "resources",
                columns: table => new
                {
                    resource_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    type_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 1m),
                    unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "đơn vị"),
                    cost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__resource__4985FC730B20AE78", x => x.resource_id);
                    table.ForeignKey(
                        name: "FK__resources__statu__03F0984C",
                        column: x => x.status_id,
                        principalTable: "status_definitions",
                        principalColumn: "status_id");
                    table.ForeignKey(
                        name: "FK__resources__type___02FC7413",
                        column: x => x.type_id,
                        principalTable: "resource_types",
                        principalColumn: "type_id");
                    table.ForeignKey(
                        name: "FK__resources__user___02084FDA",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "user_notifications",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    channel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_read = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    sent_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    read_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__user_not__3213E83F371A5221", x => x.id);
                    table.ForeignKey(
                        name: "FK__user_noti__user___4C6B5938",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "user_subscriptions",
                columns: table => new
                {
                    subscription_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    plan_id = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    next_billing_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Active"),
                    payment_method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    last_payment_id = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__user_sub__863A7EC1F1B43AA2", x => x.subscription_id);
                    table.ForeignKey(
                        name: "FK__user_subs__plan___31B762FC",
                        column: x => x.plan_id,
                        principalTable: "subscription_plans",
                        principalColumn: "plan_id");
                    table.ForeignKey(
                        name: "FK__user_subs__user___30C33EC3",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "plants",
                columns: table => new
                {
                    plant_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    area_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    species = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    planted_date = table.Column<DateOnly>(type: "date", nullable: true),
                    image_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    health_id = table.Column<int>(type: "int", nullable: true),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__plants__A576B3B458207BB9", x => x.plant_id);
                    table.ForeignKey(
                        name: "FK__plants__area_id__6383C8BA",
                        column: x => x.area_id,
                        principalTable: "areas",
                        principalColumn: "area_id");
                    table.ForeignKey(
                        name: "FK__plants__health_i__656C112C",
                        column: x => x.health_id,
                        principalTable: "health_definitions",
                        principalColumn: "health_id");
                    table.ForeignKey(
                        name: "FK__plants__status_i__6477ECF3",
                        column: x => x.status_id,
                        principalTable: "status_definitions",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "subscription_payments",
                columns: table => new
                {
                    payment_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subscription_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    plan_id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true, defaultValue: "VND"),
                    payment_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    payment_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Pending"),
                    payment_method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    provider_ref = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    provider_id = table.Column<int>(type: "int", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__subscrip__ED1FC9EA52CA1362", x => x.payment_id);
                    table.ForeignKey(
                        name: "FK__subscript__plan___3A4CA8FD",
                        column: x => x.plan_id,
                        principalTable: "subscription_plans",
                        principalColumn: "plan_id");
                    table.ForeignKey(
                        name: "FK__subscript__provi__51300E55",
                        column: x => x.provider_id,
                        principalTable: "payment_providers",
                        principalColumn: "provider_id");
                    table.ForeignKey(
                        name: "FK__subscript__subsc__3864608B",
                        column: x => x.subscription_id,
                        principalTable: "user_subscriptions",
                        principalColumn: "subscription_id");
                    table.ForeignKey(
                        name: "FK__subscript__user___395884C4",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "alerts",
                columns: table => new
                {
                    alert_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    plant_id = table.Column<long>(type: "bigint", nullable: false),
                    alert_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    alert_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    resolved = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    resolved_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__alerts__4B8FB03AE54D1A27", x => x.alert_id);
                    table.ForeignKey(
                        name: "FK__alerts__plant_id__7A672E12",
                        column: x => x.plant_id,
                        principalTable: "plants",
                        principalColumn: "plant_id");
                    table.ForeignKey(
                        name: "FK__alerts__status_i__7B5B524B",
                        column: x => x.status_id,
                        principalTable: "status_definitions",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "plant_logs",
                columns: table => new
                {
                    log_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    plant_id = table.Column<long>(type: "bigint", nullable: false),
                    log_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    activity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    image_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    health_id = table.Column<int>(type: "int", nullable: true),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__plant_lo__9E2397E05E0A3742", x => x.log_id);
                    table.ForeignKey(
                        name: "FK__plant_log__healt__73BA3083",
                        column: x => x.health_id,
                        principalTable: "health_definitions",
                        principalColumn: "health_id");
                    table.ForeignKey(
                        name: "FK__plant_log__plant__71D1E811",
                        column: x => x.plant_id,
                        principalTable: "plants",
                        principalColumn: "plant_id");
                    table.ForeignKey(
                        name: "FK__plant_log__statu__72C60C4A",
                        column: x => x.status_id,
                        principalTable: "status_definitions",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "plant_resource_usage",
                columns: table => new
                {
                    usage_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    plant_id = table.Column<long>(type: "bigint", nullable: false),
                    resource_id = table.Column<long>(type: "bigint", nullable: false),
                    used_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    quantity_used = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__plant_re__B6B13A02021D9E92", x => x.usage_id);
                    table.ForeignKey(
                        name: "FK__plant_res__plant__09A971A2",
                        column: x => x.plant_id,
                        principalTable: "plants",
                        principalColumn: "plant_id");
                    table.ForeignKey(
                        name: "FK__plant_res__resou__0A9D95DB",
                        column: x => x.resource_id,
                        principalTable: "resources",
                        principalColumn: "resource_id");
                    table.ForeignKey(
                        name: "FK__plant_res__statu__0B91BA14",
                        column: x => x.status_id,
                        principalTable: "status_definitions",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "schedules",
                columns: table => new
                {
                    schedule_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    plant_id = table.Column<long>(type: "bigint", nullable: false),
                    task_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    frequency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    next_due = table.Column<DateTime>(type: "datetime", nullable: false),
                    last_done = table.Column<DateTime>(type: "datetime", nullable: true),
                    reminder = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__schedule__C46A8A6FB956B174", x => x.schedule_id);
                    table.ForeignKey(
                        name: "FK__schedules__plant__6B24EA82",
                        column: x => x.plant_id,
                        principalTable: "plants",
                        principalColumn: "plant_id");
                    table.ForeignKey(
                        name: "FK__schedules__statu__6C190EBB",
                        column: x => x.status_id,
                        principalTable: "status_definitions",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_alerts_plant_id",
                table: "alerts",
                column: "plant_id");

            migrationBuilder.CreateIndex(
                name: "IX_alerts_status_id",
                table: "alerts",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_areas_status_id",
                table: "areas",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_areas_user_id",
                table: "areas",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_email_notifications_user_id",
                table: "email_notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ__email_te__357D4CF9D155981E",
                table: "email_templates",
                column: "code",
                unique: true,
                filter: "[code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_media_files_user_id",
                table: "media_files",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ__payment___357D4CF9326AF7B0",
                table: "payment_providers",
                column: "code",
                unique: true,
                filter: "[code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_plant_logs_health_id",
                table: "plant_logs",
                column: "health_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_logs_plant_id",
                table: "plant_logs",
                column: "plant_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_logs_status_id",
                table: "plant_logs",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_resource_usage_plant_id",
                table: "plant_resource_usage",
                column: "plant_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_resource_usage_resource_id",
                table: "plant_resource_usage",
                column: "resource_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_resource_usage_status_id",
                table: "plant_resource_usage",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_plants_area_id",
                table: "plants",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "IX_plants_health_id",
                table: "plants",
                column: "health_id");

            migrationBuilder.CreateIndex(
                name: "IX_plants_status_id",
                table: "plants",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_resources_status_id",
                table: "resources",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_resources_type_id",
                table: "resources",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "IX_resources_user_id",
                table: "resources",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_roles_status_id",
                table: "roles",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "UQ__roles__783254B1C41E1C9E",
                table: "roles",
                column: "role_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_schedules_plant_id",
                table: "schedules",
                column: "plant_id");

            migrationBuilder.CreateIndex(
                name: "IX_schedules_status_id",
                table: "schedules",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_payments_plan_id",
                table: "subscription_payments",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_payments_provider_id",
                table: "subscription_payments",
                column: "provider_id");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_payments_subscription_id",
                table: "subscription_payments",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_payments_user_id",
                table: "subscription_payments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_notifications_user_id",
                table: "user_notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_subscriptions_plan_id",
                table: "user_subscriptions",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_subscriptions_user_id",
                table: "user_subscriptions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_status_id",
                table: "users",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "UQ__users__AB6E616447E67C59",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alerts");

            migrationBuilder.DropTable(
                name: "email_notifications");

            migrationBuilder.DropTable(
                name: "email_templates");

            migrationBuilder.DropTable(
                name: "media_files");

            migrationBuilder.DropTable(
                name: "plant_logs");

            migrationBuilder.DropTable(
                name: "plant_resource_usage");

            migrationBuilder.DropTable(
                name: "schedules");

            migrationBuilder.DropTable(
                name: "subscription_payments");

            migrationBuilder.DropTable(
                name: "user_notifications");

            migrationBuilder.DropTable(
                name: "resources");

            migrationBuilder.DropTable(
                name: "plants");

            migrationBuilder.DropTable(
                name: "payment_providers");

            migrationBuilder.DropTable(
                name: "user_subscriptions");

            migrationBuilder.DropTable(
                name: "resource_types");

            migrationBuilder.DropTable(
                name: "areas");

            migrationBuilder.DropTable(
                name: "health_definitions");

            migrationBuilder.DropTable(
                name: "subscription_plans");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "status_definitions");
        }
    }
}
