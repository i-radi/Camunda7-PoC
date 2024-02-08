using Microsoft.AspNetCore.Mvc;
using muatamer_camunda_poc.Services.GenericWorkflow;

namespace muatamer_camunda_poc.Controllers;
[Route("api/[controller]")]
[ApiController]
public class WorkflowController : ControllerBase
{
    private readonly IWorkflowService _workflowService;

    public WorkflowController(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    [Route("status")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = (await _workflowService.Status()).ToString();
        return Ok(result);
    }
}
