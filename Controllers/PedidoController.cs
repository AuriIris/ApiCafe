
using System.IdentityModel.Tokens.Jwt;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.JWT;
using Microsoft.EntityFrameworkCore;


namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class PedidoController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration _config;

    public PedidoController(DataContext contexto, IConfiguration config)
    {
        _context = contexto;
        _config = config;
    }


    [HttpGet]
    [Authorize]
    public IActionResult GetPedidos()
    {
        // Obtener el nombre de usuario del usuario logueado
        var usuario = User.Identity.Name;

        // Filtrar los pedidos por el usuario logueado
        var pedidos = _context.Pedido
            .Join(_context.Mesa,
                pedido => pedido.MesaId,
                mesa => mesa.Id,
                (pedido, mesa) => new { Pedido = pedido, Mesa = mesa })
            .Join(_context.Usuario,
                pedidoMesa => pedidoMesa.Pedido.UsuarioId,
                usuario => usuario.Id,
                (pedidoMesa, usuario) => new PedidoView
                {
                    Id=pedidoMesa.Pedido.Id,
                    MesaId=pedidoMesa.Pedido.MesaId,
                    UsuarioId=pedidoMesa.Pedido.UsuarioId,
                    Estado=pedidoMesa.Pedido.Estado,
                    PrecioTotal=pedidoMesa.Pedido.PrecioTotal,
                    Fecha=pedidoMesa.Pedido.Fecha,
                    Mesa = pedidoMesa.Mesa,
                    Usuario = usuario,
                })
            .Where(pedidoView => pedidoView.Usuario.Mail == usuario)  // Filtrar por el email del usuario logueado
            .ToList();

        if (pedidos == null || pedidos.Count == 0)
        {
            return NotFound();
        }

        return Ok(pedidos);
    }



    [HttpGet("{id}")]
    public IActionResult GetPedido(int id)
    {
        var pedidos = _context.Pedido.Find(id);
        if (pedidos == null)
        {
            return NotFound();
        }

        return Ok(pedidos);
    }


    [HttpPut("{id}")]
    public IActionResult ActualizarPedido(int id, [FromBody] Pedido nuevoPedido)
    {

        var mail = User.Identity.Name;

        var usuario = _context.Usuario.FirstOrDefault(x => x.Mail == mail);
        Console.WriteLine(usuario);
        if (id == 0)
        {   
            Console.WriteLine(nuevoPedido.Fecha);
            nuevoPedido.UsuarioId = usuario.Id;
            _context.Pedido.Add(nuevoPedido);
            _context.SaveChanges();

            return Ok(nuevoPedido);
        }

        // El ID no es 0, se intenta buscar el producto existente
        var pedido = _context.Pedido.FirstOrDefault(p => p.Id == id);
        if (pedido == null)
        {
            return NotFound();
        }
        var mesa = _context.Mesa.FirstOrDefault(m => m.Id == pedido.MesaId);
        if (mesa == null)
        {
            return NotFound();
        }

        // Actualizar los datos del producto existente
        pedido.MesaId = nuevoPedido.MesaId;
        pedido.UsuarioId = usuario.Id;
        pedido.Estado = nuevoPedido.Estado;
        pedido.PrecioTotal = nuevoPedido.PrecioTotal;
        Console.WriteLine(nuevoPedido.Fecha);
        pedido.Fecha = nuevoPedido.Fecha;
        mesa.Estado = 1;

        _context.Update(pedido);
        _context.Update(mesa);
        _context.SaveChanges();

        return Ok(pedido);
    }
  
[HttpGet("Cierre/{fechas}")]
public IActionResult GetCierre(string fechas)
{
    fechas = fechas.Replace("%2F", "-");
    Console.WriteLine(fechas);
    string[] componentes = fechas.Split(' ');

    string fechaDesde = componentes[0];
    string horaDesde = componentes[1] + ":00";
    string fechaHasta = componentes[2];
    string horaHasta = componentes[3] + ":00";

    Console.WriteLine("SEGUNDO: " + fechaDesde + horaDesde + fechaDesde + fechaHasta);//SEGUNDO: 04-06-202322:32:0004-06-202309-06-2023

    DateTime fechaDesdeHoraDesde = DateTime.ParseExact(fechaDesde + " " + horaDesde, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
    DateTime fechaHastaHoraHasta = DateTime.ParseExact(fechaHasta + " " + horaHasta, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
    
    Console.WriteLine(fechaDesdeHoraDesde.ToString("dd-MM-yyyy HH:mm:ss"));
    Console.WriteLine(fechaHastaHoraHasta.ToString("dd-MM-yyyy HH:mm:ss"));


    

    var usuario = User.Identity.Name;

    // Filtrar los pedidos por el usuario logueado y el rango de fechas
    var pedidos = _context.Pedido
        .Join(_context.Mesa,
            pedido => pedido.MesaId,
            mesa => mesa.Id,
            (pedido, mesa) => new { Pedido = pedido, Mesa = mesa })
        .Join(_context.Usuario,
            pedidoMesa => pedidoMesa.Pedido.UsuarioId,
            usuario => usuario.Id,
            (pedidoMesa, usuario) => new PedidoView
            {
                Id = pedidoMesa.Pedido.Id,
                MesaId = pedidoMesa.Pedido.MesaId,
                UsuarioId = pedidoMesa.Pedido.UsuarioId,
                Estado = pedidoMesa.Pedido.Estado,
                PrecioTotal = pedidoMesa.Pedido.PrecioTotal,
                Fecha = pedidoMesa.Pedido.Fecha,
                Mesa = pedidoMesa.Mesa,
                Usuario = usuario,
            })
        .ToList();

    Console.WriteLine(pedidos.Count);
        var pedidosFiltrados = pedidos.Where(pedidoView => pedidoView.Usuario.Mail == usuario &&
        DateTime.ParseExact(pedidoView.Fecha, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture) >= fechaDesdeHoraDesde &&
        DateTime.ParseExact(pedidoView.Fecha, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture) <= fechaHastaHoraHasta)
        .ToList();

    Console.WriteLine(pedidosFiltrados.Count);
    if (pedidosFiltrados == null || pedidosFiltrados.Count == 0)
    {
        return Ok("$0,00");
    }

    decimal totalPedidos = pedidosFiltrados.Sum(p => p.PrecioTotal);
    string totalPedidosString = totalPedidos.ToString();
    Console.WriteLine(totalPedidosString);
    return Ok("$"+totalPedidosString);
}







}

