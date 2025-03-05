using System;
using System.Collections.Generic;
using BoletinServiceWorker.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoletinServiceWorker.Data;

public partial class SicemContext : DbContext
{
    public SicemContext()
    {
    }

    public SicemContext(DbContextOptions<SicemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BoletinMensaje> BoletinMensajes { get; set; }

    public virtual DbSet<CatProveedore> CatProveedores { get; set; }

    public virtual DbSet<Destinatario> Destinatarios { get; set; }

    public virtual DbSet<OprBoletin> OprBoletins { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=SICEM");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BoletinMensaje>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BoletinM__3213E83FF30858ED");

            entity.ToTable("BoletinMensaje", "Boletin");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.BoletinId).HasColumnName("boletinId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createdAt");
            entity.Property(e => e.DeletedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("deletedAt");
            entity.Property(e => e.EsArchivo)
                .HasDefaultValue(false)
                .HasColumnName("esArchivo");
            entity.Property(e => e.FileName)
                .IsUnicode(false)
                .HasColumnName("fileName");
            entity.Property(e => e.FileSize).HasColumnName("fileSize");
            entity.Property(e => e.Mensaje)
                .IsUnicode(false)
                .HasColumnName("mensaje");
            entity.Property(e => e.MimmeType)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("mimmeType");

            entity.HasOne(d => d.Boletin).WithMany(p => p.BoletinMensajes)
                .HasForeignKey(d => d.BoletinId)
                .HasConstraintName("FK_BoletinMensaje_Boletin");
        });

        modelBuilder.Entity<CatProveedore>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CatProveedores", "Boletin");

            entity.HasIndex(e => e.Name, "UQ__CatProve__737584F683CA713A").IsUnique();

            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Destinatario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Destinat__3213E83FE6E92155");

            entity.ToTable("Destinatario", "Boletin");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.BoletinId).HasColumnName("boletinId");
            entity.Property(e => e.Correo)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.EnvioMetadata)
                .IsUnicode(false)
                .HasColumnName("envioMetadata");
            entity.Property(e => e.Error).HasColumnName("error");
            entity.Property(e => e.FechaEnvio).HasColumnName("fechaEnvio");
            entity.Property(e => e.Lada)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("lada");
            entity.Property(e => e.Resultado)
                .IsUnicode(false)
                .HasColumnName("resultado");
            entity.Property(e => e.Telefono).HasColumnName("telefono");
            entity.Property(e => e.Titulo)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("titulo");

            entity.HasOne(d => d.Boletin).WithMany(p => p.Destinatarios)
                .HasForeignKey(d => d.BoletinId)
                .HasConstraintName("FK_Destinatario_Boletin");
        });

        modelBuilder.Entity<OprBoletin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OprBolet__3213E83F01A9C76E");

            entity.ToTable("OprBoletin", "Boletin");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createdAt");
            entity.Property(e => e.FinishedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("finishedAt");
            entity.Property(e => e.Proveedor)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("WAPP")
                .HasColumnName("proveedor");
            entity.Property(e => e.Titulo)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("titulo");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
