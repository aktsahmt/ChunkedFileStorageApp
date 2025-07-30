using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ChunkedFileStorageApp.Infrastructure.Persistences;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FileStorageDbContext>
{
    public FileStorageDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FileStorageDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FileStorageDb;Trusted_Connection=True;");

        return new FileStorageDbContext(optionsBuilder.Options);
    }
}
