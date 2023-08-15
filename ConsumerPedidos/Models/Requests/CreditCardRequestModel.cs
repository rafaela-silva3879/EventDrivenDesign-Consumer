namespace ConsumerPedidos.Models.Requests
{
    public class CreditCardRequestModel
    {
        public string? CardNumber { get; set; }
        public string? CardHolderName { get; set; }
        public int? ExpirationMonth { get; set; }
        public int? ExpirationYear { get; set; }
        public int? SecurityCode { get; set; }
    }
}
