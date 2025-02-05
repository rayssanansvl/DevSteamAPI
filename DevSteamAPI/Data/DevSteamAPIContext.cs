using DevSteamAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevSteamAPI.Data
{
    public class DevSteamAPIContext : IdentityDbContext
    {
        // Método Construtor
        public DevSteamAPIContext(DbContextOptions<DevSteamAPIContext> options) : base(options)
        { }

        //Definir as tabelas do banco de dados
        public DbSet<Jogo> Jogos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        //Sobrescrever o metodo OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);
            modelbuilder.Entity<Jogo>().ToTable("Jogos");
            modelbuilder.Entity<Categoria>().ToTable("Categorias");
        }
    }
}
