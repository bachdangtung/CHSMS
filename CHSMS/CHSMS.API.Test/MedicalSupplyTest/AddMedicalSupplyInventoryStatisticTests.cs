using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class AddMedicalSupplyInventoryStatisticTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public AddMedicalSupplyInventoryStatisticTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);
        }

        [Fact]
        public void AddMedicalSupplyInventoryStatistic_ValidList_ReturnsTrue()
        {
            // Arrange
            var msiStatisticDTOs = new List<MSIStatisticDTO>
            {
                new MSIStatisticDTO
                {
                    Msisid = 1,
                    MsinventoryId = 1,
                    Quantity = 100,
                    ActualQuantity = 90,
                    StatisticPerson = 1,
                    StatisticDate = DateTime.Now,
                    IsUpdate = false,
                    Note = "Test statistic"
                }
            };
            _mockRepository.Setup(repo => repo.GetAllMSISNotConfirm())
                .Returns(new List<MedicalSupplyInventoryStatistic>());
            _mockRepository.Setup(repo => repo.AddMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()))
                .Returns(true);

            // Act
            var result = _service.AddMedicalSupplyInventoryStatistic(msiStatisticDTOs);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.AddMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()), Times.Once());
        }

        [Fact]
        public void AddMedicalSupplyInventoryStatistic_EmptyList_ReturnsFalse()
        {
            // Arrange
            var msiStatisticDTOs = new List<MSIStatisticDTO>();

            // Act
            var result = _service.AddMedicalSupplyInventoryStatistic(msiStatisticDTOs);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(repo => repo.AddMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()), Times.Never());
        }

        [Fact]
        public void AddMedicalSupplyInventoryStatistic_NullMsinventoryId_ThrowsException()
        {
            // Arrange
            var msiStatisticDTOs = new List<MSIStatisticDTO>
            {
                new MSIStatisticDTO
                {
                    Msisid = 1,
                    MsinventoryId = null,
                    Quantity = 100,
                    ActualQuantity = 90,
                    StatisticPerson = 1,
                    StatisticDate = DateTime.Now,
                    IsUpdate = false
                }
            };

            // Act & Assert
            var exception = Assert.ThrowsAny<Exception>(() => _service.AddMedicalSupplyInventoryStatistic(msiStatisticDTOs));
            _mockRepository.Verify(repo => repo.AddMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()), Times.Never());
        }

        [Fact]
        public void AddMedicalSupplyInventoryStatistic_NullQuantity_ThrowsException()
        {
            // Arrange
            var msiStatisticDTOs = new List<MSIStatisticDTO>
            {
                new MSIStatisticDTO
                {
                    Msisid = 1,
                    MsinventoryId = 1,
                    Quantity = null,
                    ActualQuantity = 90,
                    StatisticPerson = 1,
                    StatisticDate = DateTime.Now,
                    IsUpdate = false
                }
            };

            // Act & Assert
            var exception = Assert.ThrowsAny<Exception>(() => _service.AddMedicalSupplyInventoryStatistic(msiStatisticDTOs));
            _mockRepository.Verify(repo => repo.AddMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()), Times.Never());
        }

        [Fact]
        public void AddMedicalSupplyInventoryStatistic_NullActualQuantity_ThrowsException()
        {
            // Arrange
            var msiStatisticDTOs = new List<MSIStatisticDTO>
            {
                new MSIStatisticDTO
                {
                    Msisid = 1,
                    MsinventoryId = 1,
                    Quantity = 100,
                    ActualQuantity = null,
                    StatisticPerson = 1,
                    StatisticDate = DateTime.Now,
                    IsUpdate = false
                }
            };

            // Act & Assert
            var exception = Assert.ThrowsAny<Exception>(() => _service.AddMedicalSupplyInventoryStatistic(msiStatisticDTOs));
            _mockRepository.Verify(repo => repo.AddMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()), Times.Never());
        }

        [Fact]
        public void AddMedicalSupplyInventoryStatistic_NullStatisticPerson_ThrowsException()
        {
            // Arrange
            var msiStatisticDTOs = new List<MSIStatisticDTO>
            {
                new MSIStatisticDTO
                {
                    Msisid = 1,
                    MsinventoryId = 1,
                    Quantity = 100,
                    ActualQuantity = 90,
                    StatisticPerson = null,
                    StatisticDate = DateTime.Now,
                    IsUpdate = false
                }
            };

            // Act & Assert
            var exception = Assert.ThrowsAny<Exception>(() => _service.AddMedicalSupplyInventoryStatistic(msiStatisticDTOs));
            _mockRepository.Verify(repo => repo.AddMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()), Times.Never());
        }

        [Fact]
        public void AddMedicalSupplyInventoryStatistic_NullStatisticDate_ThrowsException()
        {
            // Arrange
            var msiStatisticDTOs = new List<MSIStatisticDTO>
            {
                new MSIStatisticDTO
                {
                    Msisid = 1,
                    MsinventoryId = 1,
                    Quantity = 100,
                    ActualQuantity = 90,
                    StatisticPerson = 1,
                    StatisticDate = null,
                    IsUpdate = false
                }
            };

            // Act & Assert
            var exception = Assert.ThrowsAny<Exception>(() => _service.AddMedicalSupplyInventoryStatistic(msiStatisticDTOs));
            _mockRepository.Verify(repo => repo.AddMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()), Times.Never());
        }

        [Fact]
        public void AddMedicalSupplyInventoryStatistic_DuplicateMsinventoryId_ThrowsException()
        {
            // Arrange
            var msiStatisticDTOs = new List<MSIStatisticDTO>
            {
                new MSIStatisticDTO
                {
                    Msisid = 1,
                    MsinventoryId = 1,
                    Quantity = 100,
                    ActualQuantity = 90,
                    StatisticPerson = 1,
                    StatisticDate = DateTime.Now,
                    IsUpdate = false
                },
                new MSIStatisticDTO
                {
                    Msisid = 2,
                    MsinventoryId = 1,
                    Quantity = 50,
                    ActualQuantity = 40,
                    StatisticPerson = 1,
                    StatisticDate = DateTime.Now,
                    IsUpdate = false
                }
            };
            _mockRepository.Setup(repo => repo.GetAllMSISNotConfirm())
                .Returns(new List<MedicalSupplyInventoryStatistic>
                {
                    new MedicalSupplyInventoryStatistic { Msisid = 3, MsinventoryId = 1 }
                });

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.AddMedicalSupplyInventoryStatistic(msiStatisticDTOs));
            Assert.Equal("Vật tư này đã tồn tại trong danh sách kiểm kê", exception.Message);
            _mockRepository.Verify(repo => repo.AddMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()), Times.Never());
        }
    }
}