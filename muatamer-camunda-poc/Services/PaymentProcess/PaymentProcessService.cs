using Microsoft.EntityFrameworkCore;
using muatamer_camunda_poc.Constants;
using muatamer_camunda_poc.Context;
using muatamer_camunda_poc.DTOs.PaymentProcess;
using muatamer_camunda_poc.Enum;
using muatamer_camunda_poc.Enum.PaymentProcess;
using muatamer_camunda_poc.Extentions;
using muatamer_camunda_poc.Services.GenericWorkflow;
using System.Text.Json;
using Zeebe.Client.Api.Responses;

namespace muatamer_camunda_poc.Services.PaymentProcess;

public class PaymentProcessService : WorkflowService, IPaymentProcessService
{
    private readonly ILogger<WorkflowService> _logger;
    private readonly ApplicationDbContext _dbContext;

    public PaymentProcessService(IConfiguration config, ILogger<WorkflowService> logger, ApplicationDbContext dbContext, IWebHostEnvironment env) : base(config, logger, env)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<IProcessInstanceResponse> CreateProcessInstance(int requestId)
    {
        _logger.LogInformation("Creating workflow instance...");

        var paymnetDto = new PaymentProcessDto
        {
            GroupId = requestId,
            BLValidationResult = 99,
            CreateVoucherResult = 99,
            IsFullPaymentResult = 99,
            MinistryApprovalResult = 99,
            PaymentSystemResult = 99
        };

        var instance = await ZeebeClient.NewCreateProcessInstanceCommand()
            .BpmnProcessId(BpmProcessId.PaymentBpmProcessId)
            .LatestVersion()
            .Variables(JsonSerializer.Serialize(paymnetDto))
            .SendWithRetry(TimeSpan.FromSeconds(30));

        return instance;
    }

    public void StartWorkers()
    {
        BLValidationWorker();
        MinistryApprovalWorker();
        CreateVoucherWorker();
        PaymentSystemWorker();
        CancelRequestWorker();
    }

    private void BLValidationWorker()
    {
        ZeebeClient.CreateWorker("BLValidationDef", async (jobClient, job) =>
        {
            _logger.LogInformation("BL validation worker received job: " + job);

            var paymentProcessDto = JsonSerializer.Deserialize<PaymentProcessDto>(job.Variables);
            paymentProcessDto.processInsanceId = job.ProcessInstanceKey.ToString();

            var group = _dbContext.UmrahGroups
                                .Include(g => g.MuatamerInformations)
                                .FirstOrDefault(g => g.Id == paymentProcessDto.GroupId);

            paymentProcessDto!.BLValidationResult = 1;
            paymentProcessDto!.IsManualNationality = false;

            if (group is not null && group.MuatamerInformations.Any())
            {
                paymentProcessDto!.BLValidationResult = 0;
            }
            if (group.Country == "Egypt")
            {
                paymentProcessDto.IsManualNationality = true;
            }

            await jobClient.ComplateTaskAsync(job.Key, paymentProcessDto);

            _logger.LogInformation("BL validation worker completed");
        });
    }

    private void MinistryApprovalWorker()
    {
        ZeebeClient.CreateWorker(nameof(PaymentProcessWorker.MinistryApprovalDef), async (jobClient, job) =>
        {
            _logger.LogInformation("Ministry Approval worker received job: " + job);

            var paymentProcessDto = JsonSerializer.Deserialize<PaymentProcessDto>(job.Variables);

            // logic
            paymentProcessDto!.MinistryApprovalResult = 0;

            await jobClient.ComplateTaskAsync(job.Key, paymentProcessDto);

            _logger.LogInformation("Ministry Approval worker completed");
        });
    }

    private void CreateVoucherWorker()
    {
        ZeebeClient.CreateWorker(nameof(PaymentProcessWorker.CreateVoucherDef), async (jobClient, job) =>
        {
            _logger.LogInformation("Create Voucher worker received job: " + job);

            var paymentProcessDto = JsonSerializer.Deserialize<PaymentProcessDto>(job.Variables);
            paymentProcessDto!.isManualPayment = true;

            // logic
            paymentProcessDto!.CreateVoucherResult = 0;

            await jobClient.ComplateTaskAsync(job.Key, paymentProcessDto);

            _logger.LogInformation("Create Voucher worker completed");
        });
    }

    private void PaymentSystemWorker()
    {
        ZeebeClient.CreateWorker(nameof(PaymentProcessWorker.PaymentSystemDef), async (jobClient, job) =>
        {
            _logger.LogInformation("Payment System worker received job: " + job);

            var paymentProcessDto = JsonSerializer.Deserialize<PaymentProcessDto>(job.Variables);

            // logic
            paymentProcessDto!.PaymentSystemResult = 1;
            paymentProcessDto!.IsFullPaymentResult = 1;

            await jobClient.ComplateTaskAsync(job.Key, paymentProcessDto);

            _logger.LogInformation("Payment System worker completed");
        });
    }

    private void CancelRequestWorker()
    {
        ZeebeClient.CreateWorker(nameof(PaymentProcessWorker.CancelRequestDef), async (jobClient, job) =>
        {
            _logger.LogInformation("Cancel Request worker received job: " + job);

            var paymentProcessDto = JsonSerializer.Deserialize<PaymentProcessDto>(job.Variables);

            // logic

            await jobClient.ComplateTaskAsync(job.Key);

            _logger.LogInformation("Cancel Request worker completed");
        });
    }

    public async Task<string> ManualApprovalMessage(string processInstanceKey)
    {
        var result = await ZeebeClient.PublishMessageAsync(
            processInstanceKey,
            nameof(PaymentProcessMessage.ManualApproval),
            new { MinistryApprovalResult = 0 });

        return JsonSerializer.Serialize(result);
    }

    public async Task<string> ManualPaymentMessage(string processInstanceKey)
    {

        var result = await ZeebeClient.PublishMessageAsync(
            processInstanceKey,
            nameof(PaymentProcessMessage.ManualPayment),
            new { PaymentSystemResult = 0, IsFullPaymentResult = 1 });

        return JsonSerializer.Serialize(result);
    }
}
