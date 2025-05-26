using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class GetMedicalSupplyInventoryStatisticsByIdTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public GetMedicalSupplyInventoryStatisticsByIdTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);

            // Setup precondition: MedicalSupplyInventoryStatistic with Msisid: 1 exists
            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryStatisticById(1))
                .Returns(new MedicalSupplyInventoryStatistic
                {
                    Msisid = 1,
                    MsinventoryId = 1,
                    Quantity = 10,
                    ActualQuantity = 10,
                    StatisticPerson = 1,
                    StatisticDate = DateTime.Now,
                    IsUpdate = false
                });
            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryStatisticById(-1))
                .Returns((MedicalSupplyInventoryStatistic)null);
        }

        [Fact]
        public void GetMedicalSupplyInventoryStatisticsById_ValidId_ReturnsStatistic()
        {
            // Arrange
            int msisid = 1;

            // Act
            var result = _service.GetMedicalSupplyInventoryStatisticsById(msisid);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Msisid);
            Assert.Equal(10, result.Quantity);
            Assert.Equal(10, result.ActualQuantity);
            Assert.Equal(1, result.StatisticPerson);
            _mockRepository.Verify(r => r.GetMedicalSupplyInventoryStatisticById(1), Times.Once());
        }

        [Fact]
        public void GetMedicalSupplyInventoryStatisticsById_InvalidId_ReturnsNull()
        {
            // Arrange
            int msisid = -1;

            // Act
            var result = _service.GetMedicalSupplyInventoryStatisticsById(msisid);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.GetMedicalSupplyInventoryStatisticById(-1), Times.Once());
        }
    }
}