using Microsoft.AspNetCore.Mvc;
using muatamer_camunda_poc.Services.MuatamerProcess;

namespace muatamer_camunda_poc.Controllers;
[ApiController]
[Route("[controller]")]
public class MuatamerWorkflowController : ControllerBase
{
    private readonly ILogger<MuatamerWorkflowController> _logger;
    private readonly IMuatamerProcessService _zeebeService;

    public MuatamerWorkflowController(ILogger<MuatamerWorkflowController> logger, IMuatamerProcessService zeebeService)
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
        var response = await _zeebeService.Deploy("muatamer-process.bpmn");
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

    [Route("approve-voucher")]
    [HttpGet]
    public IActionResult ApproveVoucher(bool isApproved, int groupId, long processInstanceKey, string timer)
    {
        _zeebeService.ApproveVoucher(isApproved, groupId, processInstanceKey, timer);
        return Ok("done");
    }

    [Route("voucher-paid")]
    [HttpGet]
    public IActionResult VoucherPaid(int groupId, string processInstanceKey)
    {
        _zeebeService.VoucherPaid(groupId, processInstanceKey);
        return Ok("done");
    }
}
