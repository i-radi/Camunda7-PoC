using System.Threading.Tasks;
using Zeebe.Client.Api.Responses;
using Zeebe.Client.Api.Worker;

namespace muatamer_camunda_poc.Services.MuatamerProcess
{
    public interface IMuatamerProcessService
    {
        public Task<ITopology> Status();
        public Task<IDeployResourceResponse> Deploy(string modelFile);
        public void StartWorkers();
        public Task<IProcessInstanceResponse> CreateWorkflowInstance(string bpmProcessId, int groupId);
        public Task<string> VoucherPaid(int groupId, string processInstanceKey);
        public void ApproveVoucher(bool isApproved, int groupId, long processInstanceKey, string timer);


    }
}