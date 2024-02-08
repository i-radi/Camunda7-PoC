using System.Text.Json;
using Zeebe.Client;
using Zeebe.Client.Api.Responses;
using Zeebe.Client.Api.Worker;

namespace muatamer_camunda_poc.Extentions;

public static class CamundaExtentions
{
    public static void CreateWorker(this IZeebeClient client, string jobType, JobHandler handleJob)
    {
        client.NewWorker()
                .JobType(jobType)
                .Handler(handleJob)
                .MaxJobsActive(5)
                .Name(jobType)
                .PollInterval(TimeSpan.FromSeconds(50))
                .PollingTimeout(TimeSpan.FromSeconds(50))
                .Timeout(TimeSpan.FromSeconds(10))
                .Open();
    }

    public static async Task ComplateTaskAsync(this IJobClient jobClient, long key, object parameters = null)
    {
        await jobClient.NewCompleteJobCommand(key)
                .Variables(JsonSerializer.Serialize(parameters))
                .Send();
    }

    public static async Task<IPublishMessageResponse> PublishMessageAsync(this IZeebeClient client, string processInstanceKey, string messageName, object parameters = null)
    {
        return await client.NewPublishMessageCommand()
                .MessageName(messageName)
                .CorrelationKey(processInstanceKey)
                .Variables(JsonSerializer.Serialize(parameters))
                .Send();
    }
}
