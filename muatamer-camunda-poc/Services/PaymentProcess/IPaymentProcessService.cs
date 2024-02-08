using Zeebe.Client.Api.Responses;

namespace muatamer_camunda_poc.Services.PaymentProcess;

public interface IPaymentProcessService
{
    public void StartWorkers();
    public Task<IProcessInstanceResponse> CreateProcessInstance(int RequestId);
    Task<string> ManualApprovalMessage(string processInstanceKey);
    Task<string> ManualPaymentMessage(string processInstanceKey);
}
