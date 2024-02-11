using Bogus;
using Microsoft.EntityFrameworkCore;
using muatamer_camunda_poc.Models;
using muatamer_camunda_poc.Enum;
using System.Globalization;

namespace muatamer_camunda_poc.Context;

public class DbSeed
{
    private readonly ApplicationDbContext _dbContext;

    public DbSeed(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedData()
    {
        _dbContext.Database.EnsureCreated();

        if (_dbContext.Countries.Any())
            return;

        var countries = new List<Country>
        {
            new() {Name = "Egypt",State = Enum.State.Active},
            new() {Name = "UAE",State = Enum.State.InActive},
            new() {Name = "Morocco",State = Enum.State.Active},
            new() {Name = "Qatar",State = Enum.State.Active},
        };
        await _dbContext.AddRangeAsync(countries);
        await _dbContext.SaveChangesAsync();

        var nationalities = new List<Nationality>
        {
            new() {Name="Egyption",State = Enum.State.Active},
            new() {Name="UAE",State = Enum.State.InActive},
            new() {Name="Morocco",State = Enum.State.Active},
            new() {Name="Qatar",State = Enum.State.Active},
        };
        await _dbContext.AddRangeAsync(nationalities);
        await _dbContext.SaveChangesAsync();

        var externalAgents = new List<ExternalAgent>
        {
            new() {Name = "Ea 1", CountryId=1,Email="Ea1@mail.com",State = State.Active},
            new() {Name = "Ea 2", CountryId=1,Email="Ea2@mail.com",State = State.Active},
            new() {Name = "Ea 3", CountryId=1,Email="Ea3@mail.com",State = State.Active},
        };
        await _dbContext.AddRangeAsync(externalAgents);
        await _dbContext.SaveChangesAsync();

        var umrahOperators = new List<UmrahOperator>
        {
            new(){Name = "Uo 1", Email = "Uo1@mail.com", State = State.Active},
            new(){Name = "Uo 2", Email = "Uo2@mail.com", State = State.Active},
            new(){Name = "Uo 3", Email = "Uo3@mail.com", State = State.Active}
        };
        await _dbContext.AddRangeAsync(umrahOperators);
        await _dbContext.SaveChangesAsync();

        var groups = new List<UmrahGroup>
        {
            new UmrahGroup
            {
                Name = "Group1",
                FromCountry = "Egypt",
                Notes = "test informations",
                HasVoucher = true,
                IsActive = true,
                VisaIssued = false,
                CountryId = 1,
                ExternalAgentId = 1,
                UmrahOperatorId = 1
            },
            new UmrahGroup
            {
                Name = "Group2",
                FromCountry = "UAE",
                Notes = "test informations2",
                HasVoucher = true,
                IsActive = true,
                VisaIssued = false,
                CountryId = 2,
                ExternalAgentId = 1,
                UmrahOperatorId = 1
            },
            new UmrahGroup
            {
                Name = "Group3",
                FromCountry = "Morocco",
                Notes = "test informations3",
                HasVoucher = true,
                IsActive = true,
                VisaIssued = false,
                CountryId = 3,
                ExternalAgentId = 1,
                UmrahOperatorId = 1
            },
            new UmrahGroup
            {
                Name = "Group4",
                FromCountry = "Qatar",
                Notes = "test informations4",
                HasVoucher = true,
                IsActive = true,
                VisaIssued = false,
                CountryId = 4,
                ExternalAgentId = 1,
                UmrahOperatorId = 1
            },
        };
        await _dbContext.AddRangeAsync(groups);
        await _dbContext.SaveChangesAsync();

        var muatamerFaker = new Faker<MuatamerInformation>()
            .RuleFor(m => m.Name, f => f.Name.FullName())
            .RuleFor(m => m.NationalityId, 1)
            .RuleFor(m => m.PassportType, f => f.PickRandom(new[] { "Regular", "Diplomatic", "Service", "Official" }))
            .RuleFor(m => m.PassportNumber, f => f.Random.Replace("??????????"))
            .RuleFor(m => m.PassportIssueDate, f => f.Date.Past(10))
            .RuleFor(m => m.PassportExpiryDate, f => f.Date.Future(10))
            .RuleFor(m => m.CountryId, 1)
            .RuleFor(m => m.Gender, f => f.PickRandom(new[] { "Male", "Female" }))
            .RuleFor(m => m.GroupId, 1);

        var muatamers = new List<MuatamerInformation>();
        for (int i = 0; i < 20; i++)
        {
            muatamers.Add(muatamerFaker.Generate());
        }
        await _dbContext.AddRangeAsync(muatamers);
        await _dbContext.SaveChangesAsync();

        var standaloneQuota = new List<StandaloneQuotaTracking>
        {
            new() {Total = 1_000, Used = 0, Reserved = 0, EntityType = QuotaType.ExternalAgent, EntityId = 1, PeriodType = PeriodType.Daily, CreatedDate = DateTime.Now},
            new() {Total = 1_000_000, Used = 0, Reserved = 0, EntityType = QuotaType.ExternalAgent, EntityId = 1, PeriodType = PeriodType.Annually, CreatedDate = DateTime.Now},
            new() {Total = 1_000, Used = 0, Reserved = 0, EntityType = QuotaType.UmrahOperator, EntityId = 1, PeriodType = PeriodType.Daily, CreatedDate = DateTime.Now},
            new() {Total = 1_000_000, Used = 0, Reserved = 0, EntityType = QuotaType.UmrahOperator, EntityId = 1, PeriodType = PeriodType.Annually, CreatedDate = DateTime.Now},
            new() {Total = 1_000, Used = 0, Reserved = 0, EntityType = QuotaType.Country, EntityId = 1, PeriodType = PeriodType.Daily, CreatedDate = DateTime.Now},
            new() {Total = 1_000_000, Used = 0, Reserved = 0, EntityType = QuotaType.Country, EntityId = 1, PeriodType = PeriodType.Annually, CreatedDate = DateTime.Now},
            new() {Total = 1_000, Used = 0, Reserved = 0, EntityType = QuotaType.Nationality, EntityId = 1, PeriodType = PeriodType.Daily, CreatedDate = DateTime.Now},
            new() {Total = 1_000_000, Used = 0, Reserved = 0, EntityType = QuotaType.Nationality, EntityId = 1, PeriodType = PeriodType.Annually, CreatedDate = DateTime.Now},
        };
        await _dbContext.AddRangeAsync(standaloneQuota);
        await _dbContext.SaveChangesAsync();

        var intersectionQuota = new List<IntersectionQuotaTracking>
        {
            new() 
            {
                Total = 1_000,
                Used = 0,
                Reserved = 0,
                Entity1Type = QuotaType.ExternalAgent,
                Entity1Id = 1,
                Entity2Type = QuotaType.UmrahOperator,
                Entity2Id = 1,
                PeriodType = PeriodType.Daily,
                CreatedDate = DateTime.Now
            },
            new()
            {
                Total = 1_000_000,
                Used = 0,
                Reserved = 0,
                Entity1Type = QuotaType.ExternalAgent,
                Entity1Id = 1,
                Entity2Type = QuotaType.UmrahOperator,
                Entity2Id = 1,
                PeriodType = PeriodType.Annually,
                CreatedDate = DateTime.Now
            }};
        await _dbContext.AddRangeAsync(intersectionQuota);
        await _dbContext.SaveChangesAsync();

        var ExternalAgentUmrahOperators = new List<ExternalAgentUmrahOperator>
        {
            new (){ExternalAgentId=1, UmrahOperatorId =1},
            new (){ExternalAgentId=2, UmrahOperatorId =1},
            new (){ExternalAgentId=3, UmrahOperatorId =1},
        };
        _dbContext.UpdateRange(ExternalAgentUmrahOperators);
        await _dbContext.SaveChangesAsync();
    }
}
