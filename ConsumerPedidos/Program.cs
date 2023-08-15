using ConsumerPedidos.Consumers;
using ConsumerPedidos.Messages;
using ConsumerPedidos.Repositories;
using ConsumerPedidos.Services;
using ConsumerPedidos.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


builder.Services.Configure<MessageBrokerSettings>
        (builder.Configuration.GetSection("MessageBrokerSettings"));
builder.Services.Configure<CheckoutSettings>
        (builder.Configuration.GetSection("CheckoutSettings"));
builder.Services.Configure<MailSettings>
        (builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<CheckoutService>();
builder.Services.AddTransient<PedidosMessage>();
//configurando a injeção de dependencia da classe PedidosRepository
//capturando e passando para o construtor da classe a string de
//conexão do banco de dados..
builder.Services.AddTransient(map
=> new PedidosRepository
(builder.Configuration.GetConnectionString("Conexao")));
//A classe PedidosConsumer é um serviço que ficará executando
//em segundo plano enquanto o projeto estiver 'rodando'
builder.Services.AddHostedService<PedidosConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
