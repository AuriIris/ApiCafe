namespace api.Models;
public class ProductoView
{
    public int Id  { get; set; }
    public string? Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Categoría { get; set; }
    public int Estado{get;set;}

    public ProductoView()
    {
    }

    public ProductoView(Producto producto)
    {
        Id  = producto.Id ;
        Nombre = producto.Nombre;
        Precio = producto.Precio;
        Categoría = producto.Categoría;
        Estado=producto.Estado;
    }
}