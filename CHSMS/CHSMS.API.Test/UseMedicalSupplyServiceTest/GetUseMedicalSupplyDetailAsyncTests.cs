using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;

namespace CHSMS.API.Tests
{
    public class GetUseMedicalSupplyDetailAsyncTests
    {
        private readonly Mock<IUseMedicalSupplyRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly IUseMedicalSupplyService _service;

        public GetUseMedicalSupplyDetailAsyncTests()
        {
            var mocks = TestHelper.CreateMocks();
            _repositoryMock = mocks.Repository;
            _contextMock = mocks.Context;
            _service = new UseMedicalSupplyService(_repositoryMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task GetUseMedicalSupplyDetailAsync_ReturnsDetail()
        {
            // Arrangement
            var useMedicalSupply = TestHelper.CreateUseMedicalSupply(1, 1, 1);
            var umsmsc = new UseMedicalSuppliesMedicalSupplyConsumption
            {
                UseMedicalSupplieId = 1,
                MsconsumptionId = 1,
                TotalPrice = 50,
                Msconsumption = TestHelper.CreateMedicalSupplyConsumption(1, 1, 5)
            };
            umsmsc.Msconsumption.MedicalSupplyInventory = TestHelper.CreateMedicalSupplyInventory(1, 1, 20);
            umsmsc.Msconsumption.MedicalSupplyInventory.MedicalSupply = new MedicalSupply
            {
                MedicalSupplyId = 1,
                MedicalSupplyName = "Test Supply"
            };
            var umsmscList = new List<UseMedicalSuppliesMedicalSupplyConsumption> { umsmsc };

            _repositoryMock.Setup(r => r.GetUseMedicalSupplyDetailAsync(1)).ReturnsAsync(useMedicalSupply);
            _contextMock.Setup(c => c.UseMedicalSuppliesMedicalSupplyConsumptions)
                .Returns(MockDbSet(umsmscList));

            // Act
            var result = await _service.GetUseMedicalSupplyDetailAsync(1);

            // Assert
            Assert.Equal(1, result.UseMedicalSupplyId);
            Assert.Single(result.MedicalSupplyConsumptions);
            Assert.Equal(50m, result.TotalPrice);
            Assert.Equal("Test Supply", result.MedicalSupplyConsumptions[0].MedicalSupplyName);
        }

        [Fact]
        public async Task GetUseMedicalSupplyDetailAsync_NonExistent_ThrowsException()
        {
            // Arrangement
            _repositoryMock.Setup(r => r.GetUseMedicalSupplyDetailAsync(999)).ReturnsAsync((UseMedicalSupply)null);

            // Act & Assert
            var exception = await Assert.ThrowsAnyAsync<Exception>(() => _service.GetUseMedicalSupplyDetailAsync(999));
            Assert.Contains("Không tìm thấy đơn vật tư", exception.Message);
        }

        [Fact]
        public async Task GetUseMedicalSupplyDetailAsync_EmptyConsumptions_ReturnsEmptyList()
        {
            // Arrangement
            var useMedicalSupply = TestHelper.CreateUseMedicalSupply(1, 1, 1);
            var umsmscList = new List<UseMedicalSuppliesMedicalSupplyConsumption>();

            _repositoryMock.Setup(r => r.GetUseMedicalSupplyDetailAsync(1)).ReturnsAsync(useMedicalSupply);
            _contextMock.Setup(c => c.UseMedicalSuppliesMedicalSupplyConsumptions)
                .Returns(MockDbSet(umsmscList));

            // Act
            var result = await _service.GetUseMedicalSupplyDetailAsync(1);

            // Assert
            Assert.Equal(1, result.UseMedicalSupplyId);
            Assert.Empty(result.MedicalSupplyConsumptions);
            Assert.Equal(0m, result.TotalPrice);
        }

        [Fact]
        public async Task GetUseMedicalSupplyDetailAsync_NullTotalPrice_ReturnsZeroTotalPrice()
        {
            // Arrangement
            var useMedicalSupply = TestHelper.CreateUseMedicalSupply(1, 1, 1);
            var umsmsc = new UseMedicalSuppliesMedicalSupplyConsumption
            {
                UseMedicalSupplieId = 1,
                MsconsumptionId = 1,
                TotalPrice = null, // Null TotalPrice
                Msconsumption = TestHelper.CreateMedicalSupplyConsumption(1, 1, 5)
            };
            umsmsc.Msconsumption.MedicalSupplyInventory = TestHelper.CreateMedicalSupplyInventory(1, 1, 20);
            umsmsc.Msconsumption.MedicalSupplyInventory.MedicalSupply = new MedicalSupply
            {
                MedicalSupplyId = 1,
                MedicalSupplyName = "Test Supply"
            };
            var umsmscList = new List<UseMedicalSuppliesMedicalSupplyConsumption> { umsmsc };

            _repositoryMock.Setup(r => r.GetUseMedicalSupplyDetailAsync(1)).ReturnsAsync(useMedicalSupply);
            _contextMock.Setup(c => c.UseMedicalSuppliesMedicalSupplyConsumptions)
                .Returns(MockDbSet(umsmscList));

            // Act
            var result = await _service.GetUseMedicalSupplyDetailAsync(1);

            // Assert
            Assert.Equal(1, result.UseMedicalSupplyId);
            Assert.Single(result.MedicalSupplyConsumptions);
            Assert.Equal(0m, result.TotalPrice); // Null TotalPrice maps to 0m
        }

        private static DbSet<T> MockDbSet<T>(List<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var dbSetMock = new Mock<DbSet<T>>();

            // Setup IQueryable with async provider
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new AsyncQueryProvider<T>(queryable.Provider));
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            return dbSetMock.Object;
        }

        // Helper class to support async queries
        private class AsyncQueryProvider<T> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            public AsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new AsyncQueryable<T>(expression, _inner);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new AsyncQueryable<TElement>(expression, _inner);
            }

            public object Execute(Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }

            public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute(expression));
            }

            public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute<TResult>(expression));
            }

            TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        private class AsyncQueryable<T> : IQueryable<T>, IAsyncEnumerable<T>
        {
            private readonly IQueryable<T> _inner;

            public AsyncQueryable(Expression expression, IQueryProvider provider)
            {
                _inner = provider.CreateQuery<T>(expression);
            }

            public Type ElementType => _inner.ElementType;
            public Expression Expression => _inner.Expression;
            public IQueryProvider Provider => _inner.Provider;

            public IEnumerator<T> GetEnumerator() => _inner.GetEnumerator();
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new AsyncEnumerator<T>(_inner.ToList());
            }
        }

        private class AsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly List<T> _data;
            private int _index = -1;

            public AsyncEnumerator(List<T> data)
            {
                _data = data;
            }

            public T Current => _data[_index];

            public ValueTask DisposeAsync() => default;

            public ValueTask<bool> MoveNextAsync()
            {
                _index++;
                return new ValueTask<bool>(_index < _data.Count);
            }
        }
    }
}