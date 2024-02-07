using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using fastJSON;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using muatamer_camunda_poc.Context;
using muatamer_camunda_poc.DTOs;
using Zeebe.Client;
using Zeebe.Client.Api.Responses;
using Zeebe.Client.Api.Worker;
using Zeebe.Client.Impl.Builder;

namespace muatamer_camunda_poc.Services;

public class ZeebeService : IZeebeService
{
    private readonly IZeebeClient _client;
    private readonly ILogger<ZeebeService> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly ApplicationDbContext _dbContext;

    public ZeebeService(IConfiguration config, ILogger<ZeebeService> logger, IWebHostEnvironment env, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _env = env;
        _dbContext = dbContext;
        var authServer = config["ZeebeClientCloudSettings:OAuthURL"];
        var clientId = config["ZeebeClientCloudSettings:ClientId"];
        var clientSecret = config["ZeebeClientCloudSettings:ClientSecret"];
        var zeebeUrl = config["ZeebeClientCloudSettings:ClusterURL"];
        char[] port = { '4', '3', ':' };
        var audience = zeebeUrl?.TrimEnd(port);

        // docker container 
        //_client =
        //    ZeebeClient.Builder()
        //        .UseGatewayAddress("localhost:26500")
        //        .UsePlainText()
        //        .Build();

        // cloud-
        _client =
            ZeebeClient.Builder()
                .UseGatewayAddress(zeebeUrl)
                .UseTransportEncryption()
                .UseAccessTokenSupplier(
                    CamundaCloudTokenProvider.Builder()
                        .UseAuthServer(authServer)
                        .UseClientId(clientId)
                        .UseClientSecret(clientSecret)
                        .UseAudience(audience)
                        .Build())
                .Build();
    }

    public Task<ITopology> Status()
    {
        return _client.TopologyRequest().Send();
    }

    public async Task<IDeployResourceResponse> Deploy(string modelFile)
    {
        var filename = Path.Combine(_env.ContentRootPath, "Resources", modelFile);
        var deployment = await _client.NewDeployCommand().AddResourceFile(filename).Send();
        var res = deployment.Processes[0];
        _logger.LogInformation("Deployed BPMN Model: " + res?.BpmnProcessId + " v." + res?.Version);
        return deployment;
    }

    public void StartWorkers()
    {
        NotEmptyGroupWorker();
        GroupNotGreaterThan100Worker();
        GroupHasSameNationalityWorker();
        CreateVoucherWorker();
        CancelVoucherWorker();
    }

    public void NotEmptyGroupWorker()
    {
        _createWorker("validate-group-not-empty", async (client, job) =>
        {
            _logger.LogInformation("Received job: " + job);
            var groupIdVariable = JSON.ToObject<GroupDTO>(job.Variables).GroupId;
            int groupId = groupIdVariable;

            var group = _dbContext.UmrahGroups
            .Include(g => g.MuatamerInformations)
            .FirstOrDefault(g => g.Id == groupId);

            var isEmpty = true;
            if (group is not null && group.MuatamerInformations.Any()) isEmpty = false;

            var notEmptyGroupDTO = new NotEmptyGroupDTO { isEmpty = isEmpty };
            await client.NewCompleteJobCommand(job.Key)
                .Variables(JsonSerializer.Serialize(notEmptyGroupDTO))
                .Send();

            _logger.LogInformation("Get NotEmptyGroup worker completed");
        });
    }

    public void GroupNotGreaterThan100Worker()
    {
        _createWorker("validate-group-not-greater-than-100", async (client, job) =>
        {
            _logger.LogInformation("Received job: " + job);

            var groupIdString = JSON.ToObject<GroupDTO>(job.Variables).GroupId;
            int groupId = groupIdString;

            var group = _dbContext.UmrahGroups
            .Include(g => g.MuatamerInformations)
            .FirstOrDefault(g => g.Id == groupId);

            var MuatamerCount = 0;
            if (group is not null && group.MuatamerInformations.Any()) MuatamerCount = group.MuatamerInformations.Count;

            var groupNotGreaterThan100DTO = new GroupNotGreaterThan100DTO { MuatamerCount= MuatamerCount };
            await client.NewCompleteJobCommand(job.Key)
                .Variables(JsonSerializer.Serialize(groupNotGreaterThan100DTO))
                .Send();
            _logger.LogInformation("Get GroupNotGreaterThan100 worker completed");
        });
    }

    public void GroupHasSameNationalityWorker()
    {
        _createWorker("validate-group-has-same-nationality", async (client, job) =>
        {
            var groupIdString = JSON.ToObject<GroupDTO>(job.Variables).GroupId;
            int groupId = groupIdString;

            var group = _dbContext.UmrahGroups
            .Include(g => g.MuatamerInformations)
            .FirstOrDefault(g => g.Id == groupId);

            var isSameCountry = false;

            if (group?.MuatamerInformations.Any() == true)
            {
                var firstCountryName = group.MuatamerInformations.FirstOrDefault()?.CountryName;
                isSameCountry = group.MuatamerInformations.All(m => m.CountryName == firstCountryName);
            }

            var groupHasSameNationalityDTO = new GroupHasSameNationalityDTO { isSameNationality = isSameCountry};
            await client.NewCompleteJobCommand(job.Key)
                .Variables(JsonSerializer.Serialize(groupHasSameNationalityDTO))
                .Send();
            _logger.LogInformation("Get GroupHasSameNationality worker completed");
        });
    }

    public void CreateVoucherWorker()
    {
        _createWorker("create-voucher", async (client, job) =>
        {
            _logger.LogInformation("Received job: " + job);

            // call api
            await client.NewCompleteJobCommand(job.Key).Send();

            _logger.LogInformation("Get CreateVoucher worker completed");
        });
    }

    public void CancelVoucherWorker()
    {
        _createWorker("cancel-voucher", async (client, job) =>
        {
            _logger.LogInformation("Received job: " + job);

            // call api
            await client.NewCompleteJobCommand(job.Key).Send();

            _logger.LogInformation("Get CancelVoucher worker completed");
        });
    }

    public async Task<IProcessInstanceResponse> CreateWorkflowInstance(string bpmProcessId, int groupId)
    {
        _logger.LogInformation("Creating workflow instance...");

        if (!IsGroupExist(groupId))
        {
            return null;
        }
        var groupDto = new GroupDTO { GroupId = groupId };
        var instance = await _client.NewCreateProcessInstanceCommand()
            .BpmnProcessId(bpmProcessId)
            .LatestVersion()
            .Variables(JsonSerializer.Serialize(groupDto))
            .SendWithRetry(TimeSpan.FromSeconds(30));

        return instance;
    }

    public void ApproveVoucher(bool isApproved, int groupId, long processInstanceKey)
    {

        var approveVoucherDTO = new ApproveVoucherDTO
        {
            isApproved = isApproved,
            processInstanceKey = processInstanceKey.ToString(),
            Voucher_Paid = "Voucher_Paid"
        };

        _createWorker("io.camunda.zeebe:userTask", async (client, job) =>
        {
            if (job.ProcessInstanceKey == processInstanceKey)
            {
                var groupIdvariable = JSON.ToObject<GroupDTO>(job.Variables).GroupId;

                if (groupId == groupIdvariable)
                {
                    await client.NewCompleteJobCommand(job.Key)
                    .Variables(JsonSerializer.Serialize(approveVoucherDTO))
                    .Send();
                }
                _logger.LogInformation("Get GroupHasSameNationality worker completed");
            }

        }, "ApproveVoucher");
    }

    private void _createWorker(string jobType, JobHandler handleJob, string name = null)
    {
        name = name ?? jobType;
        _client.NewWorker()
                .JobType(jobType)
                .Handler(handleJob)
                .MaxJobsActive(5)
                .Name(jobType)
                .PollInterval(TimeSpan.FromSeconds(50))
                .PollingTimeout(TimeSpan.FromSeconds(50))
                .Timeout(TimeSpan.FromSeconds(10))
                .Open();
    }

    public async Task<string> VoucherPaid(int groupId, string processInstanceKey)
    {
        var voucherPaidDTO = new VoucherPaidDTO { Voucher_Paid = "Voucher_Paid" };
        var result = await _client.NewPublishMessageCommand()
                .MessageName("Voucher_Paid")
                .CorrelationKey(processInstanceKey)
                .Variables(JsonSerializer.Serialize(voucherPaidDTO))
                .Send();

        var jsonParams = new JSONParameters { ShowReadOnlyProperties = true };
        return JSON.ToJSON(result, jsonParams);
    }

    private bool IsGroupExist(int groupId)
    {
        return _dbContext.UmrahGroups.Any(g => g.Id == groupId);
    }
}