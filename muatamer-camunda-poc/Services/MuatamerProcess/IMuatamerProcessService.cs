using Zeebe.Client.Api.Responses;

namespace muatamer_camunda_poc.Services.MuatamerProcess
{
    public interface IMuatamerProcessService
    {
        public void StartWorkers();
        public Task<IProcessInstanceResponse> CreateWorkflowInstance(int groupId);
        public Task<string> VoucherPaid(int groupId, string processInstanceKey);
        public void ApproveVoucher(bool isApproved, int groupId, long processInstanceKey, string timer);
    }
}