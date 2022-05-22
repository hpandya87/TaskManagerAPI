using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.DBModels;

namespace TaskManager.Infrastructure.DbContexts
{
    public partial class TaskManagerDbContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        public TaskManagerDbContext()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TaskDetail> TaskDetails { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<TaskDetail>(entity =>
            {
                entity.HasIndex(e => e.Id, "IX_dboTasks")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Priority)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
