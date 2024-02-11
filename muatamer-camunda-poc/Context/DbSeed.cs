using Bogus;
using Microsoft.EntityFrameworkCore;
using muatamer_camunda_poc.Models;

namespace muatamer_camunda_poc.Context;

public class DbSeed
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public DbSeed(ApplicationDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task SeedData()
    {
        _dbContext.Database.EnsureCreated();

        if (_configuration["cleanupDb"] == "Yes")
        {
            await CleanUp();
        }

        //if (!_dbContext.Countries.Any())
        //{

        //}
       

        //if (_dbContext.UmrahGroups.Any())
        //    return;

        //var groups = new List<UmrahGroup>
        //{
        //    new UmrahGroup
        //    {
        //        Name = "Group1",
        //        FromCountry = "Egypt",
        //        Notes = "test informations",
        //        HasVoucher = true,
        //        IsActive = true,
        //        VisaIssued = false
        //    },
        //    new UmrahGroup
        //    {
        //        Name = "Group2",
        //        FromCountry = "UAE",
        //        Notes = "test informations2",
        //        HasVoucher = true,
        //        IsActive = true,
        //        VisaIssued = false
        //    },
        //    new UmrahGroup
        //    {
        //        Name = "Group3",
        //        FromCountry = "Morocco",
        //        Notes = "test informations3",
        //        HasVoucher = true,
        //        IsActive = true,
        //        VisaIssued = false
        //    },
        //    new UmrahGroup
        //    {
        //        Name = "Group4",
        //        FromCountry = "Qatar",
        //        Notes = "test informations4",
        //        HasVoucher = true,
        //        IsActive = true,
        //        VisaIssued = false
        //    },
        //};
        //await _dbContext.AddRangeAsync(groups);
        //await _dbContext.SaveChangesAsync();


        //var muatamerFaker = new Faker<MuatamerInformation>()
        //    .RuleFor(m => m.Name, f => f.Name.FullName())
        //    //.RuleFor(m => m.NationalityId, f => f.Random.AlphaNumeric(10))
        //    .RuleFor(m => m.PassportType, f => f.PickRandom(new[] { "Regular", "Diplomatic", "Service", "Official" }))
        //    .RuleFor(m => m.PassportNumber, f => f.Random.Replace("??????????"))
        //    .RuleFor(m => m.PassportIssueDate, f => f.Date.Past(10))
        //    .RuleFor(m => m.PassportExpiryDate, f => f.Date.Future(10))
        //    //.RuleFor(m => m.CountryName, f => f.PickRandom(new[] { "Egypt", "UAE", "Qatar", "Morocco" }))
        //    .RuleFor(m => m.Gender, f => f.PickRandom(new[] { "Male", "Female" }))
        //    .RuleFor(m => m.GroupId, f => f.Random.Int(1, 4));

        //var muatamers = new List<MuatamerInformation>();
        //for (int i = 0; i < 10; i++)
        //{
        //    muatamers.Add(muatamerFaker.Generate());
        //}
        //await _dbContext.AddRangeAsync(muatamers);
        //await _dbContext.SaveChangesAsync();
    }

    private async Task CleanUp()
    {
        _dbContext.Database.ExecuteSqlRaw("DELETE FROM \"MuatamerInformations\"");
        _dbContext.Database.ExecuteSqlRaw("DELETE FROM \"UmrahGroups\"");

        await _dbContext.SaveChangesAsync();
    }
}
