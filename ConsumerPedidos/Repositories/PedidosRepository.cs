using Dapper;
using System.Data.SqlClient;

namespace ConsumerPedidos.Repositories
{
    public class PedidosRepository
    {
        private readonly string? _connectionString;
        public PedidosRepository(string? connectionString)
        {
            _connectionString = connectionString;
        }
        public void Update(Guid? id, int? status,
                            string? message, Guid? transactionId)
        {
            var query = @"
                UPDATE Pedido SET
                Status = @status,
                DetalhesPagamento = @message,
                ProtocoloPagamento = @transactionId
                WHERE
                Id = @id";
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Execute(query, new
                    {
                        id,
                        status,
                        message,
                        transactionId
                    });
                }
        }
    }
}