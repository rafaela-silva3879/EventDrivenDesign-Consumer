using ConsumerPedidos.Services;
using ConsumerPedidos.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Diagnostics;
using ConsumerPedidos.Models.Payloads;
using ConsumerPedidos.Models.Requests;
using ConsumerPedidos.Models.Responses;
using Newtonsoft.Json;
using ConsumerPedidos.Messages;
using ConsumerPedidos.Repositories;

namespace ConsumerPedidos.Consumers
{
    public class PedidosConsumer : BackgroundService
    {
        private readonly IServiceProvider? _serviceProvider;
        private readonly IOptions<MessageBrokerSettings>?
        _messageBrokerSettings;
        private readonly IOptions<CheckoutSettings>? _checkoutSettings;
        private readonly CheckoutService? _checkoutService;
        private readonly AuthService? _authService;
        private readonly PedidosRepository? _pedidosRepository;
        private readonly PedidosMessage? _pedidosMessage;
        private IConnection? _connection;
        private IModel? _model;
        public PedidosConsumer(IServiceProvider? serviceProvider,
        IOptions<MessageBrokerSettings>? messageBrokerSettings, IOptions<CheckoutSettings>? checkoutSettings, CheckoutService? checkoutService, AuthService? authService, PedidosRepository? pedidosRepository, PedidosMessage? pedidosMessage)
        {
            _serviceProvider = serviceProvider;
            _messageBrokerSettings = messageBrokerSettings;
            _checkoutSettings = checkoutSettings;
            _checkoutService = checkoutService;
            _authService = authService;
            _pedidosRepository = pedidosRepository;
            _pedidosMessage = pedidosMessage;
            #region Conexão com o servidor da mensageria
            var factory = new ConnectionFactory
            { Uri = new Uri(_messageBrokerSettings.Value.Url) };
            _connection = factory.CreateConnection();
            _model = _connection.CreateModel();
            _model.QueueDeclare(
            queue: _messageBrokerSettings.Value.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
            );
            #endregion
        }
        /// <summary>
        /// Método implementado da classe BackgroundService para executar processos
        /// continuamente no servidor enquanto o projeto estiver rodando
        /// </summary>
        protected override async Task ExecuteAsync
        (CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_model);
            consumer.Received += async (sender, args) =>
            {
                //lendo o conteúdo do Payload da fila
                var contentArray = args.Body.ToArray();
                var contentString = Encoding.UTF8.GetString(contentArray);
                //processando este conteudo
                using (var scope = _serviceProvider.CreateScope())
                {
                    //deserializar o conteudo lido da fila
                    var pedidoPayloadModel = JsonConvert.DeserializeObject
                    <PedidoPayloadModel>(contentString);
                    
                    //realizar o pagamento
                    var result = await RealizarPagamento(pedidoPayloadModel);
                    
                    //gravando o resultado do pedido no banco de dados
                    int status = result.Status.Equals("success") ? 2 : 3;
                    
                    _pedidosRepository.Update(pedidoPayloadModel.EventId,
                    status, result.Message, result.TransactionId);

                    //retirar a mensagem da fila
                    _model.BasicAck(args.DeliveryTag, false);

                    //enviando email..
                    var to = result.Order.CustomerEmail;
                    var subject = $"Pagamento do Pedido:{pedidoPayloadModel.EventId}";
                    var body = JsonConvert.SerializeObject(result, Formatting.Indented);
                    _pedidosMessage.Send(to, subject, body);
                }
            };
            _model.BasicConsume
            (_messageBrokerSettings.Value.Queue, false, consumer);
        }
        /// <summary>
        /// Método que envia um pagamento para a API,
        /// os dados enviados são referentes
        /// ao pedido que foi lido da fila do RabbitMQ
        /// </summary>
        private async Task<CheckoutResponseModel> RealizarPagamento
        (PedidoPayloadModel model)
        {
            //Realizando a autenticação na API de pagamentos
            //para obter um Token
            var authResponse = await _authService.Post(new AuthRequestModel
            {
                Client = _checkoutSettings.Value.Client,
                Password = _checkoutSettings.Value.Password
            });
            //Criando os dados para fazer a chamada para o
            //endpoint de pagamento (checkout)
            var checkoutRequest = new CheckoutRequestModel
            {
                Total = model.DetalhesPedido.Valor,
                Customer = new CustomerRequestModel
                {
                    Name = model.DetalhesPedido.Cliente.Nome,
                    Email = model.DetalhesPedido.Cliente.Email,
                    Cpf = model.DetalhesPedido.Cliente.Cpf
                },
                CreditCard = new CreditCardRequestModel
                {
                    CardNumber = model.DetalhesPedido.Cobranca.NumeroCartao,
                    CardHolderName = model.DetalhesPedido
            .Cobranca.NomeImpressoNoCartao,
                    ExpirationMonth = model.DetalhesPedido
            .Cobranca.MesValidade,
                    ExpirationYear = model.DetalhesPedido
            .Cobranca.AnoValidade,
                    SecurityCode = model.DetalhesPedido
            .Cobranca.CodigoSeguranca
                }
            };
            //Fazendo o processamento do pagamento, enviando o token
            //o usuário autenticado no passo anterior
            return await _checkoutService.Post(checkoutRequest,
            authResponse.AccessToken);
        }
    }
}