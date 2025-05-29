using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;

namespace CHSMS.API.Test.ExternalPrescriptionTest
{
    public class GetExternalPrescriptionDetailAsyncTests : IDisposable
    {
        private readonly Mock<IExternalPrescriptionRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _dbContextMock;
        private readonly ExternalPrescriptionService _service;

        public GetExternalPrescriptionDetailAsyncTests()
        {
            _repositoryMock = new Mock<IExternalPrescriptionRepository>();
            _dbContextMock = new Mock<SEP_TestContext>(new DbContextOptions<SEP_TestContext>());

            _service = new ExternalPrescriptionService(_repositoryMock.Object, _dbContextMock.Object);
        }

        // Helper Methods
        private ExternalPrescription CreateDefaultPrescription(int externalPrescriptionId = 1)
        {
            return new ExternalPrescription
            {
                ExternalPrescriptionId = externalPrescriptionId,
                MedicalRecordHistoryId = 1,
                UserId = 1,
                IssueDate = DateTime.Now.Date,
                Status = true,
                Note = "Test prescription",
                IsBhyt = false,
                User = new User { Fullname = "Dr. Smith" },
                MedicalRecordHistory = new MedicalRecordHistory
                {
                    MedicalRecord = new MedicalRecord
                    {
                        PatientName = "John Doe",
                        Gender = "Male",
                        Dob = new DateTime(1990, 1, 1),
                        Address = "123 Main St",
                        HealthInsurance = "HI123456"
                    },
                    DiagnoseConclusion = "Flu"
                }
            };
        }

        private List<MedicinePrescription> CreateDefaultMedicinePrescriptions(int externalPrescriptionId = 1)
        {
            return new List<MedicinePrescription>
            {
                new MedicinePrescription
                {
                    ExternalPrescriptionId = externalPrescriptionId,
                    MedicineId = 1,
                    Amount = 10,
                    Note = "Take daily",
                    Medicine = new Medicine
                    {
                        MedicineId = 1,
                        MedicineName = "Medicine A",
                        DosageForm = "Tablet",
                        IsBhyt = false
                    }
                },
                new MedicinePrescription
                {
                    ExternalPrescriptionId = externalPrescriptionId,
                    MedicineId = 2,
                    Amount = 5,
                    Note = "Take twice daily",
                    Medicine = new Medicine
                    {
                        MedicineId = 2,
                        MedicineName = "Medicine B",
                        DosageForm = "Capsule",
                        IsBhyt = true
                    }
                }
            };
        }

        private void SetupMedicinePrescriptionsDbSet(List<MedicinePrescription> medicinePrescriptions)
        {
            var queryable = medicinePrescriptions.AsQueryable();
            var dbSetMock = new Mock<DbSet<MedicinePrescription>>();

            // Setup IQueryable
            dbSetMock.As<IQueryable<MedicinePrescription>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<MedicinePrescription>(queryable.Provider));
            dbSetMock.As<IQueryable<MedicinePrescription>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSetMock.As<IQueryable<MedicinePrescription>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSetMock.As<IQueryable<MedicinePrescription>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            // Setup IAsyncEnumerable
            dbSetMock.As<IAsyncEnumerable<MedicinePrescription>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<MedicinePrescription>(queryable.GetEnumerator()));

            _dbContextMock.Setup(db => db.MedicinePrescriptions).Returns(dbSetMock.Object);
        }
        // Helper class for async enumeration
        private class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            internal TestAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }

            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
            {
                var expectedResultType = typeof(TResult).GetGenericArguments()[0];
                var executionResult = typeof(IQueryProvider)
                    .GetMethod(
                        name: nameof(IQueryProvider.Execute),
                        genericParameterCount: 1,
                        types: new[] { typeof(Expression) })
                    .MakeGenericMethod(expectedResultType)
                    .Invoke(this, new[] { expression });

                return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                    .MakeGenericMethod(expectedResultType)
                    .Invoke(null, new[] { executionResult });
            }
        }

        private class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable)
                : base(enumerable)
            { }

            public TestAsyncEnumerable(Expression expression)
                : base(expression)
            { }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }

            IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
        }
        [Fact]
        public async Task GetExternalPrescriptionDetailAsync_ValidId_ReturnsPrescriptionDetailDTO()
        {
            // Arrange
            int externalPrescriptionId = 1;
            var prescription = CreateDefaultPrescription(externalPrescriptionId);
            var medicinePrescriptions = CreateDefaultMedicinePrescriptions(externalPrescriptionId);

            _repositoryMock.Setup(r => r.GetExternalPrescriptionDetailAsync(externalPrescriptionId))
                .ReturnsAsync(prescription);
            SetupMedicinePrescriptionsDbSet(medicinePrescriptions);

            // Act
            var result = await _service.GetExternalPrescriptionDetailAsync(externalPrescriptionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(externalPrescriptionId, result.ExternalPrescriptionId);
            Assert.Equal(prescription.IssueDate, result.IssueDate);
            Assert.True(result.Status);
            Assert.Equal("Test prescription", result.Note);
            Assert.Equal("Dr. Smith", result.FullName);
            Assert.Equal("John Doe", result.PatientName);
            Assert.Equal("Male", result.Gender);
            Assert.Equal(new DateTime(1990, 1, 1), result.Dob);
            Assert.Equal("123 Main St", result.Address);
            Assert.Equal("HI123456", result.HealthInsurance);
            Assert.Equal("Flu", result.DiagnoseConclusion);
            Assert.False(result.IsBhyt);

            Assert.Equal(2, result.Medicines.Count);
            var firstMedicine = result.Medicines[0];
            Assert.Equal(1, firstMedicine.MedicineId);
            Assert.Equal("Medicine A", firstMedicine.MedicineName);
            Assert.Equal("Tablet", firstMedicine.DosageForm);
            Assert.Equal(10, firstMedicine.Amount);
            Assert.Equal("Take daily", firstMedicine.Note);
            Assert.False(firstMedicine.IsBhyt);

            var secondMedicine = result.Medicines[1];
            Assert.Equal(2, secondMedicine.MedicineId);
            Assert.Equal("Medicine B", secondMedicine.MedicineName);
            Assert.Equal("Capsule", secondMedicine.DosageForm);
            Assert.Equal(5, secondMedicine.Amount);
            Assert.Equal("Take twice daily", secondMedicine.Note);
            Assert.True(secondMedicine.IsBhyt);

            _repositoryMock.Verify(r => r.GetExternalPrescriptionDetailAsync(externalPrescriptionId), Times.Once());
            _dbContextMock.Verify(db => db.MedicinePrescriptions, Times.AtLeastOnce());
        }

        [Fact]
        public async Task GetExternalPrescriptionDetailAsync_InvalidId_ThrowsArgumentException()
        {
            // Arrange
            int externalPrescriptionId = 0;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.GetExternalPrescriptionDetailAsync(externalPrescriptionId));
            Assert.Equal("ExternalPrescriptionId không hợp lệ.", exception.Message);
            _repositoryMock.Verify(r => r.GetExternalPrescriptionDetailAsync(It.IsAny<int>()), Times.Never());
            _dbContextMock.Verify(db => db.MedicinePrescriptions, Times.Never());
        }

        [Fact]
        public async Task GetExternalPrescriptionDetailAsync_NonExistentPrescription_ThrowsException()
        {
            // Arrange
            int externalPrescriptionId = 1;
            _repositoryMock.Setup(r => r.GetExternalPrescriptionDetailAsync(externalPrescriptionId))
                .ReturnsAsync((ExternalPrescription)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.GetExternalPrescriptionDetailAsync(externalPrescriptionId));
            Assert.Equal("Không tìm thấy đơn thuốc ngoài", exception.Message);
            _repositoryMock.Verify(r => r.GetExternalPrescriptionDetailAsync(externalPrescriptionId), Times.Once());
            _dbContextMock.Verify(db => db.MedicinePrescriptions, Times.Never());
        }

        [Fact]
        public async Task GetExternalPrescriptionDetailAsync_NoMedicinePrescriptions_ReturnsEmptyMedicinesList()
        {
            // Arrange
            int externalPrescriptionId = 1;
            var prescription = CreateDefaultPrescription(externalPrescriptionId);
            _repositoryMock.Setup(r => r.GetExternalPrescriptionDetailAsync(externalPrescriptionId))
                .ReturnsAsync(prescription);
            SetupMedicinePrescriptionsDbSet(new List<MedicinePrescription>()); // Empty list

            // Act
            var result = await _service.GetExternalPrescriptionDetailAsync(externalPrescriptionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(externalPrescriptionId, result.ExternalPrescriptionId);
            Assert.Empty(result.Medicines);
            _repositoryMock.Verify(r => r.GetExternalPrescriptionDetailAsync(externalPrescriptionId), Times.Once());
            _dbContextMock.Verify(db => db.MedicinePrescriptions, Times.AtLeastOnce());
        }

        private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public TestAsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public T Current => _inner.Current;

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return ValueTask.CompletedTask;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return ValueTask.FromResult(_inner.MoveNext());
            }
        }
        public void Dispose()
        {
        }
    }
}