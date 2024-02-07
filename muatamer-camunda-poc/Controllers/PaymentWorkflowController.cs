using Microsoft.AspNetCore.Mvc;
using muatamer_camunda_poc.Services.PaymentProcess;

namespace muatamer_camunda_poc.Controllers;
[ApiController]
[Route("[controller]")]
public class PaymentWorkflowController : ControllerBase
{
    private readonly ILogger<PaymentWorkflowController> _logger;
    private readonly IPaymentProcessService _zeebeService;

    public PaymentWorkflowController(ILogger<PaymentWorkflowController> logger, IPaymentProcessService zeebeService)
    {
        _logger = logger;
        _zeebeService = zeebeService;
    }

    [Route("status")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = (await _zeebeService.Status()).ToString();
        return Ok(result);
    }

    [Route("deploy")]
    [HttpGet]
    public async Task<IActionResult> DeployWorkflow()
    {
        var response = await _zeebeService.Deploy("payment-process.bpmn");
        _zeebeService.StartWorkers();
        return Ok(response);
    }

    [Route("create-instance")]
    [HttpGet]
    public async Task<IActionResult> StartWorkflowInstance(string processId, int groupId)
    {
        var instance = await _zeebeService.CreateWorkflowInstance(processId, groupId);

        if (instance == null) return BadRequest("invalid groupid");
        return Ok(instance);
    }
}
