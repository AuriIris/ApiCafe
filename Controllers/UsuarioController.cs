
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Models;
using Google.Apis.Auth;
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
    
   [AllowAnonymous]
[HttpPost("verify")]
public async Task<ActionResult> Verify()
{
    
    // Obtiene el token de autenticación de los encabezados de la solicitud
    string authorizationHeader = Request.Headers["Authorization"].ToString();
    var token = string.Empty;
    if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
    {
        token = authorizationHeader.Substring(7);
    }


    // Verifica el token de autenticación de Google
    var payload = await VerifyGoogleTokenId(token);
    if (payload == null)
    {
        return BadRequest("Invalid token");
    }


    
        // Obtener el ID de usuario único
        var userId = payload.Subject;
        Console.WriteLine($"ID de usuario: {userId}");

        // Obtener el correo electrónico del usuario
        var email = payload.Email;
        Console.WriteLine($"Correo electrónico: {email}");

        // Obtener el nombre completo del usuario
        var fullName = payload.Name;
        Console.WriteLine($"Nombre completo: {fullName}");

        // Obtener la URL de la imagen de perfil del usuario
        var profilePictureUrl = payload.Picture;
        Console.WriteLine($"URL de la imagen de perfil: {profilePictureUrl}");
    
    ///generar token

    var propietario = _context.Usuario.FirstOrDefault(x => x.Mail == payload.Email);
        if (propietario == null)
        {
            return NotFound();
        }

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["TokenAuthentication:SecretKey"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, propietario.Mail),
                new Claim("Id", propietario.Id.ToString()),
                new Claim(ClaimTypes.Role, "Propietario")
            };

        var tokenLocal = new JwtSecurityToken(
            issuer: _config["TokenAuthentication:Issuer"],
            audience: _config["TokenAuthentication:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(60),
            signingCredentials: credentials
        );

        return Ok(new JwtSecurityTokenHandler().WriteToken(tokenLocal));
}

public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenId(string token)
{
    Console.WriteLine(token);

    try
    {
        // Verifica el token de autenticación de Google y obtiene el payload (información) del token
        GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(token);

        return payload;
    }
    catch (System.Exception)
    {
        Console.WriteLine("invalid google token");
    }

    return null;
}


}

