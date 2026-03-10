using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CHSMS.API.Migrations
{
    public partial class InitDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MedicalRecord",
                columns: table => new
                {
                    MedicalRecordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PatientName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Gender = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DOB = table.Column<DateTime>(type: "date", nullable: true),
                    EthnicGroup = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EducationLevel = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HealthInsurance = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Job = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    Note = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecord", x => x.MedicalRecordID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Medicine",
                columns: table => new
                {
                    MedicineID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MedicineName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ActiveIngredient = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Dosage = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DosageForm = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImportPrice = table.Column<double>(type: "double", nullable: true),
                    SellingPrice = table.Column<double>(type: "double", nullable: true),
                    ShelfLife = table.Column<int>(type: "int", nullable: true),
                    BidNumber = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    IsBHYT = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    MedicineCode = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicine", x => x.MedicineID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    SupplierID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContactInfo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.SupplierID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Gender = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DOB = table.Column<DateTime>(type: "date", nullable: true),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleID = table.Column<int>(type: "int", nullable: true),
                    PhoneNumber = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResetToken = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResetTokenExpiry = table.Column<DateTime>(type: "datetime", nullable: true),
                    Specialization = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    Fullname = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RefreshToken = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                    table.ForeignKey(
                        name: "FK__Users__RoleID__5812160E",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MedicalSupplies",
                columns: table => new
                {
                    MedicalSupplyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MedicalSupplyName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SupplyType = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UnitOfMeasure = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SupplierID = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    ImportPrice = table.Column<double>(type: "double", nullable: true),
                    SellingPrice = table.Column<double>(type: "double", nullable: true),
                    BidNumber = table.Column<int>(type: "int", nullable: true),
                    MedicalSupplyCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalSupplies", x => x.MedicalSupplyID);
                    table.ForeignKey(
                        name: "FK__MedicalSu__Suppl__49C3F6B7",
                        column: x => x.SupplierID,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MedicalRecordHistory",
                columns: table => new
                {
                    MedicalRecordHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MedicalRecordID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime", nullable: true),
                    Address = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DiagnoseConclusion = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TreatmentMethod = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Pulse = table.Column<double>(type: "double", nullable: true),
                    BloodPressure = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RespiratoryRate = table.Column<double>(type: "double", nullable: true),
                    Temperature = table.Column<double>(type: "double", nullable: true),
                    Height = table.Column<double>(type: "double", nullable: true),
                    Weight = table.Column<double>(type: "double", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    Symptom = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MedicalRecordHistoryCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InsuranceExemption = table.Column<double>(type: "double", nullable: true),
                    PatientCategory = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DiseaseProgress = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DiseaseStage = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ICD = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MedicalOrder = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TreatmentBed = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecordHistory", x => x.MedicalRecordHistoryID);
                    table.ForeignKey(
                        name: "FK_MedicalRecordHistory_MedicalRecord",
                        column: x => x.MedicalRecordID,
                        principalTable: "MedicalRecord",
                        principalColumn: "MedicalRecordID");
                    table.ForeignKey(
                        name: "FK_MedicalRecordHistory_Users",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MedicineInventory",
                columns: table => new
                {
                    MedicineInventoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MedicineID = table.Column<int>(type: "int", nullable: false),
                    CertificateNumber = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TransactionType = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    Quantity = table.Column<double>(type: "double", nullable: true),
                    ManufacturingDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ReceiverId = table.Column<int>(type: "int", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "date", nullable: true),
                    Note = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BatchNumber = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SupplierID = table.Column<int>(type: "int", nullable: true),
                    ImportQuantity = table.Column<double>(type: "double", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineInventory", x => x.MedicineInventoryID);
                    table.ForeignKey(
                        name: "FK__MedicalIn__Medic__5DCAEF64",
                        column: x => x.MedicineID,
                        principalTable: "Medicine",
                        principalColumn: "MedicineID");
                    table.ForeignKey(
                        name: "FK_MedicineInventory_Suppliers",
                        column: x => x.SupplierID,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierID");
                    table.ForeignKey(
                        name: "FK_MedicineInventory_Users",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "UserID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MedicalSupplyInventory",
                columns: table => new
                {
                    SupplyInventoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MedicalSupplyID = table.Column<int>(type: "int", nullable: false),
                    CertificateNumber = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TransactionType = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    Quantity = table.Column<double>(type: "double", nullable: true),
                    ManufactureDate = table.Column<DateTime>(type: "date", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "date", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "date", nullable: true),
                    ReceiverId = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BatchNumber = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImportQuantity = table.Column<double>(type: "double", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MedicalS__8C3DCF4A17054FB0", x => x.SupplyInventoryID);
                    table.ForeignKey(
                        name: "FK__MedicalSu__Medic__4CA06362",
                        column: x => x.MedicalSupplyID,
                        principalTable: "MedicalSupplies",
                        principalColumn: "MedicalSupplyID");
                    table.ForeignKey(
                        name: "FK_MedicalSupplyInventory_Users",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "UserID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ExternalPrescription",
                columns: table => new
                {
                    ExternalPrescriptionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MedicalRecordHistoryID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    Note = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsBHYT = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalPrescription", x => x.ExternalPrescriptionID);
                    table.ForeignKey(
                        name: "FK_ExternalPrescription_MedicalRecordHistory",
                        column: x => x.MedicalRecordHistoryID,
                        principalTable: "MedicalRecordHistory",
                        principalColumn: "MedicalRecordHistoryID");
                    table.ForeignKey(
                        name: "FK_ExternalPrescription_Users",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    PrescriptionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MedicalRecordHistoryID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    Note = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsBHYT = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.PrescriptionID);
                    table.ForeignKey(
                        name: "FK__Prescript__UserI__5535A963",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_Prescriptions_MedicalRecordHistory",
                        column: x => x.MedicalRecordHistoryID,
                        principalTable: "MedicalRecordHistory",
                        principalColumn: "MedicalRecordHistoryID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UseMedicalSupplies",
                columns: table => new
                {
                    UseMedicalSupplieID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MedicalRecordHistoryID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    Note = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UseMedicalSupplies", x => x.UseMedicalSupplieID);
                    table.ForeignKey(
                        name: "FK_UseMedicalSupplies_MedicalRecordHistory",
                        column: x => x.MedicalRecordHistoryID,
                        principalTable: "MedicalRecordHistory",
                        principalColumn: "MedicalRecordHistoryID");
                    table.ForeignKey(
                        name: "FK_UseMedicalSupplies_Users",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MedicineConsumption",
                columns: table => new
                {
                    MedicineConsumptionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MedicineInventoryID = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<double>(type: "double", nullable: true),
                    ConsumptionDate = table.Column<DateTime>(type: "date", nullable: true),
                    IsSpecialMedicine = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    Note = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineConsumption", x => x.MedicineConsumptionID);
                    table.ForeignKey(
                        name: "FK_MedicineConsumption_MedicineInventory",
                        column: x => x.MedicineInventoryID,
                        principalTable: "MedicineInventory",
                        principalColumn: "MedicineInventoryID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MedicineInventoryStatistics",
                columns: table => new
                {
                    MedicineInventoryStatisticsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MedicineInventoryID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "double", nullable: false),
                    ActualQuantity = table.Column<double>(type: "double", nullable: false),
                    StatisticPerson = table.Column<int>(type: "int", nullable: false),
                    ConfirmPerson = table.Column<int>(type: "int", nullable: true),
                    StatisticDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ConfirmDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsUpdate = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineInventoryStatistics", x => x.MedicineInventoryStatisticsID);
                    table.ForeignKey(
                        name: "FK_MedicineInventoryStatistics_MedicineInventory",
                        column: x => x.MedicineInventoryID,
                        principalTable: "MedicineInventory",
                        principalColumn: "MedicineInventoryID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MedicalSupplyConsumption",
                columns: table => new
                {
                    MSConsumptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MedicalSupplyInventoryID = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<double>(type: "double", nullable: true),
                    ConsumptionDate = table.Column<DateTime>(type: "date", nullable: true),
                    Note = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalSupplyConsumption", x => x.MSConsumptionId);
                    table.ForeignKey(
                        name: "FK_MedicalSupplyConsumption_MedicalSupplyInventory1",
                        column: x => x.MedicalSupplyInventoryID,
                        principalTable: "MedicalSupplyInventory",
                        principalColumn: "SupplyInventoryID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MedicalSupplyInventoryStatistics",
                columns: table => new
                {
                    MSISID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MSInventoryID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "double", nullable: false),
                    ActualQuantity = table.Column<double>(type: "double", nullable: false),
                    StatisticPerson = table.Column<int>(type: "int", nullable: false),
                    ConfirmPerson = table.Column<int>(type: "int", nullable: true),
                    StatisticDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ConfirmDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsUpdate = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalSupplyInventoryStatistics", x => x.MSISID);
                    table.ForeignKey(
                        name: "FK_MedicalSupplyInventoryStatistics_MedicalSupplyInventory",
                        column: x => x.MSInventoryID,
                        principalTable: "MedicalSupplyInventory",
                        principalColumn: "SupplyInventoryID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Medicine_Prescription",
                columns: table => new
                {
                    MedicineID = table.Column<int>(type: "int", nullable: false),
                    ExternalPrescriptionID = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Amount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicine_Prescription", x => new { x.ExternalPrescriptionID, x.MedicineID });
                    table.ForeignKey(
                        name: "FK_Medicine_Prescription_ExternalPrescription",
                        column: x => x.ExternalPrescriptionID,
                        principalTable: "ExternalPrescription",
                        principalColumn: "ExternalPrescriptionID");
                    table.ForeignKey(
                        name: "FK_Medicine_Prescription_Medicine",
                        column: x => x.MedicineID,
                        principalTable: "Medicine",
                        principalColumn: "MedicineID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Prescription_MedicineConsumption",
                columns: table => new
                {
                    MedicineConsumtionID = table.Column<int>(type: "int", nullable: false),
                    PrescriptionID = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<double>(type: "double", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescription_MedicineConsumption", x => new { x.MedicineConsumtionID, x.PrescriptionID });
                    table.ForeignKey(
                        name: "FK_Prescription_MedicineConsumption_MedicineConsumption",
                        column: x => x.MedicineConsumtionID,
                        principalTable: "MedicineConsumption",
                        principalColumn: "MedicineConsumptionID");
                    table.ForeignKey(
                        name: "FK_Prescription_MedicineConsumption_Prescriptions",
                        column: x => x.PrescriptionID,
                        principalTable: "Prescriptions",
                        principalColumn: "PrescriptionID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UseMedicalSupplies_MedicalSupplyConsumption",
                columns: table => new
                {
                    MSConsumptionId = table.Column<int>(type: "int", nullable: false),
                    UseMedicalSupplieID = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<double>(type: "double", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UseMedicalSupplies_MedicalSupplyConsumption", x => new { x.MSConsumptionId, x.UseMedicalSupplieID });
                    table.ForeignKey(
                        name: "FK_UseMedicalSupplies_MedicalSupplyConsumption_MedicalSupplyConsumption",
                        column: x => x.MSConsumptionId,
                        principalTable: "MedicalSupplyConsumption",
                        principalColumn: "MSConsumptionId");
                    table.ForeignKey(
                        name: "FK_UseMedicalSupplies_MedicalSupplyConsumption_UseMedicalSupplies",
                        column: x => x.UseMedicalSupplieID,
                        principalTable: "UseMedicalSupplies",
                        principalColumn: "UseMedicalSupplieID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalPrescription_MedicalRecordHistoryID",
                table: "ExternalPrescription",
                column: "MedicalRecordHistoryID");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalPrescription_UserID",
                table: "ExternalPrescription",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecordHistory_MedicalRecordID",
                table: "MedicalRecordHistory",
                column: "MedicalRecordID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecordHistory_UserID",
                table: "MedicalRecordHistory",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalSupplies_SupplierID",
                table: "MedicalSupplies",
                column: "SupplierID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalSupplyConsumption_MedicalSupplyInventoryID",
                table: "MedicalSupplyConsumption",
                column: "MedicalSupplyInventoryID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalSupplyInventory_MedicalSupplyID",
                table: "MedicalSupplyInventory",
                column: "MedicalSupplyID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalSupplyInventory_ReceiverId",
                table: "MedicalSupplyInventory",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalSupplyInventoryStatistics_MSInventoryID",
                table: "MedicalSupplyInventoryStatistics",
                column: "MSInventoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_Prescription_MedicineID",
                table: "Medicine_Prescription",
                column: "MedicineID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineConsumption_MedicineInventoryID",
                table: "MedicineConsumption",
                column: "MedicineInventoryID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineInventory_MedicineID",
                table: "MedicineInventory",
                column: "MedicineID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineInventory_ReceiverId",
                table: "MedicineInventory",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineInventory_SupplierID",
                table: "MedicineInventory",
                column: "SupplierID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineInventoryStatistics_MedicineInventoryID",
                table: "MedicineInventoryStatistics",
                column: "MedicineInventoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_MedicineConsumption_PrescriptionID",
                table: "Prescription_MedicineConsumption",
                column: "PrescriptionID");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_MedicalRecordHistoryID",
                table: "Prescriptions",
                column: "MedicalRecordHistoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_UserID",
                table: "Prescriptions",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UseMedicalSupplies_MedicalRecordHistoryID",
                table: "UseMedicalSupplies",
                column: "MedicalRecordHistoryID");

            migrationBuilder.CreateIndex(
                name: "IX_UseMedicalSupplies_UserID",
                table: "UseMedicalSupplies",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UseMedicalSupplies_MedicalSupplyConsumption_UseMedicalSuppli~",
                table: "UseMedicalSupplies_MedicalSupplyConsumption",
                column: "UseMedicalSupplieID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleID",
                table: "Users",
                column: "RoleID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicalSupplyInventoryStatistics");

            migrationBuilder.DropTable(
                name: "Medicine_Prescription");

            migrationBuilder.DropTable(
                name: "MedicineInventoryStatistics");

            migrationBuilder.DropTable(
                name: "Prescription_MedicineConsumption");

            migrationBuilder.DropTable(
                name: "UseMedicalSupplies_MedicalSupplyConsumption");

            migrationBuilder.DropTable(
                name: "ExternalPrescription");

            migrationBuilder.DropTable(
                name: "MedicineConsumption");

            migrationBuilder.DropTable(
                name: "Prescriptions");

            migrationBuilder.DropTable(
                name: "MedicalSupplyConsumption");

            migrationBuilder.DropTable(
                name: "UseMedicalSupplies");

            migrationBuilder.DropTable(
                name: "MedicineInventory");

            migrationBuilder.DropTable(
                name: "MedicalSupplyInventory");

            migrationBuilder.DropTable(
                name: "MedicalRecordHistory");

            migrationBuilder.DropTable(
                name: "Medicine");

            migrationBuilder.DropTable(
                name: "MedicalSupplies");

            migrationBuilder.DropTable(
                name: "MedicalRecord");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
