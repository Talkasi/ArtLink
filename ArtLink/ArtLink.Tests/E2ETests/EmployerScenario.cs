using ArtLink.Services.Employer;
using ArtLink.Services.Contract;
using ArtLink.Services.Artist;
using ArtLink.Services.Portfolio;
using ArtLink.Services.Technique;
using ArtLink.Tests.Common.Database;
using ArtLink.Tests.Common.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.E2ETests;

[Collection("Database collection")]
public class EmployerScenarioE2ETest(DatabaseFixture db)
{
    private readonly EmployerFixture _employerFixture = new EmployerFixture();
    private readonly ContractFixture _contractFixture = new ContractFixture();
    private readonly ArtistFixture _artistFixture = new ArtistFixture();
    private readonly TechniqueFixture _techniqueFixture = new TechniqueFixture();
    private readonly PortfolioFixture _portfolioFixture = new PortfolioFixture();

    [Fact]
    public async Task DemoScenario_FullFlow()
    {
        var options = new DbContextOptionsBuilder<DataAccess.Context.ArtLinkDbContext>()
            .UseNpgsql(db.ConnectionString)
            .Options;

        var context = new DataAccess.Context.ArtLinkDbContext(options);

        var employerService = new EmployerService(
            new DataAccess.Repositories.EmployerRepository(context, new NullLogger<DataAccess.Repositories.EmployerRepository>()),
            new NullLogger<EmployerService>());

        var artistService = new ArtistService(
            new DataAccess.Repositories.ArtistRepository(context, new NullLogger<DataAccess.Repositories.ArtistRepository>()),
            new NullLogger<ArtistService>());

        var techniqueService = new TechniqueService(
            new DataAccess.Repositories.TechniqueRepository(context, new NullLogger<DataAccess.Repositories.TechniqueRepository>()),
            new NullLogger<TechniqueService>());

        var portfolioService = new PortfolioService(
            new DataAccess.Repositories.PortfolioRepository(context, new NullLogger<DataAccess.Repositories.PortfolioRepository>()),
            new NullLogger<PortfolioService>());

        var contractService = new ContractService(
            new DataAccess.Repositories.ContractRepository(context, new NullLogger<DataAccess.Repositories.ContractRepository>()),
            new NullLogger<ContractService>());

        var employer = _employerFixture.CreateEmployer();
        var employerId = await employerService.AddEmployerAsync(
            employer.CompanyName, employer.Email, employer.PasswordHash, employer.CpFirstName, employer.CpLastName);

        var artist = _artistFixture.CreateArtist();
        var artistId = await artistService.AddArtistAsync(
            artist.FirstName, artist.LastName, artist.Email, artist.PasswordHash!, artist.Bio, artist.ProfilePicturePath, artist.Experience);

        var technique = _techniqueFixture.CreateTechnique();
        var techniqueId = await techniqueService.AddTechniqueAsync(technique.Name, technique.Description);

        var portfolio = _portfolioFixture.CreatePortfolio(artistId: artistId, techniqueId: techniqueId);
        var portfolioId = await portfolioService.AddPortfolioAsync(portfolio.ArtistId, portfolio.Title, portfolio.TechniqueId, portfolio.Description);

        var contract = _contractFixture.CreateContract(artistId: artistId, employerId: employerId);
        var contractId = await contractService.AddContractAsync(
            contract.ArtistId,
            contract.EmployerId,
            contract.ProjectDescription,
            contract.Status,
            contract.StartDate,
            contract.EndDate);

        var savedContractsForArtist = await contractService.GetAllByArtistIdAsync(artistId);
        savedContractsForArtist.Should().ContainSingle(c => c.Id == contractId);

        var savedContractsForEmployer = await contractService.GetAllByEmployerIdAsync(employerId);
        savedContractsForEmployer.Should().ContainSingle(c => c.Id == contractId);

        await contractService.DeleteContractAsync(contractId);
        await portfolioService.DeletePortfolioAsync(portfolioId);
        await artistService.DeleteArtistAsync(artistId);
        await employerService.DeleteEmployerAsync(employerId);
        await techniqueService.DeleteTechniqueAsync(techniqueId);

        (await contractService.GetAllByArtistIdAsync(artistId)).Should().BeEmpty();
        (await contractService.GetAllByEmployerIdAsync(employerId)).Should().BeEmpty();
    }
}
