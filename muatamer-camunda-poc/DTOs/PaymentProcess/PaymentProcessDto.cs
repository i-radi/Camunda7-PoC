namespace muatamer_camunda_poc.DTOs.PaymentProcess;

public class PaymentProcessDto
{
    public int GroupId { get; set; }
    public int BLValidationResult { get; set; }
    public int MinistryApprovalResult { get; set; }
    public int CreateVoucherResult { get; set; }
    public int PaymentSystemResult { get; set; }
    public int IsFullPaymentResult { get; set; }
    public bool IsManualNationality { get; set; }
    public bool isManualPayment { get; set; }
    public string processInsanceId { get; set; }
    public string Message { get; set; }
    public int MuatamersCount { get; set; }
}




