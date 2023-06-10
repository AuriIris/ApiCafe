using System.ComponentModel.DataAnnotations.Schema;
namespace api.Models;
public class Pedido
{
    public int Id { get; set; }
    public int MesaId { get; set; }
    [NotMapped]
    public Mesa? mesa { get; set; }
    
    public int UsuarioId { get; set; }
    [NotMapped]
    public Usuario? usuario { get; set; }
    public int Estado { get; set; }
    public Decimal PrecioTotal { get; set; }
    public String Fecha { get; set; }
   
}
