using Microsoft.EntityFrameworkCore;
using Thunders.TechTest.Domain.Entities;
using Thunders.TechTest.Domain.Enums;
using Thunders.TechTest.Infrastructure.Data;
using Thunders.TechTest.Infrastructure.Repositories;

namespace Thunders.TechTest.Tests.Infrastructure.Repositories
{
    public class TestTollDataContext(DbContextOptions<TollDataContext> options) : TollDataContext(options)
    {
    }

    public class TollDataRepositoryTests
    {
        private static TollDataContext InMemoryContext
        {
            get
            {
                var options = new DbContextOptionsBuilder<TollDataContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
                return new TestTollDataContext(options);
            }
        }

        [Fact]
        public async Task AddTollUsageAsync_DeveAdicionarUsoCorretamente()
        {
            // Arrange
            using var context = InMemoryContext;
            var repository = new TollDataRepository(context);
            var usage = new TollUsage
            {
                UsageDateTime = DateTime.UtcNow,
                TollPlaza = "Plaza1",
                City = "City1",
                State = "State1",
                AmountPaid = 10m,
                VehicleType = VehicleType.Car
            };

            // Act
            await repository.AddTollUsageAsync(usage);
            await repository.SaveChangesAsync();

            var usages = await repository.GetAllTollUsagesAsync();

            // Assert
            Assert.Single(usages);
            Assert.Equal("Plaza1", usages.First().TollPlaza);
        }

        [Fact]
        public async Task GetAllTollUsagesAsync_DeveRetornarTodosOsUsos()
        {
            // Arrange
            using var context = InMemoryContext;
            var repository = new TollDataRepository(context);
            var usage1 = new TollUsage
            {
                UsageDateTime = DateTime.UtcNow,
                TollPlaza = "Plaza1",
                City = "City1",
                State = "State1",
                AmountPaid = 10m,
                VehicleType = VehicleType.Car
            };
            var usage2 = new TollUsage
            {
                UsageDateTime = DateTime.UtcNow,
                TollPlaza = "Plaza2",
                City = "City2",
                State = "State2",
                AmountPaid = 20m,
                VehicleType = VehicleType.Truck
            };

            await repository.AddTollUsageAsync(usage1);
            await repository.AddTollUsageAsync(usage2);
            await repository.SaveChangesAsync();

            // Act
            var usages = await repository.GetAllTollUsagesAsync();

            // Assert
            Assert.Equal(2, usages.Count);
        }

        [Fact]
        public async Task GetHourlyCityReportAsync_DeveRetornarRelatorioAgrupadoPorHoraECidade()
        {
            // Arrange
            using var context = InMemoryContext;
            var repository = new TollDataRepository(context);
            var now = DateTime.UtcNow;
            var usage1 = new TollUsage
            {
                UsageDateTime = now,
                TollPlaza = "Plaza1",
                City = "City1",
                State = "State1",
                AmountPaid = 10m,
                VehicleType = VehicleType.Car
            };

            var usage2 = new TollUsage
            {
                UsageDateTime = now.AddMinutes(10),
                TollPlaza = "Plaza1",
                City = "City1",
                State = "State1",
                AmountPaid = 15m,
                VehicleType = VehicleType.Truck
            };
            var usage3 = new TollUsage
            {
                UsageDateTime = now,
                TollPlaza = "Plaza2",
                City = "City2",
                State = "State2",
                AmountPaid = 20m,
                VehicleType = VehicleType.Car
            };

            await repository.AddTollUsageAsync(usage1);
            await repository.AddTollUsageAsync(usage2);
            await repository.AddTollUsageAsync(usage3);
            await repository.SaveChangesAsync();

            // Act
            var reports = await repository.GetHourlyCityReportAsync();

            // Assert
            Assert.Equal(2, reports.Count);

            var city1Report = reports.FirstOrDefault(r => r.City == "City1");
            Assert.NotNull(city1Report);
            Assert.Equal(25m, city1Report.TotalAmount);
        }

        [Fact]
        public async Task GetTopTollPlazasReportAsync_DeveRetornarAsPrincipaisPracasDePedagio()
        {
            // Arrange
            using var context = InMemoryContext;
            var repository = new TollDataRepository(context);
            var now = DateTime.UtcNow;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);

            var usage1 = new TollUsage
            {
                UsageDateTime = firstDayOfMonth.AddDays(1),
                TollPlaza = "Plaza1",
                City = "City1",
                State = "State1",
                AmountPaid = 30m,
                VehicleType = VehicleType.Car
            };
            var usage2 = new TollUsage
            {
                UsageDateTime = firstDayOfMonth.AddDays(2),
                TollPlaza = "Plaza2",
                City = "City2",
                State = "State2",
                AmountPaid = 50m,
                VehicleType = VehicleType.Truck
            };
            var usage3 = new TollUsage
            {
                UsageDateTime = firstDayOfMonth.AddDays(3),
                TollPlaza = "Plaza1",
                City = "City1",
                State = "State1",
                AmountPaid = 20m,
                VehicleType = VehicleType.Car
            };

            await repository.AddTollUsageAsync(usage1);
            await repository.AddTollUsageAsync(usage2);
            await repository.AddTollUsageAsync(usage3);
            await repository.SaveChangesAsync();

            // Act
            var topReports = await repository.GetTopTollPlazasReportAsync(1);

            // Assert
            Assert.Single(topReports);
            var report = topReports.First();
            Assert.Equal(50m, report.TotalAmount);
        }

        [Fact]
        public async Task GetTollUsageCountReportAsync_DeveRetornarContagemAgrupadaPorTipoDeVeiculo()
        {
            // Arrange
            using var context = InMemoryContext;
            var repository = new TollDataRepository(context);
            var tollPlaza = "Plaza1";
            var usage1 = new TollUsage
            {
                UsageDateTime = DateTime.UtcNow,
                TollPlaza = tollPlaza,
                City = "City1",
                State = "State1",
                AmountPaid = 10m,
                VehicleType = VehicleType.Car
            };
            var usage2 = new TollUsage
            {
                UsageDateTime = DateTime.UtcNow,
                TollPlaza = tollPlaza,
                City = "City1",
                State = "State1",
                AmountPaid = 15m,
                VehicleType = VehicleType.Truck
            };
            var usage3 = new TollUsage
            {
                UsageDateTime = DateTime.UtcNow,
                TollPlaza = tollPlaza,
                City = "City1",
                State = "State1",
                AmountPaid = 20m,
                VehicleType = VehicleType.Car
            };

            await repository.AddTollUsageAsync(usage1);
            await repository.AddTollUsageAsync(usage2);
            await repository.AddTollUsageAsync(usage3);
            await repository.SaveChangesAsync();

            // Act
            var countReports = await repository.GetTollUsageCountReportAsync(tollPlaza);

            // Assert
            var carReport = countReports.FirstOrDefault(r => r.VehicleType == VehicleType.Car);
            var truckReport = countReports.FirstOrDefault(r => r.VehicleType == VehicleType.Truck);

            Assert.NotNull(carReport);
            Assert.NotNull(truckReport);
            Assert.Equal(2, carReport.Count);
            Assert.Equal(1, truckReport.Count);
        }
    }
}
