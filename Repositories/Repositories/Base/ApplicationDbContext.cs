﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Repositories.Repositories.Entities;

namespace Repositories.Repositories.Base;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Apartment> Apartments { get; set; }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<Block> Blocks { get; set; }

    public virtual DbSet<CommunityRoom> CommunityRooms { get; set; }

    public virtual DbSet<CommunityRoomBooking> CommunityRoomBookings { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Equipment> Equipment { get; set; }

    public virtual DbSet<Floor> Floors { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Maintenance> Maintenances { get; set; }

    public virtual DbSet<MalfunctionReport> MalfunctionReports { get; set; }

    public virtual DbSet<ManagementFeeHistory> ManagementFeeHistories { get; set; }

    public virtual DbSet<ManagementFeeInvoice> ManagementFeeInvoices { get; set; }

    public virtual DbSet<Regulation> Regulations { get; set; }

    public virtual DbSet<RepairInvoice> RepairInvoices { get; set; }

    public virtual DbSet<Representative> Representatives { get; set; }

    public virtual DbSet<Resident> Residents { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<VechicleInvoice> VechicleInvoices { get; set; }

    public virtual DbSet<VechicleInvoiceDetail> VechicleInvoiceDetails { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VehicleCategory> VehicleCategories { get; set; }

    public virtual DbSet<Violation> Violations { get; set; }

    public virtual DbSet<WaterFeeHistory> WaterFeeHistories { get; set; }

    public virtual DbSet<WaterInvoice> WaterInvoices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=QuanLyChungCu;Trusted_Connection=True;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK__Account__536C85E5AA1AE6F3");

            entity.ToTable("Account");

            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_Role");
        });

        modelBuilder.Entity<Apartment>(entity =>
        {
            entity.HasKey(e => e.ApartmentId).HasName("PK__Apartmen__CBDF5764DFA8B51F");

            entity.ToTable("Apartment", tb => tb.HasTrigger("TRG_Apartment_Insert"));

            entity.Property(e => e.ApartmentId).ValueGeneratedNever();
            entity.Property(e => e.ApartmentCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.Floor).WithMany(p => p.Apartments)
                .HasForeignKey(d => d.FloorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Apartment_Floor");
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.AreaId).HasName("PK__Area__70B8204880A20D7B");

            entity.ToTable("Area");

            entity.Property(e => e.AreaName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Block>(entity =>
        {
            entity.HasKey(e => e.BlockId).HasName("PK__Block__144215F180954C19");

            entity.ToTable("Block");

            entity.Property(e => e.BlockCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.IsDeleleted).HasDefaultValue(false);
            entity.Property(e => e.NumberOfFloor).HasDefaultValue(0);

            entity.HasOne(d => d.Area).WithMany(p => p.Blocks)
                .HasForeignKey(d => d.AreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Block_Area");
        });

        modelBuilder.Entity<CommunityRoom>(entity =>
        {
            entity.HasKey(e => e.CommunityRoomId).HasName("PK__Communit__9AA1F65C5842B044");

            entity.ToTable("CommunityRoom");

            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.RoomName).HasMaxLength(50);
        });

        modelBuilder.Entity<CommunityRoomBooking>(entity =>
        {
            entity.HasKey(e => e.CommunityRoomBookingId).HasName("PK__Communit__14F5AE4C8D06A79A");

            entity.ToTable("CommunityRoomBooking");

            entity.Property(e => e.BookingDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NumberOfPeople).HasDefaultValue(1);

            entity.HasOne(d => d.Apartment).WithMany(p => p.CommunityRoomBookings)
                .HasForeignKey(d => d.ApartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CommunityRoomBooking_Apartment");

            entity.HasOne(d => d.CommunityRoom).WithMany(p => p.CommunityRoomBookings)
                .HasForeignKey(d => d.CommunityRoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CommunityRoomBooking_CommunityRoom");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BEDFEF0C1AF");

            entity.ToTable("Department");

            entity.HasIndex(e => e.DepartmentName, "UQ__Departme__D949CC34769135D4").IsUnique();

            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.EquipmentId).HasName("PK__Equipmen__34474479505D598A");

            entity.Property(e => e.EquipmentId).ValueGeneratedNever();
            entity.Property(e => e.Discription)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EquipmentName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Hoạt động");

            entity.HasOne(d => d.Area).WithMany(p => p.Equipment)
                .HasForeignKey(d => d.AreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Equipment_Area");
        });

        modelBuilder.Entity<Floor>(entity =>
        {
            entity.HasKey(e => e.FloorId).HasName("PK__Floor__49D1E84B70F186A9");

            entity.ToTable("Floor", tb => tb.HasTrigger("TRG_Floor_Insert"));

            entity.Property(e => e.NumberOfApartment).HasDefaultValue(0);

            entity.HasOne(d => d.Block).WithMany(p => p.Floors)
                .HasForeignKey(d => d.BlockId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Floor_Block");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoice__D796AAB52B958C04");

            entity.ToTable("Invoice");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Month).HasDefaultValueSql("(datepart(month,getdate()))");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Chưa thanh toán");
            entity.Property(e => e.Year).HasDefaultValueSql("(datepart(year,getdate()))");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_CreatedBy");
        });

        modelBuilder.Entity<Maintenance>(entity =>
        {
            entity.HasKey(e => e.MaintenanceId).HasName("PK__Maintena__E60542D5020378C5");

            entity.ToTable("Maintenance");

            entity.Property(e => e.MaintenanceId).ValueGeneratedNever();
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MaintanaceDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MaintenanceName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Chưa hoàn thành");

            entity.HasOne(d => d.Department).WithMany(p => p.Maintenances)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Maintenance_Department");

            entity.HasMany(d => d.Equipment).WithMany(p => p.Maintenances)
                .UsingEntity<Dictionary<string, object>>(
                    "EquipmentMaintanance",
                    r => r.HasOne<Equipment>().WithMany()
                        .HasForeignKey("EquipmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_EquipmentMaintanance_Equipment"),
                    l => l.HasOne<Maintenance>().WithMany()
                        .HasForeignKey("MaintenanceId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_EquipmentMaintanance_Maintenance"),
                    j =>
                    {
                        j.HasKey("MaintenanceId", "EquipmentId").HasName("PK__Equipmen__654136EC46F280D6");
                        j.ToTable("EquipmentMaintanance");
                        j.IndexerProperty<int>("MaintenanceId").HasColumnName("MaintenanceID");
                        j.IndexerProperty<int>("EquipmentId").HasColumnName("EquipmentID");
                    });
        });

        modelBuilder.Entity<MalfunctionReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Malfunct__D5BD480510B3B9A5");

            entity.ToTable("MalfunctionReport");

            entity.Property(e => e.ReportId).ValueGeneratedNever();
            entity.Property(e => e.ReportContent)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ReportDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ReportStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Chưa xử lý");

            entity.HasOne(d => d.Equipment).WithMany(p => p.MalfunctionReports)
                .HasForeignKey(d => d.EquipmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MalfunctionReport_Equipment");
        });

        modelBuilder.Entity<ManagementFeeHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ManagementFeeHistory");
        });

        modelBuilder.Entity<ManagementFeeInvoice>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ManagementFeeInvoice");

            entity.Property(e => e.ManagementFeeInvoiceId).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Apartment).WithMany()
                .HasForeignKey(d => d.ApartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ManagementFeeInvoice_Apartment");

            entity.HasOne(d => d.Invoice).WithMany()
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ManagementFeeInvoice_Invoice");
        });

        modelBuilder.Entity<Regulation>(entity =>
        {
            entity.HasKey(e => e.RegulationId).HasName("PK__Regulati__A192C7E9F2EB773D");

            entity.ToTable("Regulation");

            entity.Property(e => e.Content).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<RepairInvoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__RepairIn__D796AAD570C880DB");

            entity.ToTable("RepairInvoice");

            entity.Property(e => e.InvoiceId)
                .ValueGeneratedNever()
                .HasColumnName("InvoiceID");
            entity.Property(e => e.InvoiceContent)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.InvoiceDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MalfunctionReportId).HasColumnName("MalfunctionReportID");
            entity.Property(e => e.SolvingMethod)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Chưa thanh toán");

            entity.HasOne(d => d.Department).WithMany(p => p.RepairInvoices)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_RepairInvoice_Department");

            entity.HasOne(d => d.MalfunctionReport).WithMany(p => p.RepairInvoices)
                .HasForeignKey(d => d.MalfunctionReportId)
                .HasConstraintName("FK_RepairInvoice_MalfunctionReport");
        });

        modelBuilder.Entity<Representative>(entity =>
        {
            entity.HasKey(e => e.RepresentativeId).HasName("PK__Represen__E25A22996C7B5617");

            entity.ToTable("Representative");

            entity.Property(e => e.RepresentativeId)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ApartmentId).HasColumnName("ApartmentID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Apartment).WithMany(p => p.Representatives)
                .HasForeignKey(d => d.ApartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Representative_Apartment");
        });

        modelBuilder.Entity<Resident>(entity =>
        {
            entity.HasKey(e => e.ResidentId).HasName("PK__Resident__07FB00DCF3855FB6");

            entity.ToTable("Resident");

            entity.Property(e => e.ResidentId)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ApartmentId).HasColumnName("ApartmentID");
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.RelationShipWithOwner).HasMaxLength(50);

            entity.HasOne(d => d.Apartment).WithMany(p => p.Residents)
                .HasForeignKey(d => d.ApartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Resident_Apartment");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1AB7654F6C");

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__8A2B616005B74E95").IsUnique();

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VechicleInvoice>(entity =>
        {
            entity.HasKey(e => e.VechicleInvoiceId).HasName("PK__Vechicle__5E303E996CAEB917");

            entity.ToTable("VechicleInvoice");

            entity.HasOne(d => d.Apartment).WithMany(p => p.VechicleInvoices)
                .HasForeignKey(d => d.ApartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VechicleInvoice_Apartment");

            entity.HasOne(d => d.Invoice).WithMany(p => p.VechicleInvoices)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VechicleInvoice_Invoice");
        });

        modelBuilder.Entity<VechicleInvoiceDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("VechicleInvoiceDetail");

            entity.Property(e => e.VehicleId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.VechicleInvoice).WithMany()
                .HasForeignKey(d => d.VechicleInvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VechicleInvoiceDetail_VechicleInvoice");

            entity.HasOne(d => d.Vehicle).WithMany()
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VechicleInvoiceDetail_Vehicle");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK__Vehicle__476B549295B1BC0F");

            entity.ToTable("Vehicle");

            entity.Property(e => e.VehicleId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Apartment).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.ApartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicle_Apartment");

            entity.HasOne(d => d.VehicleCategory).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.VehicleCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicle_VehicleCategory");
        });

        modelBuilder.Entity<VehicleCategory>(entity =>
        {
            entity.HasKey(e => e.VehicleCategoryId).HasName("PK__VehicleC__55CCA5679236DC0E");

            entity.ToTable("VehicleCategory");

            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.IsDeleleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<Violation>(entity =>
        {
            entity.HasKey(e => e.ViolationId).HasName("PK__Violatio__18B6DC08E3B12B44");

            entity.ToTable("Violation");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Detail).HasMaxLength(255);

            entity.HasOne(d => d.Apartment).WithMany(p => p.Violations)
                .HasForeignKey(d => d.ApartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Violation_Apartment");

            entity.HasOne(d => d.Regulation).WithMany(p => p.Violations)
                .HasForeignKey(d => d.RegulationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Violation_Regulation");
        });

        modelBuilder.Entity<WaterFeeHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("WaterFeeHistory");
        });

        modelBuilder.Entity<WaterInvoice>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("WaterInvoice");

            entity.Property(e => e.WaterInvoiceId).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Apartment).WithMany()
                .HasForeignKey(d => d.ApartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WaterInvoice_Apartment");

            entity.HasOne(d => d.Invoice).WithMany()
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WaterInvoice_Invoice");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}