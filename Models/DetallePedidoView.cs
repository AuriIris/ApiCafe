namespace api.Models;
public class DetallePedidoView
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
     public Pedido? pedido { get; set; }
    public int ProductoId { get; set; }
     public Producto? producto { get; set; }
    public int Cantidad { get; set; }
    public decimal Precio { get; set; }

    public DetallePedidoView()
    {
    }

    public DetallePedidoView(DetallePedido detallePedido, Producto producto)
    {
        Id = detallePedido.Id;
        PedidoId = detallePedido.PedidoId;
        pedido=detallePedido.pedido;
        ProductoId = detallePedido.ProductoId;
        producto= detallePedido.producto;
        Cantidad = detallePedido.Cantidad;
        Precio = detallePedido.producto.Precio* detallePedido.Cantidad;
    }
}
