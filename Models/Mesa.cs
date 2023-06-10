using System.ComponentModel.DataAnnotations.Schema;
namespace api.Models;
public class Mesa
{
    public int Id { get; set; }
    public int SucursalId { get; set; }
    [NotMapped]
    public Sucursal? sucursal { get; set; }
    public int Numero { get; set; }
    public int Estado { get; set; }
}