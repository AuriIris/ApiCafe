
using api.Models;
using Microsoft.EntityFrameworkCore;

public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{

		}
		
	
		public DbSet<Usuario> Usuario { get; set; }
		public DbSet<Mesa> Mesa { get; set; }
		public DbSet<Producto> Producto { get; set; }
		public DbSet<Sucursal> Sucursal { get; set; }
		public DbSet<DetallePedido> DetallePedido { get; set; }
		public DbSet<Pedido> Pedido { get; set; }

		
	}
    
