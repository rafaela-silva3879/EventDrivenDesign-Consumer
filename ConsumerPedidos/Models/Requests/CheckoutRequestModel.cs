namespace ConsumerPedidos.Models.Requests
{
    public class CheckoutRequestModel
    {
        public decimal? Total { get; set; }
        public CustomerRequestModel? Customer { get; set; }
        public CreditCardRequestModel? CreditCard { get; set; }
    }
}
