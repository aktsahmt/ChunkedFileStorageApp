using ChunkedFileStorageApp.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChunkedFileStorageApp.Infrastructure.Persistences;

public class FileStorageDbContext : DbContext
{
    public FileStorageDbContext(DbContextOptions<FileStorageDbContext> options) : base(options) { }

    public DbSet<StoredFile> StoredFiles { get; set; }
    public DbSet<FileChunk> FileChunks { get; set; }
    public DbSet<StoredChunkData> ChunkDatas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StoredFile>().HasKey(f => f.Id);
        modelBuilder.Entity<StoredFile>()
            .HasMany(f => f.Chunks)
            .WithOne()
            .HasForeignKey(c => c.StoredFileId);

        modelBuilder.Entity<FileChunk>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StorageProviderName).IsRequired();
        });


        modelBuilder.Entity<StoredChunkData>().HasKey(c => c.Id);

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}