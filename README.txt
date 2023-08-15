• Fluxo de trabalho:
o Primeiro, o consumer lê os registros gravados na fila do RabbitMQ (Payloads)
o Em seguida, cada item gravado na fila é enviado para a API de pagamentos de forma a pagar o pedido
▪ Primeiro o consumer autentica na API de pagamentos e obtém um TOKEN de acesso
▪ Depois é enviado a requisição para pagamento do pedido (usando o token de autenticação obtido)
o Por último, o sistema grava o resultado do pagamento no banco de dados.



1 - O método ExecuteAsync é chamado automaticamente quando o sistema é executado. Ele faz parte do ciclo de vida de um serviço de fundo (background service) em aplicações .NET Core.

O ExecuteAsync é o método principal de um serviço de fundo. Quando você implementa um serviço de fundo herdando da classe BackgroundService, o método ExecuteAsync é responsável por executar a lógica do serviço continuamente, até que o aplicativo seja parado ou o serviço seja explicitamente parado.

2 - O método RealizarPagamento utiliza informações do PedidoPayloadModel recebido como parâmetro para preencher um objeto CheckoutRequestModel. Esse objeto contém detalhes relevantes para o pagamento, como o valor total do pedido, informações do cliente e informações do cartão de crédito para processar o pagamento.

3 - Em seguida, o método chama o serviço de checkout _checkoutService.Post para realizar efetivamente o pagamento usando as informações do pedido. O token de acesso obtido na chamada anterior ao serviço de autenticação _authService é passado como parâmetro para garantir que a solicitação de pagamento seja autenticada corretamente.

4 - Por fim, o método retorna um objeto CheckoutResponseModel, que contém informações sobre o status do pagamento, como se o pagamento foi bem-sucedido ou não.



5 - Para executar o sistema, crie uma fila com o nome de "pedidos" em https://www.cloudamqp.com/ e cole a url da fila  no appsetting.json, em MessageBrokerSettings / url

Cole a conexão do banco de dados da ApiPedidos no Connectionstrings / Conexao

