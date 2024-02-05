using System;
using System.IO;
using System.Net.Http;
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
            var groupIdString = JSON.ToObject<NotEmptyGroupDTO>(job.Variables).GroupId;
            int groupId = int.Parse(groupIdString);

            var group = _dbContext.UmrahGroups
            .Include(g => g.MuatamerInformations)
            .FirstOrDefault(g => g.Id == groupId);

            var isEmpty = true;
            if (group is not null && group.MuatamerInformations.Any()) isEmpty = false;


            await client.NewCompleteJobCommand(job.Key)
                .Variables("{\"isEmpty\": \"" + isEmpty + "\"}")
                .Send();

            _logger.LogInformation("Get NotEmptyGroup worker completed");
        });
    }

    public void GroupNotGreaterThan100Worker()
    {
        _createWorker("validate-group-not-greater-than-100", async (client, job) =>
        {
            _logger.LogInformation("Received job: " + job);

            var groupIdString = JSON.ToObject<NotEmptyGroupDTO>(job.Variables).GroupId;
            int groupId = int.Parse(groupIdString);

            var group = _dbContext.UmrahGroups
            .Include(g => g.MuatamerInformations)
            .FirstOrDefault(g => g.Id == groupId);

            var MuatamerCount = 0;
            if (group is not null && group.MuatamerInformations.Any()) MuatamerCount = group.MuatamerInformations.Count;


            await client.NewCompleteJobCommand(job.Key)
                .Variables("{\"MuatamerCount\": " + MuatamerCount + "}")
                .Send();
            _logger.LogInformation("Get GroupNotGreaterThan100 worker completed");
        });
    }

    public void GroupHasSameNationalityWorker()
    {
        _createWorker("validate-group-has-same-nationality", async (client, job) =>
        {
            var groupIdString = JSON.ToObject<NotEmptyGroupDTO>(job.Variables).GroupId;
            int groupId = int.Parse(groupIdString);

            var group = _dbContext.UmrahGroups
            .Include(g => g.MuatamerInformations)
            .FirstOrDefault(g => g.Id == groupId);

            var isSameNationality = true;
            if (group is not null 
            && group.MuatamerInformations.Any())
            {
                var nationalityId = group.MuatamerInformations.FirstOrDefault().NationalityId;
                foreach ( var Muatamer in group.MuatamerInformations)
                {
                    if (Muatamer.NationalityId != nationalityId)
                    {
                        isSameNationality = false;
                    }
                }
            }


            await client.NewCompleteJobCommand(job.Key)
                .Variables("{\"isSameNationality\": \"" + isSameNationality + "\"}")
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

    public async Task<String> CreateWorkflowInstance(string bpmProcessId, string groupId)
    {
        _logger.LogInformation("Creating workflow instance...");
        var instance = await _client.NewCreateProcessInstanceCommand()
             .BpmnProcessId(bpmProcessId)
            .LatestVersion()
            .Variables("{\"groupId\": \"" + groupId + "\"}")
            .WithResult()
            .Send();
        var jsonParams = new JSONParameters { ShowReadOnlyProperties = true };
        return JSON.ToJSON(instance, jsonParams);
    }

    public void ApproveVoucher(bool isApproved, string groupId)
    {

        _createWorker("io.camunda.zeebe:userTask", async (client, job) =>
        {
            var groupIdvariable = JSON.ToObject<NotEmptyGroupDTO>(job.Variables).GroupId;

            if (groupId == groupIdvariable)
            {
                await client.NewCompleteJobCommand(job.Key)
                .Variables("{\"isApproved\": \"" + isApproved + "\" , \"Voucher_Paid\": \"Voucher_Paid\"}")
                .Send();
            }

            _logger.LogInformation("Get GroupHasSameNationality worker completed");
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

    public async Task<string> VoucherPaid(string groupId)
    {
        var result = await _client.NewPublishMessageCommand()
                .MessageName("Voucher_Paid")
                .CorrelationKey("Voucher_Paid")
                .Variables("{\"Voucher_Paid\": \"Voucher_Paid\"}")
                .Send();

        var jsonParams = new JSONParameters { ShowReadOnlyProperties = true };
        return JSON.ToJSON(result, jsonParams);
    }
}