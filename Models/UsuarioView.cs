namespace api.Models
{
    public class UsuarioView
    {
        public int Id { get; set; }
        public string? Mail {get;set;}
        public string? Clave { get; set; }
        public string? Nombre { get; set; }
        public int Rol { get; set; }
        public int Permiso { get; set; }

        public UsuarioView()
        {
        }

        public UsuarioView(Usuario usuario)
        {
            Id = usuario.Id;
            Mail=usuario.Mail;
            Clave=usuario.Clave;
            Nombre = usuario.Nombre;
            Rol = usuario.Rol;
            Permiso = usuario.Permiso;
        }
    }
}
