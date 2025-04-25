using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class AddMedicineInventoryListTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public AddMedicineInventoryListTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_medicineRepositoryMock.Object, _contextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void AddMedicineInventoryList_AddsValidInventories()
        {
            // Arrange
            var dtoList = new List<MedicineInventoryAddDTO>
            {
                TestHelper.CreateMedicineInventoryAddDTO(1, 100)
            };
            var medicine = TestHelper.CreateMedicine(1);
            _medicineRepositoryMock.Setup(repo => repo.CheckDuplicateBatch(1, "BATCH001", It.IsAny<DateTime>())).Returns(false);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicine(1)).Returns(medicine);
            _medicineRepositoryMock.Setup(repo => repo.CalculateExpiryDate(It.IsAny<DateTime>(), 24)).Returns(DateTime.Now.AddMonths(18));
            _medicineRepositoryMock.Setup(repo => repo.AddMedicineInventoryList(It.IsAny<List<MedicineInventory>>())).Returns(true);

            // Act
            var result = _service.AddMedicineInventoryList(dtoList, 1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.AddedCount);
            Assert.Empty(result.Warnings);
        }

        [Fact]
        public void AddMedicineInventoryList_HandlesDuplicateBatch()
        {
            // Arrange
            var dtoList = new List<MedicineInventoryAddDTO>
            {
                TestHelper.CreateMedicineInventoryAddDTO(1, 100)
            };
            _medicineRepositoryMock.Setup(repo => repo.CheckDuplicateBatch(1, "BATCH001", It.IsAny<DateTime>())).Returns(true);

            // Act
            var result = _service.AddMedicineInventoryList(dtoList, 1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(0, result.AddedCount);
            Assert.Contains("số lô BATCH001 đã nhập", result.Warnings[0]);
        }

        [Fact]
        public void AddMedicineInventoryList_HandlesInvalidMedicine()
        {
            // Arrange
            var dtoList = new List<MedicineInventoryAddDTO>
            {
                TestHelper.CreateMedicineInventoryAddDTO(1, 100)
            };
            _medicineRepositoryMock.Setup(repo => repo.CheckDuplicateBatch(1, "BATCH001", It.IsAny<DateTime>())).Returns(false);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicine(1)).Returns((Medicine)null);

            // Act
            var result = _service.AddMedicineInventoryList(dtoList, 1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(0, result.AddedCount);
            Assert.Contains("Thuốc ID 1 không tồn tại", result.Warnings[0]);
        }
    }
}
