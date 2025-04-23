using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Services.Technique;
using Microsoft.Extensions.Logging;
using Moq;

namespace ArtLink.Tests.Services;

public class TechniqueServiceTests
{
    [Fact]
    public async Task AddAsync_ShouldCallRepository()
    {
        var mock = new Mock<ITechniqueRepository>();
        var loggerMock =  new Mock<ILogger<TechniqueService>>();
        var service = new TechniqueService(mock.Object, loggerMock.Object);

        await service.AddTechniqueAsync("Digital", "Tablet drawing");

        mock.Verify(r => r.AddAsync("Digital", "Tablet drawing"), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldCallRepository()
    {
        var mock = new Mock<ITechniqueRepository>();
        var loggerMock =  new Mock<ILogger<TechniqueService>>();
        var service = new TechniqueService(mock.Object, loggerMock.Object);
        var id = Guid.NewGuid();

        await service.UpdateTechniqueAsync(id, "Updated", "Desc");

        mock.Verify(r => r.UpdateAsync(id, "Updated", "Desc"), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepository()
    {
        var mock = new Mock<ITechniqueRepository>();
        var loggerMock =  new Mock<ILogger<TechniqueService>>();
        var service = new TechniqueService(mock.Object, loggerMock.Object);
        var id = Guid.NewGuid();

        await service.DeleteTechniqueAsync(id);

        mock.Verify(r => r.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnFromRepository()
    {
        var mock = new Mock<ITechniqueRepository>();
        var expected = new List<Domain.Models.Technique>();

        if (expected == null)
        {
            throw new ArgumentNullException(nameof(expected));
        }

        mock.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

        var loggerMock =  new Mock<ILogger<TechniqueService>>();
        var service = new TechniqueService(mock.Object, loggerMock.Object);
        var result = await service.GetAllAsync();

        Assert.Equal(expected, result);
    }
}
