using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class ConsumptionDetailTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public ConsumptionDetailTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);
        }

        [Fact]
        public void ConsumptionDetail_ReturnsConsumptionList_WhenDataExists()
        {
            // Arrange
            int id = 1;
            DateTime? from = DateTime.Now.AddDays(-30);
            DateTime? to = DateTime.Now;
            var consumptions = GetSampleConsumptions();
            _mockRepository.Setup(repo => repo.MSConsumptionDetail(id, from, to)).Returns(consumptions);

            // Act
            var result = _service.ConsumptionDetail(id, from, to);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(10.0, result[0].Amount);
            Assert.Equal("Consumed", result[0].Note);
            _mockRepository.Verify(repo => repo.MSConsumptionDetail(id, from, to), Times.Once());
        }

        [Fact]
        public void ConsumptionDetail_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            int id = 999;
            DateTime? from = DateTime.Now.AddDays(-30);
            DateTime? to = DateTime.Now;
            var emptyList = new List<MedicalSupplyConsumption>();
            _mockRepository.Setup(repo => repo.MSConsumptionDetail(id, from, to)).Returns(emptyList);

            // Act
            var result = _service.ConsumptionDetail(id, from, to);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRepository.Verify(repo => repo.MSConsumptionDetail(id, from, to), Times.Once());
        }

        [Fact]
        public void ConsumptionDetail_ReturnsNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            int id = 999;
            DateTime? from = DateTime.Now.AddDays(-30);
            DateTime? to = DateTime.Now;
            _mockRepository.Setup(repo => repo.MSConsumptionDetail(id, from, to)).Returns((List<MedicalSupplyConsumption>)null);

            // Act
            var result = _service.ConsumptionDetail(id, from, to);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.MSConsumptionDetail(id, from, to), Times.Once());
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