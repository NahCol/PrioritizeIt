using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Prioritize.Data
{
    public partial class PrioritizeDatabaseContext : DbContext
    {
        public PrioritizeDatabaseContext()
        {
        }

        public PrioritizeDatabaseContext(DbContextOptions<PrioritizeDatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DItem> DItems { get; set; }
        public virtual DbSet<LCoder> LCoders { get; set; }
        public virtual DbSet<LPriorityLevel> LPriorityLevels { get; set; }
        public virtual DbSet<LStatus> LStatuses { get; set; }
        public virtual DbQuery<VDEQEmp> DEQEmps { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<DItem>(entity =>
            {
                entity.Property(e => e.Action).IsUnicode(false);

                entity.Property(e => e.Board).IsUnicode(false);

                entity.Property(e => e.CardNumber).IsUnicode(false);

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Link).IsUnicode(false);

                entity.Property(e => e.List).IsUnicode(false);

                entity.Property(e => e.Requirement).IsUnicode(false);

                entity.HasOne(d => d.Coder)
                    .WithMany(p => p.DItems)
                    .HasForeignKey(d => d.CoderId)
                    .HasConstraintName("FK_d_item_Coder");

                entity.HasOne(d => d.PriorityLevel)
                    .WithMany(p => p.DItems)
                    .HasForeignKey(d => d.PriorityLevelId)
                    .HasConstraintName("FK_d_items_PriorityLevel");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.DItems)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_d_item_Status");
            });

            modelBuilder.Entity<LCoder>(entity =>
            {
                entity.Property(e => e.UserName).IsUnicode(false);
            });

            modelBuilder.Entity<LPriorityLevel>(entity =>
            {
                entity.Property(e => e.Text).IsUnicode(false);
            });

            modelBuilder.Entity<LStatus>(entity =>
            {
                entity.Property(e => e.Text).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}