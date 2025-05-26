using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class DeleteMedicineInventoryStatisticTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _medicineService;

        public DeleteMedicineInventoryStatisticTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _medicineService = new MedicineService(_medicineRepositoryMock.Object, _loggerMock.Object);
        }
        [Fact]
        public void DeleteMedicineInventoryStatistic_ValidId_ReturnsTrue()
        {
            // Arrange
            var medicineInventoryStatisticId = 1;
            var medicineInventoryStatistic = new MedicineInventoryStatistic
            {
                MedicineInventoryStatisticsId = medicineInventoryStatisticId,
                MedicineInventoryId = 1,
                Quantity = 100,
                ActualQuantity = 95,
                StatisticPerson = 1,
                StatisticDate = DateTime.Now
            };

            _medicineRepositoryMock
                .Setup(repo => repo.GetMedicineInventoryStatisticById(medicineInventoryStatisticId))
                .Returns(medicineInventoryStatistic);
            _medicineRepositoryMock
                .Setup(repo => repo.DeleteMedicineInventoryStatistic(medicineInventoryStatistic))
                .Returns(true);

            // Act
            var result = _medicineService.DeleteMedicineInventoryStatistic(medicineInventoryStatisticId);

            // Assert
            Assert.True(result);
            _medicineRepositoryMock.Verify(repo => repo.GetMedicineInventoryStatisticById(medicineInventoryStatisticId), Times.Once());
            _medicineRepositoryMock.Verify(repo => repo.DeleteMedicineInventoryStatistic(medicineInventoryStatistic), Times.Once());
        }

        [Fact]
        public void DeleteMedicineInventoryStatistic_InvalidId_ReturnsFalse()
        {
            // Arrange
            var medicineInventoryStatisticId = -1;

            _medicineRepositoryMock
                .Setup(repo => repo.GetMedicineInventoryStatisticById(medicineInventoryStatisticId))
                .Returns((MedicineInventoryStatistic)null);

            // Act
            var result = _medicineService.DeleteMedicineInventoryStatistic(medicineInventoryStatisticId);

            // Assert
            Assert.False(result);
            _medicineRepositoryMock.Verify(repo => repo.GetMedicineInventoryStatisticById(medicineInventoryStatisticId), Times.Once());
            _medicineRepositoryMock.Verify(repo => repo.DeleteMedicineInventoryStatistic(It.IsAny<MedicineInventoryStatistic>()), Times.Never());
        }
    }
}
