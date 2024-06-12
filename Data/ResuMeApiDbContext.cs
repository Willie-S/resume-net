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

            modelBuilder.Entity<Profile>(e =>
            {
                e.HasKey(p => p.Id);
                e.HasIndex(p => p.Email).IsUnique();
            });
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

        /// <summary>
        /// Automatically assigns the values to each entity's DateCreated & DateUpdated fields
        /// </summary>
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
                ((BaseEntity)entityEntry.Entity).DateCreated = entityEntry.State == EntityState.Added ? DateTime.Now : ((BaseEntity)entityEntry.Entity).DateCreated;
            }
        }

        #endregion
    }
}
