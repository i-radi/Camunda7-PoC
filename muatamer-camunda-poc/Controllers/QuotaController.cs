using Microsoft.AspNetCore.Mvc;
using muatamer_camunda_poc.Context;
using muatamer_camunda_poc.Models;

namespace muatamer_camunda_poc.Controllers;
[Route("api/[controller]")]
[ApiController]
public class QuotaController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public QuotaController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var quota = _dbContext.StandaloneQuotaTracking.ToList();
        return Ok(quota);
    }

    [HttpPost]
    public async Task<IActionResult> Add(StandaloneQuotaTracking quotaTracking)
    {
        _dbContext.Add(quotaTracking);
        _dbContext.SaveChanges();
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> update(StandaloneQuotaTracking quotaTracking)
    {
        var quota = _dbContext.StandaloneQuotaTracking.FirstOrDefault(q => q.Id == quotaTracking.Id);
        if (quota == null) return BadRequest();

        quota.Used = quotaTracking.Used;
        quota.Reserved = quotaTracking.Reserved;

        _dbContext.Update(quota);
        _dbContext.SaveChanges();

        return Ok();
    }
}
