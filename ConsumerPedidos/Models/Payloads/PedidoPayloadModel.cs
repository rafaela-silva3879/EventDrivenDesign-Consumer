using Newtonsoft.Json;

namespace ConsumerPedidos.Models.Payloads
{
    public class PedidoPayloadModel
    {
        [JsonProperty("DetalhesPedido")]
        public string DetalhesPedidoString { get; set; }
        public Guid EventId { get; set; }
        public DateTime? CreatedAt { get; set; }
        [JsonIgnore]
        public DetalhesPedido DetalhesPedido
        {
            get
            {
                return JsonConvert.DeserializeObject
                            <DetalhesPedido>(DetalhesPedidoString);
            }
        }
    }
    public class Cliente
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Cpf { get; set; }
        public Endereco? Endereco { get; set; }
    }
    public class Endereco
    {
        public string? Logradouro { get; set; }
        public string? Complemento { get; set; }
        public string? Numero { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Cep { get; set; }
    }
    public class Cobranca
    {
        public string? NumeroCartao { get; set; }
        public string? NomeImpressoNoCartao { get; set; }
        public int? MesValidade { get; set; }
        public int? AnoValidade { get; set; }
        public int? CodigoSeguranca { get; set; }
        public decimal? ValorCobranca { get; set; }
    }
    public class ItemPedido
    {
        public string? CodigoItem { get; set; }
        public string? Nome { get; set; }
        public decimal? Preco { get; set; }
        public int? Quantidade { get; set; }
    }
    public class DetalhesPedido
    {
        public DateTime? DataHora { get; set; }
        public decimal? Valor { get; set; }
        public Cliente? Cliente { get; set; }
        public Cobranca? Cobranca { get; set; }
        public List<ItemPedido>? ItensPedido { get; set; }
    }
}
