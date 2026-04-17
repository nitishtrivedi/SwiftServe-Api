namespace SwiftServe_API.DTOs
{
    public class CreatePaymentDto
    {
        public int OrderId { get; set; }
    }

    public class VerifyPaymentDto
    {
        public int OrderId { get; set; }
        public string RazorpayOrderId { get; set; }
        public string RazorpayPaymentId { get; set; }
        public string RazorpaySignature { get; set; }
    }
}
