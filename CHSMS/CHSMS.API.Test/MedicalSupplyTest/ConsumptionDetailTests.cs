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

            // Setup precondition: MedicalSupplyConsumption with MsconsumptionId: 1, ConsumptionDate: 4/4/2025 exists
            var consumption = new MedicalSupplyConsumption
            {
                MsconsumptionId = 1,
                MedicalSupplyInventoryId = 1,
                ConsumptionDate = new DateTime(2025, 4, 4),
                Amount = 10,
                Status = true
            };
            _mockRepository.Setup(r => r.MSConsumptionDetail(1, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Returns(new List<MedicalSupplyConsumption> { consumption });
        }

        [Fact]
        public void ConsumptionDetail_ValidId_NullFrom_NullTo_ReturnsConsumptionList()
        {
            // Arrange
            int id = 1;
            DateTime? from = null;
            DateTime? to = null;

            // Act
            var result = _service.ConsumptionDetail(id, from, to);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result[0].MsconsumptionId);
            Assert.Equal(new DateTime(2025, 4, 4), result[0].ConsumptionDate);
            _mockRepository.Verify(r => r.MSConsumptionDetail(id, from, to), Times.Once());
        }

        [Fact]
        public void ConsumptionDetail_ValidId_NullFrom_ValidTo_ReturnsConsumptionList()
        {
            // Arrange
            int id = 1;
            DateTime? from = null;
            DateTime? to = new DateTime(2025, 5, 5);

            // Act
            var result = _service.ConsumptionDetail(id, from, to);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result[0].MsconsumptionId);
            Assert.Equal(new DateTime(2025, 4, 4), result[0].ConsumptionDate);
            _mockRepository.Verify(r => r.MSConsumptionDetail(id, from, to), Times.Once());
        }

        [Fact]
        public void ConsumptionDetail_ValidId_ValidFrom_NullTo_ReturnsConsumptionList()
        {
            // Arrange
            int id = 1;
            DateTime? from = new DateTime(2025, 3, 3);
            DateTime? to = null;

            // Act
            var result = _service.ConsumptionDetail(id, from, to);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result[0].MsconsumptionId);
            Assert.Equal(new DateTime(2025, 4, 4), result[0].ConsumptionDate);
            _mockRepository.Verify(r => r.MSConsumptionDetail(id, from, to), Times.Once());
        }

        [Fact]
        public void ConsumptionDetail_ValidId_ValidFrom_ValidTo_ReturnsConsumptionList()
        {
            // Arrange
            int id = 1;
            DateTime? from = new DateTime(2025, 3, 3);
            DateTime? to = new DateTime(2025, 5, 5);

            // Act
            var result = _service.ConsumptionDetail(id, from, to);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result[0].MsconsumptionId);
            Assert.Equal(new DateTime(2025, 4, 4), result[0].ConsumptionDate);
            _mockRepository.Verify(r => r.MSConsumptionDetail(id, from, to), Times.Once());
        }

        [Fact]
        public void ConsumptionDetail_InvalidId_NullFrom_ValidTo_ReturnsEmptyList()
        {
            // Arrange
            int id = -1;
            DateTime? from = null;
            DateTime? to = new DateTime(2025, 5, 5);

            _mockRepository.Setup(r => r.MSConsumptionDetail(-1, from, to))
                .Returns(new List<MedicalSupplyConsumption>());

            // Act
            var result = _service.ConsumptionDetail(id, from, to);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRepository.Verify(r => r.MSConsumptionDetail(id, from, to), Times.Once());
        }
    }
}