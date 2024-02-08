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

    [Route("create-instance")]
    [HttpGet]
    public async Task<IActionResult> StartWorkflowInstance(int requestId)
    {
        var instance = await _zeebeService.CreateProcessInstance(requestId);

        if (instance == null) return BadRequest("invalid groupid");
        return Ok(instance);
    }

    [Route("manual-approve")]
    [HttpGet]
    public async Task<IActionResult> ManualApproval(string processId)
    {
        var instance = await _zeebeService.ManualApprovalMessage(processId);

        return Ok(instance);
    }

    [Route("manual-payment")]
    [HttpGet]
    public async Task<IActionResult> ManualPayment(string processId)
    {
        var instance = await _zeebeService.ManualPaymentMessage(processId);

        return Ok(instance);
    }
}
