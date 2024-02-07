using Zeebe.Client.Api.Responses;

namespace muatamer_camunda_poc.Services.PaymentProcess;

public interface IPaymentProcessService
{
    public Task<ITopology> Status();
    public Task<IDeployResourceResponse> Deploy(string modelFile);
    public void StartWorkers();
    public Task<IProcessInstanceResponse> CreateWorkflowInstance(string bpmProcessId, int groupId);
}
