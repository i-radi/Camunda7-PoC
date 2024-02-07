namespace muatamer_camunda_poc.DTOs;

public class PaymentProcessDto
{
    public int GroupId { get; set; }
    public int BLValidationResult { get; set; }
    public int MinistryApprovalResult { get; set; }
    public int CreateVoucherResult { get; set; }
    public int PaymentSystemResult { get; set; }
    public int IsFullPaymentResult { get; set; }
}




