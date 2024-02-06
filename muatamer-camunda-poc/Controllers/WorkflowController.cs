using Microsoft.AspNetCore.Mvc;
using muatamer_camunda_poc.Services;

namespace muatamer_camunda_poc.Controllers;
[ApiController]
[Route("[controller]")]
public class WorkflowController : ControllerBase
{
    private readonly ILogger<WorkflowController> _logger;
    private readonly IZeebeService _zeebeService;

    public WorkflowController(ILogger<WorkflowController> logger, IZeebeService zeebeService)
    {
        _logger = logger;
        _zeebeService = zeebeService;
    }

    [Route("/status")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = (await _zeebeService.Status()).ToString();
        return Ok(result);
    }

    [Route("/deploy")]
    [HttpGet]
    public async Task<IActionResult> DeployWorkflow()
    {
        var response = await _zeebeService.Deploy("muatamer-process.bpmn");
        return Ok(response);
    }

    [Route("/start-workers")]
    [HttpGet]
    public IActionResult StartWorkflow()
    {
        _zeebeService.StartWorkers();
        return Ok("done");
    }

    [Route("/create-instance")]
    [HttpGet]
    public async Task<IActionResult> StartWorkflowInstance(string bpmProcessId, int groupId)
    {
        var instance = await _zeebeService.CreateWorkflowInstance(bpmProcessId, groupId);

        if (instance == null) return BadRequest("invalid groupid");
        return Ok(instance);
    }

    [Route("/approve-voucher")]
    [HttpGet]
    public IActionResult ApproveVoucher(bool isApproved, string groupId, long processInstanceKey)
    {
        _zeebeService.ApproveVoucher(isApproved, groupId, processInstanceKey);
        return Ok("done");
    }

    [Route("/voucher-paid")]
    [HttpGet]
    public IActionResult VoucherPaid(string groupId, string processInstanceKey)
    {
        _zeebeService.VoucherPaid(groupId, processInstanceKey);
        return Ok("done");
    }
}
