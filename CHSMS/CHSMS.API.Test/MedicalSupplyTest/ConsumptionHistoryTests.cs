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

            // Setup precondition: MedicalSupplyConsumption with MsconsumptionId: 1, ConsumptionDate: 4/4/2025
            var consumption = new MedicalSupplyConsumption
            {
                MsconsumptionId = 1,
                MedicalSupplyInventoryId = 1,
                Amount = 10,
                ConsumptionDate = new DateTime(2025, 4, 4),
                Status = true,
                Note = "Test consumption"
            };
            _mockRepository.Setup(r => r.ConsumptionHistory(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns((DateTime from, DateTime to) => new List<MedicalSupplyConsumption> { consumption }
                    .FindAll(c => c.ConsumptionDate >= from && c.ConsumptionDate <= to && c.Status == true));
        }

        [Fact]
        public void ConsumptionHistory_NullFromNullTo_ReturnsRecordsFromMinValueToNow()
        {
            // Arrange
            DateTime? from = null;
            DateTime? to = null;
            var expectedFrom = DateTime.MinValue;
            var expectedTo = DateTime.Now;

            // Act
            var result = _service.ConsumptionHistory(from, to);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, c => Assert.True(c.ConsumptionDate >= expectedFrom && c.ConsumptionDate <= expectedTo));
            Assert.All(result, c => Assert.True(c.Status));
            _mockRepository.Verify(r => r.ConsumptionHistory(expectedFrom, It.Is<DateTime>(t => t.Date == expectedTo.Date)), Times.Once());
        }

        [Fact]
        public void ConsumptionHistory_NullFromValidTo_ReturnsRecordsFromMinValueToSpecifiedTo()
        {
            // Arrange
            DateTime? from = null;
            DateTime? to = new DateTime(2025, 5, 5);
            var expectedFrom = DateTime.MinValue;

            // Act
            var result = _service.ConsumptionHistory(from, to);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, c => Assert.True(c.ConsumptionDate >= expectedFrom && c.ConsumptionDate <= to));
            Assert.All(result, c => Assert.True(c.Status));
            _mockRepository.Verify(r => r.ConsumptionHistory(expectedFrom, to.Value), Times.Once());
        }

        [Fact]
        public void ConsumptionHistory_ValidFromNullTo_ReturnsRecordsFromSpecifiedFromToNow()
        {
            // Arrange
            DateTime? from = new DateTime(2025, 3, 3);
            DateTime? to = null;
            var expectedTo = DateTime.Now;

            // Act
            var result = _service.ConsumptionHistory(from, to);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, c => Assert.True(c.ConsumptionDate >= from && c.ConsumptionDate <= expectedTo));
            Assert.All(result, c => Assert.True(c.Status));
            _mockRepository.Verify(r => r.ConsumptionHistory(from.Value, It.Is<DateTime>(t => t.Date == expectedTo.Date)), Times.Once());
        }

        [Fact]
        public void ConsumptionHistory_ValidFromValidTo_ReturnsRecordsInDateRange()
        {
            // Arrange
            DateTime? from = new DateTime(2025, 3, 3);
            DateTime? to = new DateTime(2025, 5, 5);

            // Act
            var result = _service.ConsumptionHistory(from, to);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, c => Assert.True(c.ConsumptionDate >= from && c.ConsumptionDate <= to));
            Assert.All(result, c => Assert.True(c.Status));
            _mockRepository.Verify(r => r.ConsumptionHistory(from.Value, to.Value), Times.Once());
        }
    }
}