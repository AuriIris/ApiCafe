using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.JWT;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class DetallePedidoController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration _config;

    public DetallePedidoController(DataContext contexto, IConfiguration config)
    {
        _context = contexto;
        _config = config;
    }
    [HttpPost()]
    [HttpPost()]
    [HttpPost()]
    public IActionResult agregarProductoaPedido([FromBody] DetallePedido nuevoDetallePedido)
    {
        _context.DetallePedido.Add(nuevoDetallePedido);
        _context.SaveChanges();

        // Obtener el pedido correspondiente
        

            // Recuperar los detalles de pedido asociados al pedido
            List<DetallePedido> detallesPedido = _context.DetallePedido.Where(d => d.PedidoId ==nuevoDetallePedido.PedidoId).ToList();

            if (detallesPedido != null && detallesPedido.Count > 0)
            {
                decimal precioTotal = 0;

                foreach (DetallePedido detalle in detallesPedido)
                {
                    
                        Console.WriteLine("Precio del producto: " + detalle.Precio); // Mensaje de depuración

                        precioTotal += detalle.Precio ;
                    
                }

                Console.WriteLine("Precio total calculado: " + precioTotal); // Mensaje de depuración
                
                var pedido = _context.Pedido.Find(nuevoDetallePedido.PedidoId);
                pedido.PrecioTotal = precioTotal;
                _context.SaveChanges();
            }


        

        return Ok(nuevoDetallePedido);
    }


}