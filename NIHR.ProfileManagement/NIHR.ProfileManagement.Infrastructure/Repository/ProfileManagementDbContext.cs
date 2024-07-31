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

        public virtual DbSet<OutboxRecordDbEntity> OutboxRecords { get; set; } = null!;

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

            modelBuilder.Entity<ProfileIdentityDbEntity>(entity => {

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

            modelBuilder.Entity<OutboxRecordDbEntity>(entity =>
            {
                entity.ToTable("outboxEntry");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created).HasColumnName("created");

                entity.Property(e => e.EventType).HasColumnName("eventtype");

                entity.Property(e => e.SourceSystem).HasColumnName("sourcesystem");

                entity.Property(e => e.ProcessingStartDate).HasColumnName("processingStartDate").IsRequired(false);

                entity.Property(e => e.ProcessingCompletedDate).HasColumnName("processingCompletedDate").IsRequired(false);

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Payload)
                    .HasColumnType("json")
                    .HasColumnName("payload");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
