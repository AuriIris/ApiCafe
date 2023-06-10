namespace api.Models;



    public class PedidoView
    {
        public int Id { get; set; }
        public int MesaId { get; set; }
        public Mesa? Mesa { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int Estado { get; set; }
        public decimal PrecioTotal { get; set; }
        public string Fecha { get; set; }

        public PedidoView()
        {
        }

        public PedidoView(Pedido pedido, Mesa mesa, Usuario usuario, decimal precioTotal, string fecha)
        {
            Id = pedido.Id;
            MesaId = pedido.MesaId;
            Mesa = mesa;
            UsuarioId = pedido.UsuarioId;
            Usuario = usuario;
            Estado = pedido.Estado;
            PrecioTotal = precioTotal;
            Fecha = fecha;
        }
    }


