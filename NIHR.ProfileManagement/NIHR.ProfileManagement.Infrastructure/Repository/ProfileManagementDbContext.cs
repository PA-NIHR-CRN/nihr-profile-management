using Microsoft.EntityFrameworkCore;
using NIHR.ProfileManagement.Infrastructure.Repository.Models;

namespace NIHR.ProfileManagement.Infrastructure.Repository
{
    public partial class ProfileManagementDbContext : DbContext
    {
        public ProfileManagementDbContext()
        {
        }

        public ProfileManagementDbContext(DbContextOptions<ProfileManagementDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ProfileInfoDbEntity> Profiles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProfileInfoDbEntity>(entity => {

                entity.ToTable("profileInfo");

                entity.HasKey("Id");

                entity.Property(e => e.Created).HasColumnName("created");
            });

            modelBuilder.Entity<PersonNameDbEntity>(entity => {

                entity.ToTable("profileInfoPersonName");

                entity.HasKey("Id");

                entity.Property(e => e.Given)
                    .HasColumnName("given")
                    .HasMaxLength(250);

                entity.Property(e => e.Family)
                    .HasColumnName("family")
                    .HasMaxLength(250);

                entity.Property(e => e.Created).HasColumnName("created");

                entity.Property(e => e.ProfileInfoId).HasColumnName("profileInfoId");

                entity.HasOne(d => d.ProfileInfo)
                    .WithMany(p => p.Names)
                    .HasForeignKey(d => d.ProfileInfoId)
                    .HasConstraintName("fk_profileInfoPersonName_profile");
            });

            modelBuilder.Entity<ProfileIdentity>(entity => {

                entity.ToTable("profileIdentity");

                entity.HasKey("Id");

                entity.Property(e => e.Sub)
                    .HasColumnName("sub")
                    .HasMaxLength(250);

                entity.Property(e => e.Created).HasColumnName("created");

                entity.Property(e => e.ProfileInfoId).HasColumnName("profileInfoId");

                entity.HasOne(d => d.ProfileInfo)
                    .WithMany(p => p.Identities)
                    .HasForeignKey(d => d.ProfileInfoId)
                    .HasConstraintName("fk_profileIdentity_profile");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
