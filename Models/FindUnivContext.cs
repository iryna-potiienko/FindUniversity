using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FindUniversity
{
    public partial class FindUnivContext : DbContext
    {
        public FindUnivContext()
        {
        }

        public FindUnivContext(DbContextOptions<FindUnivContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Countries> Countries { get; set; }
        public virtual DbSet<EducationalProg> EducationalProg { get; set; }
        public virtual DbSet<Faculties> Faculties { get; set; }
        public virtual DbSet<FacultyEducationalProg> FacultyEducationalProg { get; set; }
        public virtual DbSet<Specialties> Specialties { get; set; }
        public virtual DbSet<Universities> Universities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=IPOTIIENKONB\\SQLEXPRESS; Database=FindUniv; Trusted_Connection=True; ");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Countries>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<EducationalProg>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(10);

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("money");

                entity.Property(e => e.SpecialtiesId).HasColumnName("specialtiesId");

                entity.HasOne(d => d.Specialties)
                    .WithMany(p => p.EducationalProg)
                    .HasForeignKey(d => d.SpecialtiesId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EducationalProg_Specialties");
            });

            modelBuilder.Entity<Faculties>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Info)
                    .HasColumnName("info")
                    .HasMaxLength(10);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(10);

                entity.Property(e => e.UniversityId).HasColumnName("universityId");

                entity.HasOne(d => d.University)
                    .WithMany(p => p.Faculties)
                    .HasForeignKey(d => d.UniversityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Faculties_Universities");
            });

            modelBuilder.Entity<FacultyEducationalProg>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.EducationalProgId).HasColumnName("educationalProgId");

                entity.Property(e => e.FacultyId).HasColumnName("facultyId");

                entity.HasOne(d => d.EducationalProg)
                    .WithMany(p => p.FacultyEducationalProg)
                    .HasForeignKey(d => d.EducationalProgId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FacultyEducationalProg_EducationalProg");

                entity.HasOne(d => d.Faculty)
                    .WithMany(p => p.FacultyEducationalProg)
                    .HasForeignKey(d => d.FacultyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FacultyEducationalProg_Faculties");
            });

            modelBuilder.Entity<Specialties>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Info)
                    .HasColumnName("info")
                    .HasColumnType("ntext");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<Universities>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CountryId).HasColumnName("countryId");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(10);

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Universities)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Universities_Countries");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
