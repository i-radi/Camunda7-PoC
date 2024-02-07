using fastJSON;
using muatamer_camunda_poc.Context;
using muatamer_camunda_poc.DTOs;
using muatamer_camunda_poc.Services.MuatamerProcess;
using Zeebe.Client.Api.Responses;
using Zeebe.Client.Api.Worker;
using Zeebe.Client.Impl.Builder;
using Zeebe.Client;
using System.Text.Json;

namespace muatamer_camunda_poc.Services.PaymentProcess;

public class PaymentProcessService : IPaymentProcessService
{
    private readonly IZeebeClient _client;
    private readonly ILogger<PaymentProcessService> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly ApplicationDbContext _dbContext;

    public PaymentProcessService(IConfiguration config, ILogger<PaymentProcessService> logger, IWebHostEnvironment env, ApplicationDbContext dbContext)
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
        BLValidationWorker();
        MinistryApprovalWorker();
        CreateVoucherWorker();
        PaymentSystemWorker();
        CancelRequestWorker();
    }

    public async Task<IProcessInstanceResponse> CreateWorkflowInstance(string bpmProcessId, int groupId)
    {
        _logger.LogInformation("Creating workflow instance...");

        var paymnetDto = new PaymentProcessDto
        {
            GroupId = groupId,
            BLValidationResult = 99,
            CreateVoucherResult = 99,
            IsFullPaymentResult = 99,
            MinistryApprovalResult = 99,
            PaymentSystemResult = 99
        };

        var instance = await _client.NewCreateProcessInstanceCommand()
            .BpmnProcessId(bpmProcessId)
            .LatestVersion()
            .Variables(JsonSerializer.Serialize(paymnetDto))
            .SendWithRetry(TimeSpan.FromSeconds(30));

        return instance;
    }

    public void BLValidationWorker()
    {
        _createWorker("BLValidationDef", async (client, job) =>
        {
            _logger.LogInformation("BL validation worker received job: " + job);

            var paymentProcessDto = JsonSerializer.Deserialize< PaymentProcessDto>(job.Variables);

            // logic
            paymentProcessDto!.BLValidationResult = 0;

            await client.NewCompleteJobCommand(job.Key)
                .Variables(JsonSerializer.Serialize(paymentProcessDto))
                .Send();

            _logger.LogInformation("BL validation worker completed");
        });
    }

    public void MinistryApprovalWorker()
    {
        _createWorker("MinistryApprovalDef", async (client, job) =>
        {
            _logger.LogInformation("Ministry Approval worker received job: " + job);

            var paymentProcessDto = JsonSerializer.Deserialize<PaymentProcessDto>(job.Variables);

            // logic
            paymentProcessDto!.MinistryApprovalResult = 0;

            await client.NewCompleteJobCommand(job.Key)
                .Variables(JsonSerializer.Serialize(paymentProcessDto))
                .Send();

            _logger.LogInformation("Ministry Approval worker completed");
        });
    }

    public void CreateVoucherWorker()
    {
        _createWorker("CreateVoucherDef", async (client, job) =>
        {
            _logger.LogInformation("Create Voucher worker received job: " + job);

            var paymentProcessDto = JsonSerializer.Deserialize<PaymentProcessDto>(job.Variables);

            // logic
            paymentProcessDto!.CreateVoucherResult = 0;

            await client.NewCompleteJobCommand(job.Key)
                .Variables(JsonSerializer.Serialize(paymentProcessDto))
                .Send();

            _logger.LogInformation("Create Voucher worker completed");
        });
    }

    public void PaymentSystemWorker()
    {
        _createWorker("PaymentSystemDef", async (client, job) =>
        {
            _logger.LogInformation("Payment System worker received job: " + job);

            var paymentProcessDto = JsonSerializer.Deserialize<PaymentProcessDto>(job.Variables);

            // logic
            paymentProcessDto!.PaymentSystemResult = 1;
            paymentProcessDto!.IsFullPaymentResult = 1;

            await client.NewCompleteJobCommand(job.Key)
                .Variables(JsonSerializer.Serialize(paymentProcessDto))
                .Send();

            _logger.LogInformation("Payment System worker completed");
        });
    }

    public void CancelRequestWorker()
    {
        _createWorker("CancelRequestDef", async (client, job) =>
        {
            _logger.LogInformation("Cancel Request worker received job: " + job);

            var paymentProcessDto = JsonSerializer.Deserialize<PaymentProcessDto>(job.Variables);

            // logic

            await client.NewCompleteJobCommand(job.Key)
                .Send();

            _logger.LogInformation("Cancel Request worker completed");
        });
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

}
