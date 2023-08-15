namespace ConsumerPedidos.Models.Responses
{
    public class OrderResponseModel
    {
        public Guid? Id { get; set; }
        public DateTime? Date { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerCpf { get; set; }
    }
}