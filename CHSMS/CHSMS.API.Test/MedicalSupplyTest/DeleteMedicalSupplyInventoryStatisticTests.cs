using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class DeleteMedicalSupplyInventoryStatisticTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public DeleteMedicalSupplyInventoryStatisticTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);

            // Setup precondition: MedicalSupplyInventoryStatistic with MsinventoryId: 1 exists
            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryStatisticById(1))
                .Returns(new MedicalSupplyInventoryStatistic { Msisid = 1, MsinventoryId = 1 });
        }

        [Fact]
        public void DeleteMedicalSupplyInventoryStatistic_ValidMsisid_ReturnsTrue()
        {
            // Arrange
            int msisid = 1;

            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryStatisticById(msisid))
                .Returns(new MedicalSupplyInventoryStatistic { Msisid = msisid, MsinventoryId = 1 });
            _mockRepository.Setup(r => r.DeleteMedicalSupplyInventoryStatistic(It.IsAny<MedicalSupplyInventoryStatistic>()))
                .Returns(true);

            // Act
            var result = _service.DeleteMedicalSupplyInventoryStatistic(msisid);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.DeleteMedicalSupplyInventoryStatistic(It.IsAny<MedicalSupplyInventoryStatistic>()), Times.Once());
        }

        [Fact]
        public void DeleteMedicalSupplyInventoryStatistic_InvalidMsisid_ReturnsFalse()
        {
            // Arrange
            int msisid = -1;

            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryStatisticById(msisid))
                .Returns((MedicalSupplyInventoryStatistic)null);

            // Act
            var result = _service.DeleteMedicalSupplyInventoryStatistic(msisid);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.DeleteMedicalSupplyInventoryStatistic(It.IsAny<MedicalSupplyInventoryStatistic>()), Times.Never());
        }
    }
}