using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class ConsumptionHistoryTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public ConsumptionHistoryTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);
        }

        [Fact]
        public void ConsumptionHistory_ReturnsConsumptionList_WhenDataExists()
        {
            // Arrange
            DateTime? from = DateTime.Now.AddDays(-30);
            DateTime? to = DateTime.Now;
            var consumptions = GetSampleConsumptions();
            _mockRepository.Setup(repo => repo.ConsumptionHistory(from, to)).Returns(consumptions);

            // Act
            var result = _service.ConsumptionHistory(from, to);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(10.0, result[0].Amount);
            Assert.Equal("Consumed", result[0].Note);
            _mockRepository.Verify(repo => repo.ConsumptionHistory(from, to), Times.Once());
        }

        [Fact]
        public void ConsumptionHistory_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            DateTime? from = DateTime.Now.AddDays(-30);
            DateTime? to = DateTime.Now;
            var emptyList = new List<MedicalSupplyConsumption>();
            _mockRepository.Setup(repo => repo.ConsumptionHistory(from, to)).Returns(emptyList);

            // Act
            var result = _service.ConsumptionHistory(from, to);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRepository.Verify(repo => repo.ConsumptionHistory(from, to), Times.Once());
        }

        [Fact]
        public void ConsumptionHistory_ReturnsNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            DateTime? from = DateTime.Now.AddDays(-30);
            DateTime? to = DateTime.Now;
            _mockRepository.Setup(repo => repo.ConsumptionHistory(from, to)).Returns((List<MedicalSupplyConsumption>)null);

            // Act
            var result = _service.ConsumptionHistory(from, to);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.ConsumptionHistory(from, to), Times.Once());
        }

        private List<MedicalSupplyConsumption> GetSampleConsumptions()
        {
            return new List<MedicalSupplyConsumption>
            {
                new MedicalSupplyConsumption
                {
                    MsconsumptionId = 1,
                    MedicalSupplyInventoryId = 1,
                    Amount = 10.0,
                    ConsumptionDate = DateTime.Now.AddDays(-10),
                    Status = true,
                    Note = "Consumed"
                },
                new MedicalSupplyConsumption
                {
                    MsconsumptionId = 2,
                    MedicalSupplyInventoryId = 1,
                    Amount = 20.0,
                    ConsumptionDate = DateTime.Now.AddDays(-5),
                    Status = true,
                    Note = "Consumed"
                }
            };
        }
    }
}