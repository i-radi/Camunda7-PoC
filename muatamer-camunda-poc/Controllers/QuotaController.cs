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
        var quota = _dbContext.TotalQuotaTracking.ToList();
        return Ok(quota);
    }

    [HttpPost]
    public async Task<IActionResult> Add(TotalQuotaTracking totalQuotaTracking)
    {
        _dbContext.Add(totalQuotaTracking);
        _dbContext.SaveChanges();
        return Ok();
    }
}
