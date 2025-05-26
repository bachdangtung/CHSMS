using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class ConsumeReportTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public ConsumeReportTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);

            // Setup precondition: MedicalSupplyConsumption with MsconsumptionId: 1, ConsumptionDate: 5/5/2025
            var medicalSupply = new MedicalSupply
            {
                MedicalSupplyId = 1,
                MedicalSupplyName = "Test Supply",
                SupplyType = "TypeA",
                UnitOfMeasure = "Unit",
                SupplierId = 1,
                Status = true,
                ImportPrice = 10.0,
                SellingPrice = 15.0,
                BidNumber = 123
            };
            var consumption = new MedicalSupplyConsumption
            {
                MsconsumptionId = 1,
                MedicalSupplyInventoryId = 1,
                Amount = 50.0,
                ConsumptionDate = new DateTime(2025, 5, 5),
                Status = true
            };
            var inventory = new MedicalSupplyInventory
            {
                SupplyInventoryId = 1,
                MedicalSupplyId = 1,
                Quantity = 100.0,
                MedicalSupply = medicalSupply
            };

            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryById(1))
                .Returns(inventory);
            _mockRepository.Setup(r => r.GetAllMedicalSupplies())
                .Returns(new List<MedicalSupply> { medicalSupply });
        }

        [Fact]
        public void ConsumeReport_NullFromNullTo_ReturnsConsumption()
        {
            // Arrange
            DateTime? from = null;
            DateTime? to = null;
            var consumptionDict = new Dictionary<MedicalSupply, double>
            {
                { new MedicalSupply { MedicalSupplyId = 1 }, 50.0 }
            };
            _mockRepository.Setup(r => r.GetAllMedicalSupplyConsumeReport(null, null))
                .Returns(consumptionDict);
            _mockRepository.Setup(r => r.GetMSQantityByID(1))
                .Returns(100.0);

            // Act
            var result = _service.ConsumeReport(from, to);

            // Assert
            Assert.Single(result);
            var entry = result.Keys.First();
            Assert.Equal(1, entry.MedicalSupplyId);
            Assert.Equal(100.0, entry.Quantity);
            Assert.Equal(50.0, result[entry]);
        }

        [Fact]
        public void ConsumeReport_NullFromToDate_ReturnsConsumption()
        {
            // Arrange
            DateTime? from = null;
            DateTime to = new DateTime(2025, 5, 10);
            var consumptionDict = new Dictionary<MedicalSupply, double>
            {
                { new MedicalSupply { MedicalSupplyId = 1 }, 50.0 }
            };
            _mockRepository.Setup(r => r.GetAllMedicalSupplyConsumeReport(null, to))
                .Returns(consumptionDict);
            _mockRepository.Setup(r => r.GetMSQantityByID(1))
                .Returns(100.0);

            // Act
            var result = _service.ConsumeReport(from, to);

            // Assert
            Assert.Single(result);
            var entry = result.Keys.First();
            Assert.Equal(1, entry.MedicalSupplyId);
            Assert.Equal(100.0, entry.Quantity);
            Assert.Equal(50.0, result[entry]);
        }

        [Fact]
        public void ConsumeReport_FromDateNullTo_ReturnsConsumption()
        {
            // Arrange
            DateTime from = new DateTime(2025, 5, 5);
            DateTime? to = null;
            var consumptionDict = new Dictionary<MedicalSupply, double>
            {
                { new MedicalSupply { MedicalSupplyId = 1 }, 50.0 }
            };
            _mockRepository.Setup(r => r.GetAllMedicalSupplyConsumeReport(from, null))
                .Returns(consumptionDict);
            _mockRepository.Setup(r => r.GetMSQantityByID(1))
                .Returns(100.0);

            // Act
            var result = _service.ConsumeReport(from, to);

            // Assert
            Assert.Single(result);
            var entry = result.Keys.First();
            Assert.Equal(1, entry.MedicalSupplyId);
            Assert.Equal(100.0, entry.Quantity);
            Assert.Equal(50.0, result[entry]);
        }

        [Fact]
        public void ConsumeReport_FromDateToDate_ReturnsConsumption()
        {
            // Arrange
            DateTime from = new DateTime(2025, 5, 5);
            DateTime to = new DateTime(2025, 5, 10);
            var consumptionDict = new Dictionary<MedicalSupply, double>
            {
                { new MedicalSupply { MedicalSupplyId = 1 }, 50.0 }
            };
            _mockRepository.Setup(r => r.GetAllMedicalSupplyConsumeReport(from, to))
                .Returns(consumptionDict);
            _mockRepository.Setup(r => r.GetMSQantityByID(1))
                .Returns(100.0);

            // Act
            var result = _service.ConsumeReport(from, to);

            // Assert
            Assert.Single(result);
            var entry = result.Keys.First();
            Assert.Equal(1, entry.MedicalSupplyId);
            Assert.Equal(100.0, entry.Quantity);
            Assert.Equal(50.0, result[entry]);
        }

        [Fact]
        public void ConsumeReport_FromAfterConsumptionDate_ReturnsEmpty()
        {
            // Arrange
            DateTime from = new DateTime(2025, 5, 10);
            DateTime to = new DateTime(2025, 5, 10);
            var consumptionDict = new Dictionary<MedicalSupply, double>();
            _mockRepository.Setup(r => r.GetAllMedicalSupplyConsumeReport(from, to))
                .Returns(consumptionDict);
            _mockRepository.Setup(r => r.GetMSQantityByID(1))
                .Returns(100.0);

            // Act
            var result = _service.ConsumeReport(from, to);

            // Assert
            Assert.Empty(result);
        }
    }
}