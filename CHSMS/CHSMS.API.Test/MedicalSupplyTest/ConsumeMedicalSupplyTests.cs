using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class ConsumeMedicalSupplyTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public ConsumeMedicalSupplyTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);
        }

        [Fact]
        public void ConsumeMedicalSupply_ReturnsConsumpMSID_WhenRepositorySucceeds()
        {
            // Arrange
            var dto = CreateConsumpMSDTO();
            int expectedConsumpMSID = 1;
            _mockRepository.Setup(repo => repo.ConsumeMedicalSupplyByMSID(dto))
                           .Returns(expectedConsumpMSID);

            // Act
            var result = _service.ConsumeMedicalSupply(dto);

            // Assert
            Assert.Equal(expectedConsumpMSID, result);
            _mockRepository.Verify(repo => repo.ConsumeMedicalSupplyByMSID(dto), Times.Once());
        }

        [Fact]
        public void ConsumeMedicalSupply_ReturnsZero_WhenRepositoryFails()
        {
            // Arrange
            var dto = CreateConsumpMSDTO();
            _mockRepository.Setup(repo => repo.ConsumeMedicalSupplyByMSID(dto))
                           .Returns(0);

            // Act
            var result = _service.ConsumeMedicalSupply(dto);

            // Assert
            Assert.Equal(0, result);
            _mockRepository.Verify(repo => repo.ConsumeMedicalSupplyByMSID(dto), Times.Once());
        }

        private ConsumpMSDTO CreateConsumpMSDTO()
        {
            return new ConsumpMSDTO
            {
                ConsumpMSID = 1,
                MedicalSupplyInventoryId = 1,
                Quantity = 10,
                Status = true,
                Note = "Consumed for patient care"
            };
        }
    }
}