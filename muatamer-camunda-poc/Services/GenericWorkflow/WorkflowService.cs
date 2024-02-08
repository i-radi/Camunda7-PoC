using Zeebe.Client;
using Zeebe.Client.Api.Responses;
using Zeebe.Client.Impl.Builder;

namespace muatamer_camunda_poc.Services.GenericWorkflow;

public class WorkflowService : IWorkflowService
{
    public readonly IZeebeClient ZeebeClient;
    private readonly ILogger<WorkflowService> _logger;
    private readonly IWebHostEnvironment _env;

    public WorkflowService(IConfiguration config, ILogger<WorkflowService> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
        var authServer = config["ZeebeClientCloudSettings:OAuthURL"];
        var clientId = config["ZeebeClientCloudSettings:ClientId"];
        var clientSecret = config["ZeebeClientCloudSettings:ClientSecret"];
        var zeebeUrl = config["ZeebeClientCloudSettings:ClusterURL"];
        char[] port = { '4', '3', ':' };
        var audience = zeebeUrl?.TrimEnd(port);

        // docker container 
        //ZeebeClient =
        //    ZeebeClient.Builder()
        //        .UseGatewayAddress("localhost:26500")
        //        .UsePlainText()
        //        .Build();

        // cloud-
        ZeebeClient =
            Zeebe.Client.ZeebeClient.Builder()
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
        return ZeebeClient.TopologyRequest().Send();
    }

    public async Task<IDeployResourceResponse> Deploy(string modelFile)
    {
        var filename = Path.Combine(_env.ContentRootPath, "Resources", modelFile);
        var deployment = await ZeebeClient.NewDeployCommand().AddResourceFile(filename).Send();
        var res = deployment.Processes[0];
        _logger.LogInformation("Deployed BPMN Model: " + res?.BpmnProcessId + " v." + res?.Version);
        return deployment;
    }

}
