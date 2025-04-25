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
        }

        [Fact]
        public void ConsumeReport_ReturnsDictionary_WhenReportDataExists()
        {
            // Arrange
            DateTime? from = DateTime.Now.AddDays(-30);
            DateTime? to = DateTime.Now;
            var reportData = GetSampleConsumeReport();
            _mockRepository.Setup(repo => repo.GetAllMedicalSupplyConsumeReport(from, to))
                           .Returns(reportData);
            _mockRepository.Setup(repo => repo.GetMSQantityByID(1)).Returns(50.0);
            _mockRepository.Setup(repo => repo.GetMSQantityByID(2)).Returns(100.0);

            // Act
            var result = _service.ConsumeReport(from, to);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            var firstEntry = result.First();
            Assert.Equal("Supply1", firstEntry.Key.MedicalSupplyName);
            Assert.Equal(50.0, firstEntry.Key.Quantity);
            Assert.Equal(20.0, firstEntry.Value);
            _mockRepository.Verify(repo => repo.GetAllMedicalSupplyConsumeReport(from, to), Times.Once());
            _mockRepository.Verify(repo => repo.GetMSQantityByID(It.IsAny<int>()), Times.Exactly(2));
        }

        [Fact]
        public void ConsumeReport_ReturnsEmptyDictionary_WhenReportDataIsEmpty()
        {
            // Arrange
            DateTime? from = DateTime.Now.AddDays(-30);
            DateTime? to = DateTime.Now;
            var emptyReport = new Dictionary<MedicalSupply, double>();
            _mockRepository.Setup(repo => repo.GetAllMedicalSupplyConsumeReport(from, to))
                           .Returns(emptyReport);

            // Act
            var result = _service.ConsumeReport(from, to);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRepository.Verify(repo => repo.GetAllMedicalSupplyConsumeReport(from, to), Times.Once());
            _mockRepository.Verify(repo => repo.GetMSQantityByID(It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void ConsumeReport_HandlesNullDateRange()
        {
            // Arrange
            DateTime? from = null;
            DateTime? to = null;
            var reportData = GetSampleConsumeReport();
            _mockRepository.Setup(repo => repo.GetAllMedicalSupplyConsumeReport(from, to))
                           .Returns(reportData);
            _mockRepository.Setup(repo => repo.GetMSQantityByID(1)).Returns(50.0);
            _mockRepository.Setup(repo => repo.GetMSQantityByID(2)).Returns(100.0);

            // Act
            var result = _service.ConsumeReport(from, to);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _mockRepository.Verify(repo => repo.GetAllMedicalSupplyConsumeReport(from, to), Times.Once());
            _mockRepository.Verify(repo => repo.GetMSQantityByID(It.IsAny<int>()), Times.Exactly(2));
        }

        private Dictionary<MedicalSupply, double> GetSampleConsumeReport()
        {
            return new Dictionary<MedicalSupply, double>
            {
                {
                    new MedicalSupply
                    {
                        MedicalSupplyId = 1,
                        MedicalSupplyName = "Supply1",
                        SupplyType = "Type1",
                        UnitOfMeasure = "Unit1",
                        SupplierId = 101,
                        Status = true,
                        ImportPrice = 10.0,
                        SellingPrice = 15.0,
                        BidNumber = 1001
                    },
                    20.0
                },
                {
                    new MedicalSupply
                    {
                        MedicalSupplyId = 2,
                        MedicalSupplyName = "Supply2",
                        SupplyType = "Type2",
                        UnitOfMeasure = "Unit2",
                        SupplierId = 102,
                        Status = true,
                        ImportPrice = 20.0,
                        SellingPrice = 30.0,
                        BidNumber = 1002
                    },
                    30.0
                }
            };
        }
    }
}