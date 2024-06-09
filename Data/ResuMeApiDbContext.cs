using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ResuMeAPI.Models;

namespace ResuMeAPI.Data
{
    public class ResuMeApiDbContext : DbContext
    {
        public ResuMeApiDbContext(DbContextOptions<ResuMeApiDbContext> options)
            : base(options)
        {
        }

        #region TABLES

        public DbSet<Profile> Profiles { get; set; }

        #endregion

        #region METHODS

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Profile>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Profile>()
                .HasIndex(p => p.Email)
                .IsUnique();
        }

        public override int SaveChanges()
        {
            InterceptSaveChanges();

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            InterceptSaveChanges();

            return await base.SaveChangesAsync(cancellationToken);
        }

        private void InterceptSaveChanges()
        {
            var entries = ChangeTracker
            .Entries()
            .Where(e =>
                    e.Entity is BaseEntity && (
                        e.State == EntityState.Added ||
                        e.State == EntityState.Modified
                    )
                );

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).DateUpdated = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).DateCreated = DateTime.Now;
                }
            }
        }

        #endregion
    }
}
