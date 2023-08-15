namespace ConsumerPedidos.Settings
{
    /// <summary>
    /// Message Broker do RabbitMQ */
    /// </summary>
    public class MessageBrokerSettings
    {
        public string? Url { get; set; }
        public string? Queue { get; set; }
    }
}
