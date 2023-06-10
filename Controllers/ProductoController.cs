
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.JWT;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductoController : ControllerBase
{
        private readonly DataContext _context;
         private readonly IConfiguration _config;

        public ProductoController(DataContext contexto, IConfiguration config)
        {
        _context = contexto;
        _config = config;
        }

        [HttpGet()]
        public IActionResult GetProductos()
        {
           var productos = _context.Producto.ToList();
            if (productos == null || productos.Count == 0)
            {
                return NotFound();
            }

            return Ok(productos);
        }

    [HttpGet("{id}")]
        public IActionResult obtenerProducto(int id)
        {
            var producto = _context.Producto.Find(id);
            if (producto == null)
            {
                return NotFound();
            }

            return Ok(producto);
        }
    [HttpPut("{id}")]
    public IActionResult ActualizarProducto(int id, [FromBody] Producto nuevoProducto)
    {
        if (id == 0)
            {
                // El ID es 0, lo cual indica que se debe crear un nuevo producto
                _context.Producto.Add(nuevoProducto);
                _context.SaveChanges();

                return Ok(nuevoProducto);
            }

            // El ID no es 0, se intenta buscar el producto existente
            var producto = _context.Producto.FirstOrDefault(p => p.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            // Actualizar los datos del producto existente 
            producto.Nombre = nuevoProducto.Nombre;
            producto.Precio = nuevoProducto.Precio;
            producto.Categoría = nuevoProducto.Categoría;

            _context.Update(producto);
            _context.SaveChanges();

            return Ok(producto);
    }
 [HttpGet("Pedido/{id}")]
public IActionResult obtenerProductosDePedido(int id)
{
    // Buscar los detalles de pedido que tengan el mismo ID de pedido
    var detallesPedido = _context.DetallePedido
        .Where(dp => dp.PedidoId == id)
        .ToList();

    if (detallesPedido.Count == 0)
    {
        return NotFound();
    }

    // Obtener los IDs de los productos de los detalles de pedido
    var idProductos = detallesPedido
        .Select(dp => dp.ProductoId)
        .ToList();

    // Buscar los productos correspondientes a los IDs obtenidos
    var productos = _context.Producto
        .Where(p => idProductos.Contains(p.Id))
        .ToList();

    // Asociar cada detalle de pedido con su producto correspondiente
    foreach (var detalle in detallesPedido)
    {
        detalle.producto = productos.FirstOrDefault(p => p.Id == detalle.ProductoId);
    }

    return Ok(detallesPedido);
}



}

