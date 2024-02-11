using fastJSON;
using Microsoft.EntityFrameworkCore;
using muatamer_camunda_poc.Constants;
using muatamer_camunda_poc.Context;
using muatamer_camunda_poc.DTOs.MuatamerProcess;
using muatamer_camunda_poc.Extentions;
using muatamer_camunda_poc.Services.GenericWorkflow;
using System.Text.Json;
using Zeebe.Client.Api.Responses;

namespace muatamer_camunda_poc.Services.MuatamerProcess;

public class MuatamerProcessService : WorkflowService, IMuatamerProcessService
{
    private readonly ILogger<WorkflowService> _logger;
    private readonly ApplicationDbContext _dbContext;

    public MuatamerProcessService(IConfiguration config, ILogger<WorkflowService> logger, IWebHostEnvironment env, ApplicationDbContext dbContext) : base(config, logger, env)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public void StartWorkers()
    {
        NotEmptyGroupWorker();
        GroupNotGreaterThan100Worker();
        GroupHasSameNationalityWorker();
        CreateVoucherWorker();
        CancelVoucherWorker();
    }

    public async Task<IProcessInstanceResponse> CreateWorkflowInstance(int groupId)
    {
        _logger.LogInformation("Creating workflow instance...");

        if (!IsGroupExist(groupId))
        {
            return null;
        }
        var groupDto = new GroupIdDTO { GroupId = groupId };
        var instance = await ZeebeClient.NewCreateProcessInstanceCommand()
            .BpmnProcessId(BpmProcessId.MuatamerBpmProcessId)
            .LatestVersion()
            .Variables(JsonSerializer.Serialize(groupDto))
            .SendWithRetry(TimeSpan.FromSeconds(30));

        return instance;
    }

    public void NotEmptyGroupWorker()
    {
        ZeebeClient.CreateWorker("validate-group-not-empty", async (client, job) =>
        {
            _logger.LogInformation("Received job: " + job);
            var groupIdVariable = JSON.ToObject<GroupIdDTO>(job.Variables).GroupId;
            int groupId = groupIdVariable;
            try
            {
                var group = _dbContext.UmrahGroups
                                .Include(g => g.MuatamerInformations)
                                .FirstOrDefault(g => g.Id == groupId);

                var isEmpty = true;
                if (group is not null && group.MuatamerInformations.Any()) isEmpty = false;

                var notEmptyGroupDTO = new NotEmptyGroupDTO { isEmpty = isEmpty };
                await client.ComplateTaskAsync(job.Key, notEmptyGroupDTO);
            }
            catch (Exception)
            {
                await client.ComplateTaskAsync(job.Key, new NotEmptyGroupDTO { isEmpty = true });
            }
            finally
            {
                _logger.LogInformation("Get NotEmptyGroup worker completed");
            }
        });
    }

    public void GroupNotGreaterThan100Worker()
    {
        ZeebeClient.CreateWorker("validate-group-not-greater-than-100", async (client, job) =>
        {
            _logger.LogInformation("Received job: " + job);

            var groupIdString = JSON.ToObject<GroupIdDTO>(job.Variables).GroupId;
            int groupId = groupIdString;

            var group = _dbContext.UmrahGroups
            .Include(g => g.MuatamerInformations)
            .FirstOrDefault(g => g.Id == groupId);

            var MuatamerCount = 0;
            if (group is not null && group.MuatamerInformations.Any()) MuatamerCount = group.MuatamerInformations.Count;

            var groupNotGreaterThan100DTO = new GroupNotGreaterThan100DTO { MuatamerCount = MuatamerCount };
            await client.ComplateTaskAsync(job.Key, groupNotGreaterThan100DTO);
            _logger.LogInformation("Get GroupNotGreaterThan100 worker completed");
        });
    }

    public void GroupHasSameNationalityWorker()
    {
        ZeebeClient.CreateWorker("validate-group-has-same-nationality", async (client, job) =>
        {
            var groupIdString = JSON.ToObject<GroupIdDTO>(job.Variables).GroupId;

            var groupHasSameNationalityDTO = new GroupHasSameNationalityDTO { isSameNationality = true };
            await client.ComplateTaskAsync(job.Key, groupHasSameNationalityDTO);
            _logger.LogInformation("Get GroupHasSameNationality worker completed");
        });
    }

    public void CreateVoucherWorker()
    {
        ZeebeClient.CreateWorker("create-voucher", async (client, job) =>
        {
            _logger.LogInformation("Received job: " + job);

            // call api
            await client.ComplateTaskAsync(job.Key);

            _logger.LogInformation("Get CreateVoucher worker completed");
        });
    }

    public void CancelVoucherWorker()
    {
        ZeebeClient.CreateWorker("cancel-voucher", async (client, job) =>
        {
            _logger.LogInformation("Received job: " + job);

            // call api
            await client.ComplateTaskAsync(job.Key);

            _logger.LogInformation("Get CancelVoucher worker completed");
        });
    }

    public void ApproveVoucher(bool isApproved, int groupId, long processInstanceKey, string timer)
    {

        var approveVoucherDTO = new ApproveVoucherDTO
        {
            isApproved = isApproved,
            processInstanceKey = processInstanceKey.ToString(),
            Voucher_Paid = "Voucher_Paid",
            remainingTime = $"PT{timer}S"
        };

        ZeebeClient.CreateWorker("io.camunda.zeebe:userTask", async (client, job) =>
        {
            if (job.ProcessInstanceKey == processInstanceKey)
            {
                var groupIdvariable = JSON.ToObject<GroupIdDTO>(job.Variables).GroupId;

                if (groupId == groupIdvariable)
                {
                    await client.ComplateTaskAsync(job.Key, approveVoucherDTO);
                }
                _logger.LogInformation("Get GroupHasSameNationality worker completed");
            }

        });
    }

    public async Task<string> VoucherPaid(int groupId, string processInstanceKey)
    {
        var result = await ZeebeClient.PublishMessageAsync(processInstanceKey, "Voucher_Paid");

        return JsonSerializer.Serialize(result);
    }

    private bool IsGroupExist(int groupId)
    {
        return _dbContext.UmrahGroups.Any(g => g.Id == groupId);
    }
}