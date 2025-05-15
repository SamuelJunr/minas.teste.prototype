using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore.Metadata;
using minas.teste.prototype.MVVM.Model.Concrete;
using System.IO;

namespace minas.teste.prototype.MVVM.Repository.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Sessao> Sessoes { get; set; }
        public DbSet<Etapa> Etapas { get; set; }
        public DbSet<Bombas> Bombas { get; set; }
        public DbSet<Motor> Motores { get; set; }
        public DbSet<Eletrovalvulas> Eletrovalvulas { get; set; }
        public DbSet<Comandos> Comandos { get; set; }
        public DbSet<Direcao> Direcoes { get; set; }
        public DbSet<Cilindros> Cilindros { get; set; }
        public DbSet<SensorDataDB> SensorDataDB { get; set; }
        public DbSet<Conexao> Conexoes { get; set; }
        public DbSet<Monitoramento> Monitoramentos { get; set; }
        public DbSet<TempoExecucao> TemposExecucao { get; set; }
        public DbSet<Relatorio> Relatorios { get; set; }
        public DbSet<Calibracao> Calibracoes { get; set; }
        public DbSet<SensorConfiguracao> SensoresConfiguracao { get; set; }
        public DbSet<Modulo> Modulos { get; set; }
        public DbSet<Imagem> Imagens { get; set; }

        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            modelBuilder.Entity<Empresa>()
                .HasIndex(e => e.CNPJ)
                .IsUnique();

            base.OnModelCreating(modelBuilder); // Boa prática chamar o método base

            // --- Restrições Únicas ---

            // Restrição única para Empresa.CNPJ
            // Schema: CNPJ TEXT UNIQUE NOT NULL
            modelBuilder.Entity<Empresa>(entity =>
            {
                entity.HasIndex(e => e.CNPJ)
                      .IsUnique();
            });
            modelBuilder.Entity<Calibracao>(entity =>
            {
                entity.HasOne(c => c.Modulo)    // Propriedade de navegação
                      .WithMany()               // Se Modulo tem uma coleção de Calibracoes
                      .HasForeignKey(c => c.ModuloID)
                      .OnDelete(DeleteBehavior.Restrict); // Define o comportamento de exclusão
            });

            modelBuilder.Entity<Imagem>(entity =>
            {
                entity.HasOne(i => i.Relatorio)    // Propriedade de navegação em Imagem
                      .WithOne(r => r.Imagem)      // Propriedade de navegação em Relatorio
                      .HasForeignKey<Imagem>(i => i.RelatorioID); // Define a FK em Imagem
            });

            modelBuilder.Entity<Relatorio>(entity =>
            {
                entity.HasOne(r => r.Imagem)
                      .WithOne(i => i.Relatorio)
                      .HasForeignKey<Relatorio>(r => r.ImagemID);
            });

            modelBuilder.Entity<Bombas>(entity =>
            {
                // Relacionamento com Etapa
                entity.HasOne(b => b.Etapa)
                      .WithMany() // Se Etapa não tiver uma coleção de Bombas
                      .HasForeignKey(b => b.EtapaID)
                      .OnDelete(DeleteBehavior.Restrict); // Opcional: define o comportamento de exclusão

                // Relacionamento com Sessao (caso necessário)
                entity.HasOne(b => b.Sessao)
                      .WithMany()
                      .HasForeignKey(b => b.SessaoID);
            });

            // Restrição única para Usuario.Name (nome de usuário)
            // Schema: Name TEXT NOT NULL
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(u => u.Name)
                      .IsUnique();
            });

            // Restrição única para "imagem"
            // Assumindo que se refere ao Relatorio.ImagemID ser único.
            // Isso significa que uma Imagem pode ser a imagem principal/capa de no máximo um Relatorio.
            // Relatorio.ImagemID é uma FK para Imagem.ID e é anulável.
            modelBuilder.Entity<Relatorio>(entity =>
            {
                // Para SQLite, um índice único em uma coluna anulável permite múltiplos valores NULL.
                // Se ImagemID estiver definido, ele deve ser único.
                entity.HasIndex(r => r.ImagemID)
                      .IsUnique();
                // Para SQL Server, para permitir múltiplos NULLs em um índice único, um filtro é necessário:
                // .HasFilter("[ImagemID] IS NOT NULL");
                // No SQLite, o filtro não é estritamente necessário para este comportamento com NULLs.
            });


            // --- Configurações de Chaves Estrangeiras e Relacionamentos (Muitas são inferidas por convenção) ---
            // Exemplos explícitos para clareza e casos não cobertos por convenção.

            

            modelBuilder.Entity<SensorConfiguracao>(entity =>
            {
                entity.HasOne(s => s.SensorData)
                      .WithMany() // Se houver navegação inversa
                      .HasForeignKey(s => s.SensorDataID);
            });

            modelBuilder.Entity<SensorConfiguracao>(entity =>
            {
                entity.Ignore(s => s.SensorData);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasOne(u => u.Empresa) // Propriedade de navegação em Usuario
                      .WithMany(e => e.Usuarios) // Propriedade de coleção em Empresa
                      .HasForeignKey(u => u.EmpresaID); // Chave estrangeira em Usuario
            });

                                 

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;
                
                if (clrType.GetProperty("CreateTime") != null)
                {
                    modelBuilder.Entity(clrType)
                        .Property("CreateTime")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");
                }
              
                if (clrType.GetProperty("UpdateTime") != null)
                {
                    modelBuilder.Entity(clrType)
                       .Property("UpdateTime")
                       .HasDefaultValueSql("CURRENT_TIMESTAMP");
                   
                }
            }

            var initialAdminCreateTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc); // Exemplo de data fixa
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    ID = 1, // PK precisa ser especificada para HasData
                    Name = "admin",
                    IsAdmin = true,
                    EmpresaID = null, // Ou ID de uma empresa semeada, se aplicável
                    CreateTime = initialAdminCreateTime,
                    UpdateTime = initialAdminCreateTime,
                    TerminateTime = null
                }
            );
            
        }

    
    }
}
