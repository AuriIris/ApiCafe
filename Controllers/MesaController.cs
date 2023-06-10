
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
public class MesaController : ControllerBase
{
        private readonly DataContext _context;
         private readonly IConfiguration _config;

        public MesaController(DataContext contexto, IConfiguration config)
        {
        _context = contexto;
        _config = config;
        }

        [HttpGet()]
        public IActionResult GetMesas()
        {
           var mesas = _context.Mesa.ToList();
            if (mesas == null || mesas.Count == 0)
            {
                return NotFound();
            }

            return Ok(mesas);
        }

        [HttpGet("{id}")]
        public IActionResult GetMesa(int id)
        {
            var mesas = _context.Mesa.Find(id);
            if (mesas == null)
            {
                return NotFound();
            }

            return Ok(mesas);
        }
        [HttpPut("{id}")]
        public IActionResult ActualizarMesa(int id, [FromBody] Mesa nuevoMesa)
        {
           if (id == 0)
            {
                // El ID es 0, lo cual indica que se debe crear un nuevo producto
                _context.Mesa.Add(nuevoMesa);
                _context.SaveChanges();

                return Ok(nuevoMesa);
            }

            // El ID no es 0, se intenta buscar el producto existente
            var mesa = _context.Mesa.FirstOrDefault(p => p.Id == id);
            if (mesa == null)
            {
                return NotFound();
            }

            // Actualizar los datos del producto existente 
            mesa.Numero = nuevoMesa.Numero;
            mesa.SucursalId = nuevoMesa.SucursalId;
            mesa.Estado = nuevoMesa.Estado;

            _context.Update(mesa);
            _context.SaveChanges();

            return Ok(mesa);
        }



}

