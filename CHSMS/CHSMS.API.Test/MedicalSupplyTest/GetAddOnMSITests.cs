using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class GetAddOnMSITests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public GetAddOnMSITests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);

            // Setup precondition: MedicalSupplyInventory with MedicalSupplyInventoryId: 1, ImportQuantity: 100, TransactionDate: 5/5/2025
            _mockRepository.Setup(r => r.GetAddOnMSI(1, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Returns((int id, DateTime? from, DateTime? to) =>
                {
                    // Default behavior: sum ImportQuantity for matching MedicalSupplyId and date range
                    var inventories = new List<MedicalSupplyInventory>
                    {
                        new MedicalSupplyInventory
                        {
                            SupplyInventoryId = 1,
                            MedicalSupplyId = 1,
                            ImportQuantity = 100,
                            TransactionDate = new DateTime(2025, 5, 5)
                        }
                    };
                    from = from ?? DateTime.MinValue;
                    to = to ?? DateTime.Now;
                    return inventories
                        .Where(x => x.MedicalSupplyId == id && x.TransactionDate >= from && x.TransactionDate <= to)
                        .Sum(x => x.ImportQuantity ?? 0);
                });

            // Setup for invalid MedicalSupplyId
            _mockRepository.Setup(r => r.GetAddOnMSI(-1, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Returns(0.0);
        }

        [Fact]
        public void GetAddOnMSI_ValidIdWithNullDates_ReturnsCorrectSum()
        {
            // Arrange
            int id = 1;
            DateTime? from = null;
            DateTime? to = null;

            // Act
            var result = _service.GetAddOnMSI(id, from, to);

            // Assert
            Assert.Equal(100.0, result);
        }

        [Fact]
        public void GetAddOnMSI_InvalidIdWithNullDates_ReturnsZero()
        {
            // Arrange
            int id = -1;
            DateTime? from = null;
            DateTime? to = null;

            // Act
            var result = _service.GetAddOnMSI(id, from, to);

            // Assert
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void GetAddOnMSI_ValidIdWithFromDate_ReturnsCorrectSum()
        {
            // Arrange
            int id = 1;
            DateTime? from = new DateTime(2025, 3, 3);
            DateTime? to = null;

            // Act
            var result = _service.GetAddOnMSI(id, from, to);

            // Assert
            Assert.Equal(100.0, result); // TransactionDate (5/5/2025) is after 3/3/2025
        }

        [Fact]
        public void GetAddOnMSI_ValidIdWithToDateBeforeTransaction_ReturnsZero()
        {
            // Arrange
            int id = 1;
            DateTime? from = null;
            DateTime? to = new DateTime(2025, 4, 4);

            // Act
            var result = _service.GetAddOnMSI(id, from, to);

            // Assert
            Assert.Equal(0.0, result); // TransactionDate (5/5/2025) is after 4/4/2025
        }

        [Fact]
        public void GetAddOnMSI_ValidIdWithDateRangeIncludingTransaction_ReturnsCorrectSum()
        {
            // Arrange
            int id = 1;
            DateTime? from = new DateTime(2025, 3, 3);
            DateTime? to = new DateTime(2025, 5, 5);

            // Act
            var result = _service.GetAddOnMSI(id, from, to);

            // Assert
            Assert.Equal(100.0, result); // TransactionDate (5/5/2025) is within 3/3/2025 to 5/5/2025
        }
    }
}