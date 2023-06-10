using System.ComponentModel.DataAnnotations.Schema;
namespace api.Models;
public class DetallePedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }

    [NotMapped]
    public Pedido? pedido { get; set; }

    public int ProductoId { get; set; }
    [NotMapped]
    public Producto? producto { get; set; }
    public int Cantidad { get; set; }
    public decimal Precio { get; set; }
}