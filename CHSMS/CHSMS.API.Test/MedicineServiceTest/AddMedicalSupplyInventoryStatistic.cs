using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class AddMedicalSupplyInventoryStatistic
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _medicineService;

        public AddMedicineInventoryListTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _medicineService = new MedicineService(_medicineRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void AddMedicineInventoryStatistic_ValidDTOList_ReturnsTrue()
        {
            // Arrange
            var dtoList = new List<MedicineInventoryStatisticDTO>
        {
            new MedicineInventoryStatisticDTO
            {
                MedicineInventoryId = 1,
                Quantity = 100,
                ActualQuantity = 95,
                StatisticPerson = 1,
                StatisticDate = DateTime.Now,
                Note = "Test statistic"
            }
        };

            _medicineRepositoryMock.Setup(repo => repo.GetAllMSISNotConfirm()).Returns(new List<MedicineInventoryStatistic>());
            _medicineRepositoryMock.Setup(repo => repo.AddMedicineInventoryStatistic(It.IsAny<List<MedicineInventoryStatistic>>())).Returns(true);

            // Act
            var result = _medicineService.AddMedicineInventoryStatistic(dtoList);

            // Assert
            Assert.True(result);
            _medicineRepositoryMock.Verify(repo => repo.AddMedicineInventoryStatistic(It.Is<List<MedicineInventoryStatistic>>(list =>
                list.Count == 1 &&
                list[0].MedicineInventoryId == 1 &&
                list[0].Quantity == 100 &&
                list[0].ActualQuantity == 95 &&
                list[0].StatisticPerson == 1 &&
                list[0].Note == "Test statistic")), Times.Once());
        }

        [Fact]
        public void AddMedicineInventoryStatistic_EmptyDTOList_ReturnsFalse()
        {
            // Arrange
            var dtoList = new List<MedicineInventoryStatisticDTO>();

            // Act
            var result = _medicineService.AddMedicineInventoryStatistic(dtoList);

            // Assert
            Assert.False(result);
            _medicineRepositoryMock.Verify(repo => repo.AddMedicineInventoryStatistic(It.IsAny<List<MedicineInventoryStatistic>>()), Times.Never());
        }

        [Fact]
        public void AddMedicineInventoryStatistic_NullMedicineInventoryId_ThrowsException()
        {
            // Arrange
            var dtoList = new List<MedicineInventoryStatisticDTO>
        {
            new MedicineInventoryStatisticDTO
            {
                MedicineInventoryId = null,
                Quantity = 100,
                ActualQuantity = 95,
                StatisticPerson = 1,
                StatisticDate = DateTime.Now
            }
        };

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _medicineService.AddMedicineInventoryStatistic(dtoList));
            Assert.Equal("Medical supply inventory statistic is not valid", exception.Message);
            _medicineRepositoryMock.Verify(repo => repo.AddMedicineInventoryStatistic(It.IsAny<List<MedicineInventoryStatistic>>()), Times.Never());
        }

        [Fact]
        public void AddMedicineInventoryStatistic_NullQuantity_ThrowsException()
        {
            // Arrange
            var dtoList = new List<MedicineInventoryStatisticDTO>
        {
            new MedicineInventoryStatisticDTO
            {
                MedicineInventoryId = 1,
                Quantity = null,
                ActualQuantity = 95,
                StatisticPerson = 1,
                StatisticDate = DateTime.Now
            }
        };

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _medicineService.AddMedicineInventoryStatistic(dtoList));
            Assert.Equal("Medical supply inventory statistic is not valid", exception.Message);
            _medicineRepositoryMock.Verify(repo => repo.AddMedicineInventoryStatistic(It.IsAny<List<MedicineInventoryStatistic>>()), Times.Never());
        }

        [Fact]
        public void AddMedicineInventoryStatistic_NullActualQuantity_ThrowsException()
        {
            // Arrange
            var dtoList = new List<MedicineInventoryStatisticDTO>
        {
            new MedicineInventoryStatisticDTO
            {
                MedicineInventoryId = 1,
                Quantity = 100,
                ActualQuantity = null,
                StatisticPerson = 1,
                StatisticDate = DateTime.Now
            }
        };

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _medicineService.AddMedicineInventoryStatistic(dtoList));
            Assert.Equal("Medical supply inventory statistic is not valid", exception.Message);
            _medicineRepositoryMock.Verify(repo => repo.AddMedicineInventoryStatistic(It.IsAny<List<MedicineInventoryStatistic>>()), Times.Never());
        }

        [Fact]
        public void AddMedicineInventoryStatistic_NullStatisticPerson_ThrowsException()
        {
            // Arrange
            var dtoList = new List<MedicineInventoryStatisticDTO>
        {
            new MedicineInventoryStatisticDTO
            {
                MedicineInventoryId = 1,
                Quantity = 100,
                ActualQuantity = 95,
                StatisticPerson = null,
                StatisticDate = DateTime.Now
            }
        };

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _medicineService.AddMedicineInventoryStatistic(dtoList));
            Assert.Equal("Medical supply inventory statistic is not valid", exception.Message);
            _medicineRepositoryMock.Verify(repo => repo.AddMedicineInventoryStatistic(It.IsAny<List<MedicineInventoryStatistic>>()), Times.Never());
        }

        [Fact]
        public void AddMedicineInventoryStatistic_NullStatisticDate_ThrowsException()
        {
            // Arrange
            var dtoList = new List<MedicineInventoryStatisticDTO>
        {
            new MedicineInventoryStatisticDTO
            {
                MedicineInventoryId = 1,
                Quantity = 100,
                ActualQuantity = 95,
                StatisticPerson = 1,
                StatisticDate = null
            }
        };

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _medicineService.AddMedicineInventoryStatistic(dtoList));
            Assert.Equal("Medical supply inventory statistic is not valid", exception.Message);
            _medicineRepositoryMock.Verify(repo => repo.AddMedicineInventoryStatistic(It.IsAny<List<MedicineInventoryStatistic>>()), Times.Never());
        }

        [Fact]
        public void AddMedicineInventoryStatistic_DuplicateMedicineInventoryId_ThrowsException()
        {
            // Arrange
            var dtoList = new List<MedicineInventoryStatisticDTO>
        {
            new MedicineInventoryStatisticDTO
            {
                MedicineInventoryId = 1,
                Quantity = 100,
                ActualQuantity = 95,
                StatisticPerson = 1,
                StatisticDate = DateTime.Now
            },
            new MedicineInventoryStatisticDTO
            {
                MedicineInventoryId = 1, // Duplicate MedicineInventoryId
                Quantity = 90,
                ActualQuantity = 85,
                StatisticPerson = "User2",
                StatisticDate = DateTime.Now
            }
        };

            var existingStatistics = new List<MedicineInventoryStatistic>
        {
            new MedicineInventoryStatistic { MedicineInventoryId = 1 }
        };

            _medicineRepositoryMock.Setup(repo => repo.GetAllMSISNotConfirm()).Returns(existingStatistics);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _medicineService.AddMedicineInventoryStatistic(dtoList));
            Assert.Equal("Vật tư này đã tồn tại trong danh sách kiểm kê", exception.Message);
            _medicineRepositoryMock.Verify(repo => repo.AddMedicineInventoryStatistic(It.IsAny<List<MedicineInventoryStatistic>>()), Times.Never());
        }
    }
}
