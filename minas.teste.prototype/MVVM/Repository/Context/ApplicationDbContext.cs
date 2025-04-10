using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using minas.teste.prototype.MVVM.Model.Concrete;

namespace minas.teste.prototype.MVVM.Repository
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Vazao_bomba> Pumps { get; set; }
        public DbSet<Pressao_bomba> Pressures { get; set; }
        public DbSet<Rotacao_bomba> Rotations { get; set; }
        public DbSet<Pilotagem_bomba> Valves { get; set; }
        public DbSet<Dreno_bomba> Drains { get; set; }
        public DbSet<Temperatura_bomba> Temperatures { get; set; }
        public DbSet<SensorCalibracoes> SensorCalibracoes { get; set; }

        private const string DatabaseName = "supervisory_SYSMT_data.db";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={DatabaseName}");
                VerifyDatabaseCreation();
            }
        }

        private void VerifyDatabaseCreation()
        {
            var databasePath = Path.Combine(Directory.GetCurrentDirectory(), DatabaseName);

            if (!File.Exists(databasePath))
            {
                this.Database.EnsureCreated();
                InitializeDatabase();
            }
        }

        private void InitializeDatabase()
        {
            // Executar scripts iniciais se necessário
            this.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
            this.Database.ExecuteSqlRaw("PRAGMA foreign_keys=ON;");
        }
        public void EnsureDatabaseCreatedAndMigrated()
        {
            var databasePath = Path.Combine(Directory.GetCurrentDirectory(), DatabaseName);
            bool dbExisted = File.Exists(databasePath);

            // Garante que o modelo do banco de dados seja criado SE não existir
            // Ou aplica migrações pendentes se o banco já existir e você usar Migrations
            this.Database.EnsureCreated(); // Ou this.Database.Migrate(); se usar Migrations

            if (!dbExisted || !IsDatabaseInitialized()) // Verifica se precisa inicializar
            {
                InitializeDatabase();
            }
        }
        private bool IsDatabaseInitialized()
        {
            try
            {
                // Tenta verificar uma configuração ou tabela que a inicialização garante
                var journalMode = this.Database.ExecuteSqlRaw("PRAGMA journal_mode;");
                // Se não lançar exceção, provavelmente está ok (pode refinar a verificação)
                return true;
            }
            catch
            {
                return false;
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuração global para todas as entidades
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Encontrar a propriedade Id em cada entidade
                var idProperty = entityType.FindProperty("Id");

                if (idProperty != null && idProperty.ClrType == typeof(int))
                {
                    // Configurar como chave primária
                    entityType.SetPrimaryKey(idProperty);

                    // Configurar autoincremento específico para SQLite
                    idProperty.ValueGenerated = ValueGenerated.OnAdd;
                    idProperty.SetAnnotation("Sqlite:Autoincrement", true);
                }
            }

            // Configurações adicionais se necessário
            modelBuilder.Entity<Vazao_bomba>().Property(p => p.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Pressao_bomba>().Property(p => p.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Pilotagem_bomba>().Property(p => p.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Dreno_bomba>().Property(p => p.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Rotacao_bomba>().Property(p => p.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Temperatura_bomba>().Property(p => p.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<SensorCalibracoes>().Property(p => p.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }

   
    }
}
