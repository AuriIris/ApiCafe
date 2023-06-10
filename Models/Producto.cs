namespace api.Models;
public class Producto
{
    public int Id  { get; set; }
    public string? Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Categoría { get; set; }
    public int Estado{get;set;}
}