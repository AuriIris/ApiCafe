
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
public class UsuarioController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration _config;
    public UsuarioController(DataContext contexto, IConfiguration config)
    {
        _context = contexto;
        _config = config;
    }

    [HttpGet("{id}")]
    public Usuario? Get(int id)
    {
        return _context.Usuario.Find(id);
    }
    [HttpPost("login")]
    public IActionResult Login(LoginView login)
    {
        var propietario = _context.Usuario.FirstOrDefault(x => x.Mail == login.Mail);
        if (propietario == null)
        {
            return NotFound();
        }

        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: login.Clave,
            salt: Encoding.ASCII.GetBytes(_config["Salt"]),
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 1000,
            numBytesRequested: 256 / 8));

        if (propietario.Clave != hashed)
        {
            return BadRequest("Contraseña incorrecta");
        }

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["TokenAuthentication:SecretKey"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, propietario.Mail),
                new Claim("Id", propietario.Id.ToString()),
                new Claim(ClaimTypes.Role, "Propietario")
            };

        var token = new JwtSecurityToken(
            issuer: _config["TokenAuthentication:Issuer"],
            audience: _config["TokenAuthentication:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(60),
            signingCredentials: credentials
        );

        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
    }
    [HttpGet("perfil")]
    [Authorize]
    public IActionResult ObtenerPerfil()
    {
        var mail = User.Identity.Name;
        var usuario = _context.Usuario.FirstOrDefault(x => x.Mail == mail);
        if (usuario == null)
        {
            return NotFound();
        }
        return Ok(usuario);
    }

    [HttpPut("{id}")]
    [Authorize]
    public IActionResult ActualizarPerfil(int id,[FromBody] UsuarioView nuevo)
    {
        var mail = User.Identity.Name;
        var usuario = _context.Usuario.FirstOrDefault(x => x.Mail == mail);
        if (usuario == null)
        {
            return NotFound();
        }
        if (!string.IsNullOrEmpty(nuevo.Mail))
        {
            usuario.Mail = nuevo.Mail;
            usuario.Nombre = nuevo.Nombre;
            usuario.Rol = nuevo.Rol;
            usuario.Permiso = nuevo.Permiso;
        }

        _context.Update(usuario);
        _context.SaveChanges();
        var perfilActualizado = _context.Usuario.FirstOrDefault(x => x.Mail == mail);
        return Ok(perfilActualizado);
    }



}

