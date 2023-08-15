namespace ConsumerPedidos.Models.Responses
{
    public class CheckoutResponseModel
    {
        public Guid? TransactionId { get; set; }
        public string? Status { get; set; }
        public string? Message { get; set; }
        public decimal? Total { get; set; }
        public OrderResponseModel? Order { get; set; }
    }
}