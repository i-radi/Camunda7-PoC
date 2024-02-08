using Zeebe.Client.Api.Responses;

namespace muatamer_camunda_poc.Services.GenericWorkflow;

public interface IWorkflowService
{
    public Task<ITopology> Status();
    public Task<IDeployResourceResponse> Deploy(string modelFile);

}
