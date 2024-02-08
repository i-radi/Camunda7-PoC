namespace muatamer_camunda_poc.DTOs.MuatamerProcess;

public class ApproveVoucherDTO
{
    public bool isApproved { get; set; }
    public string Voucher_Paid { get; set; }
    public string processInstanceKey { get; set; }
    public string remainingTime { get; set; }
}
