namespace api.Models;
public class MesaView
{
    public int Id  { get; set; }
    public int SucursalId  { get; set; }
    
    public Sucursal? sucursal { get; set; }
    public int Numero { get; set; }
    public int Estado { get; set; }

    public MesaView()
    {
    }

    public MesaView(Mesa mesa)
    {
        Id  = mesa.Id ;
        SucursalId  = mesa.SucursalId ;
        sucursal=mesa.sucursal;
        Numero = mesa.Numero;
        Estado = mesa.Estado;
    }
}