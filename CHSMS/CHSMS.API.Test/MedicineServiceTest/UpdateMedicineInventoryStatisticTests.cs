using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class UpdateMedicineInventoryStatisticTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _medicineService;

        public UpdateMedicineInventoryStatisticTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _medicineService = new MedicineService(_medicineRepositoryMock.Object, _loggerMock.Object);
        }
        [Fact]
        public void UpdateMedicineInventoryStatistic_ValidDTOListWithUpdate_ReturnsTrue()
        {
            // Arrange
            var medicineInventoryId = 1;
            var dtoList = new List<MedicineInventoryStatisticDTO>
        {
            new MedicineInventoryStatisticDTO
            {
                MedicineInventoryStatisticsId = 1,
                MedicineInventoryId = medicineInventoryId,
                Quantity = 20,
                ActualQuantity = 18,
                StatisticPerson = 1,
                StatisticDate = DateTime.Now,
                IsUpdate = true,
                Note = "Updated statistic"
            }
        };

            var medicineInventory = new MedicineInventory
            {
                MedicineInventoryId = medicineInventoryId,
                Quantity = 20
            };

            _medicineRepositoryMock
                .Setup(repo => repo.GetMedicineInventoryById(medicineInventoryId))
                .Returns(medicineInventory);
            _medicineRepositoryMock
                .Setup(repo => repo.UpdateMedicineInInventory(It.IsAny<List<MedicineInventory>>()))
                .Returns(true);
            _medicineRepositoryMock
                .Setup(repo => repo.UpdateMedicineInventoryStatistic(It.IsAny<List<MedicineInventoryStatistic>>()))
                .Returns(true);

            // Act
            var result = _medicineService.UpdateMedicineInventoryStatistic(dtoList);

            // Assert
            Assert.True(result);
            _medicineRepositoryMock.Verify(repo => repo.UpdateMedicineInInventory(It.Is<List<MedicineInventory>>(list =>
                list.Count == 1 &&
                list[0].MedicineInventoryId == medicineInventoryId &&
                list[0].Quantity == 18)), Times.Once());
            _medicineRepositoryMock.Verify(repo => repo.UpdateMedicineInventoryStatistic(It.Is<List<MedicineInventoryStatistic>>(list =>
                list.Count == 1 &&
                list[0].MedicineInventoryId == medicineInventoryId &&
                list[0].Quantity == 20 &&
                list[0].ActualQuantity == 18 &&
                list[0].Note == "Updated statistic" &&
                list[0].IsUpdate == true)), Times.Once());
        }

        [Fact]
        public void UpdateMedicineInventoryStatistic_ValidDTOListWithoutUpdate_ReturnsTrue()
        {
            // Arrange
            var medicineInventoryId = 1;
            var dtoList = new List<MedicineInventoryStatisticDTO>
        {
            new MedicineInventoryStatisticDTO
            {
                MedicineInventoryStatisticsId = 1,
                MedicineInventoryId = medicineInventoryId,
                Quantity = 20,
                ActualQuantity = 18,
                StatisticPerson = 1,
                StatisticDate = DateTime.Now,
                IsUpdate = false,
                Note = "Statistic without update"
            }
        };

            var medicineInventory = new MedicineInventory
            {
                MedicineInventoryId = medicineInventoryId,
                Quantity = 20
            };

            _medicineRepositoryMock
                .Setup(repo => repo.GetMedicineInventoryById(medicineInventoryId))
                .Returns(medicineInventory);
            _medicineRepositoryMock
                .Setup(repo => repo.UpdateMedicineInventoryStatistic(It.IsAny<List<MedicineInventoryStatistic>>()))
                .Returns(true);

            // Act
            var result = _medicineService.UpdateMedicineInventoryStatistic(dtoList);

            // Assert
            Assert.True(result);
            _medicineRepositoryMock.Verify(repo => repo.UpdateMedicineInInventory(It.IsAny<List<MedicineInventory>>()), Times.Never());
            _medicineRepositoryMock.Verify(repo => repo.UpdateMedicineInventoryStatistic(It.Is<List<MedicineInventoryStatistic>>(list =>
                list.Count == 1 &&
                list[0].MedicineInventoryId == medicineInventoryId &&
                list[0].Quantity == 20 &&
                list[0].ActualQuantity == 18 &&
                list[0].Note == "Statistic without update" &&
                list[0].IsUpdate == false)), Times.Once());
        }

        [Fact]
        public void UpdateMedicineInventoryStatistic_InvalidMedicineInventoryId_ThrowsException()
        {
            // Arrange
            var dtoList = new List<MedicineInventoryStatisticDTO>
        {
            new MedicineInventoryStatisticDTO
            {
                MedicineInventoryStatisticsId = 1,
                MedicineInventoryId = -1,
                Quantity = 20,
                ActualQuantity = 18,
                StatisticPerson = 1,
                StatisticDate = DateTime.Now,
                IsUpdate = true
            }
        };

            _medicineRepositoryMock
                .Setup(repo => repo.GetMedicineInventoryById(-1))
                .Returns((MedicineInventory)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _medicineService.UpdateMedicineInventoryStatistic(dtoList));
            Assert.Equal("Vật tư không hợp lệ", exception.Message);
            _medicineRepositoryMock.Verify(repo => repo.UpdateMedicineInInventory(It.IsAny<List<MedicineInventory>>()), Times.Never());
            _medicineRepositoryMock.Verify(repo => repo.UpdateMedicineInventoryStatistic(It.IsAny<List<MedicineInventoryStatistic>>()), Times.Never());
        }

        [Fact]
        public void UpdateMedicineInventoryStatistic_QuantityMismatch_ThrowsException()
        {
            // Arrange
            var medicineInventoryId = 1;
            var dtoList = new List<MedicineInventoryStatisticDTO>
        {
            new MedicineInventoryStatisticDTO
            {
                MedicineInventoryStatisticsId = 1,
                MedicineInventoryId = medicineInventoryId,
                Quantity = 30, // Mismatches MedicineInventory.Quantity (20)
                ActualQuantity = 18,
                StatisticPerson = 1,
                StatisticDate = DateTime.Now,
                IsUpdate = true
            }
        };

            var medicineInventory = new MedicineInventory
            {
                MedicineInventoryId = medicineInventoryId,
                Quantity = 20
            };

            _medicineRepositoryMock
                .Setup(repo => repo.GetMedicineInventoryById(medicineInventoryId))
                .Returns(medicineInventory);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _medicineService.UpdateMedicineInventoryStatistic(dtoList));
            Assert.Equal("Số lượng tồn không đồng nhất so với hệ thống", exception.Message);
            _medicineRepositoryMock.Verify(repo => repo.UpdateMedicineInInventory(It.IsAny<List<MedicineInventory>>()), Times.Never());
            _medicineRepositoryMock.Verify(repo => repo.UpdateMedicineInventoryStatistic(It.IsAny<List<MedicineInventoryStatistic>>()), Times.Never());
        }
    }
}
