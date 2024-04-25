using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProjectNFTs.Infraestructure.Models;

namespace ProjectNFTs.Infraestructure.Data;

public partial class ProjectNFTsContext : DbContext
{
    public ProjectNFTsContext(DbContextOptions<ProjectNFTsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Cliente { get; set; }

    public virtual DbSet<DetalleFactura> DetalleFactura { get; set; }

    public virtual DbSet<EncabezadoFactura> EncabezadoFactura { get; set; }

    public virtual DbSet<Nft> Nft { get; set; }

    public virtual DbSet<Pais> Pais { get; set; }

    public virtual DbSet<Purchase> Purchase { get; set; }

    public virtual DbSet<RolUsuario> RolUsuario { get; set; }

    public virtual DbSet<Tarjeta> Tarjeta { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK__Cliente__D59466429CACD694");

            entity.Property(e => e.IdCliente).ValueGeneratedNever();
            entity.Property(e => e.Apellido1)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Apellido2)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FechaDeNacimiento).HasColumnType("datetime");
            entity.Property(e => e.IdCriptoWallet)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Sexo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.IdPaisNavigation).WithMany(p => p.Cliente)
                .HasForeignKey(d => d.IdPais)
                .HasConstraintName("FK_Pais_Cliente");
        });

        modelBuilder.Entity<DetalleFactura>(entity =>
        {
            entity.HasKey(e => new { e.IdFactura, e.IdDetalle }).HasName("PK__Detalle___E43646A5F28235CF");

            entity.ToTable(tb => tb.HasTrigger("trgInsertPurchase"));

            entity.Property(e => e.IdNft).HasColumnName("IdNFT");
            entity.Property(e => e.Precio).HasColumnType("numeric(18, 2)");

            entity.HasOne(d => d.IdFacturaNavigation).WithMany(p => p.DetalleFactura)
                .HasForeignKey(d => d.IdFactura)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleFactura_EncabezadoFactura");

            entity.HasOne(d => d.IdNftNavigation).WithMany(p => p.DetalleFactura)
                .HasForeignKey(d => d.IdNft)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleFactura_NFT1");
        });

        modelBuilder.Entity<EncabezadoFactura>(entity =>
        {
            entity.HasKey(e => e.IdFactura).HasName("PK__Encabeza__50E7BAF1FB18A0B6");

            entity.Property(e => e.IdFactura).ValueGeneratedNever();
            entity.Property(e => e.FechaFacturacion).HasColumnType("datetime");
            entity.Property(e => e.NumeroTarjeta)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Total).HasColumnType("numeric(18, 2)");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.EncabezadoFactura)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK_EncabezadoFactura_Cliente1");

            entity.HasOne(d => d.IdTarjetaNavigation).WithMany(p => p.EncabezadoFactura)
                .HasForeignKey(d => d.IdTarjeta)
                .HasConstraintName("FK_Tarjeta_Encabezado_Factura");
        });

        modelBuilder.Entity<Nft>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NFT__3214EC079F982CA3");

            entity.ToTable("NFT");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Autor)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Valor).HasColumnType("numeric(18, 2)");
        });

        modelBuilder.Entity<Pais>(entity =>
        {
            entity.HasKey(e => e.IdPais).HasName("PK__Pais__FC850A7B2B030A20");

            entity.Property(e => e.IdPais).ValueGeneratedNever();
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.Property(e => e.PurchaseId).HasColumnName("Purchase_ID");
            entity.Property(e => e.CustomerId).HasColumnName("Customer_ID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.IdNft).HasColumnName("ID_NFT");
        });

        modelBuilder.Entity<RolUsuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RolUsuar__3214EC07DD907290");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Tarjeta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tarjeta__3214EC07675DC6A0");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Login).HasName("PK__Usuario__5E55825A041A6F32");

            entity.Property(e => e.Login)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK_Login_Rol");
        });
        modelBuilder.HasSequence("ReceiptNumber");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
