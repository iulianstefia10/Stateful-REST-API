using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stateful_REST_API.Controllers;
using Stateful_REST_API.Models.EntityFramework;


namespace Stateful_REST_API.Tests
{
    [TestClass]
    public class SeriesControllerTests
    {
        private SeriesDbContext _context = null!;
        private SeriesController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            var builder = new DbContextOptionsBuilder<SeriesDbContext>()
                .UseNpgsql("Server=localhost;port=5432;Database=SeriesDB;uid=postgres;password=1234;");

            _context = new SeriesDbContext(builder.Options);
            _controller = new SeriesController(_context);
        }

        [TestMethod]
        public async Task GetSeries_ReturnsAllSeries_Pass()
        {
            var result = await _controller.GetSeries();
            var actionResult = result.Value;

            Assert.IsNotNull(actionResult);

            var listSeriesRecuperees = actionResult.Where(s => s.Serieid <= 3).ToList();

            Assert.IsTrue(listSeriesRecuperees.Count == 3);
            Assert.AreEqual("Scrubs", listSeriesRecuperees[0].Titre);
        }

        [TestMethod]
        public async Task GetSerie_ValidId_ReturnsSerie_Pass()
        {
            var result = await _controller.GetSerie(1);

            Assert.IsNotNull(result.Value);
            Assert.AreEqual("Scrubs", result.Value.Titre);
        }

        [TestMethod]
        public async Task GetSerie_InvalidId_ReturnsNotFound_Fail()
        {
            var result = await _controller.GetSerie(9999);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteSerie_ValidId_ReturnsNoContent_Pass()
        {
            Serie serieTemporara = new Serie { Titre = "Serial de sters", Anneecreation = 2026 };
            _context.Series.Add(serieTemporara);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteSerie(serieTemporara.Serieid);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task DeleteSerie_InvalidId_ReturnsNotFound_Fail()
        {
            var result = await _controller.DeleteSerie(9999);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PostSerie_MissingTitle_ThrowsDbUpdateException_Fail()
        {
            Serie invalidSerie = new Serie
            {
                Resume = "Acesta este un test fara titlu",
                Nbsaisons = 1,
                Nbepisodes = 10,
                Anneecreation = 2026,
                Network = "TestNet"
            };

            await Assert.ThrowsExceptionAsync<DbUpdateException>(async () =>
            {
                await _controller.PostSerie(invalidSerie);
            });
        }
    }
}