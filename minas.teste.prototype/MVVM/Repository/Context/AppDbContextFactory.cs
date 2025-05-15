using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace minas.teste.prototype.MVVM.Repository.Context
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Configuração para SQLite (ajuste o caminho se necessário)
            optionsBuilder.UseSqlite("Data Source=supervisory_SYSMT_data.db");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}