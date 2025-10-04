using ArtLink.Domain.Models;
using AutoFixture;

namespace ArtLink.Tests.Common.Fixtures;

public class EmployerFixture
{
    private readonly IFixture _fixture;

    public EmployerFixture()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    public Employer CreateEmployer(
        Guid? id = null,
        string? companyName = null,
        string? email = null,
        string? passwordHash = null,
        string? cpFirstName = null,
        string? cpLastName = null)
    {
        var employer = _fixture.Build<Employer>()
            .With(e => e.Id, id ?? Guid.NewGuid())
            .With(e => e.CompanyName, companyName ?? "TestCompany")
            .With(e => e.Email, email ?? "company@example.com")
            .With(e => e.PasswordHash, passwordHash ?? "hashed_password")
            .With(e => e.CpFirstName, cpFirstName ?? "Contact")
            .With(e => e.CpLastName, cpLastName ?? "Person")
            .Create();

        return employer;
    }

    public List<Employer> CreateEmployers(int count)
    {
        var employers = new List<Employer>();
        for (int i = 0; i < count; i++)
        {
            employers.Add(CreateEmployer(
                companyName: $"Company {i + 1}",
                email: $"company{i + 1}@example.com",
                cpFirstName: $"First{i + 1}",
                cpLastName: $"Last{i + 1}"));
        }
        return employers;
    }
}
