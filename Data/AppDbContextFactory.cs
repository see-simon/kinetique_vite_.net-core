using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace KinetiqueAPI.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql("Host=aws-1-eu-west-2.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.orubnbrduaxsdocevpoz;Password=JnObYEIZSkGNLwxM;SSL Mode=Require;Trust Server Certificate=true;Pooling=false;");
            

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}