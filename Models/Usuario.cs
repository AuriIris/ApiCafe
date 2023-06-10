namespace api.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string? Mail { get; set; }
        public string? Nombre { get; set; }
        public string? Clave { get; set; }
        public int Rol { get; set; }
        public int Permiso { get; set; }
    }
}
