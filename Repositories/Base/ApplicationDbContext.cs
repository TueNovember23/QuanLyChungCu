using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using Repositories.Entities;

namespace Repositories.Base;

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

    public virtual DbSet<ManagementFeeInvoice> ManagementFeeInvoices { get; set; }

    public virtual DbSet<RepairInvoice> RepairInvoices { get; set; }

    public virtual DbSet<Representative> Representatives { get; set; }

    public virtual DbSet<Resident> Residents { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<VechicleInvoice> VechicleInvoices { get; set; }

    public virtual DbSet<VechicleInvoiceDetail> VechicleInvoiceDetails { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VehicleCategory> VehicleCategories { get; set; }

    public virtual DbSet<WaterInvoice> WaterInvoices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["QuanLyChungCuDb"].ConnectionString;
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK__Account__536C85E52E1D2D94");

            entity.ToTable("Account");

            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(255);

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_Role");
        });

        modelBuilder.Entity<Apartment>(entity =>
        {
            entity.HasKey(e => e.ApartmentId).HasName("PK__Apartmen__CBDF57647CCC7650");

            entity.ToTable("Apartment");

            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.Floor).WithMany(p => p.Apartments)
                .HasForeignKey(d => d.FloorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Apartment_Floor");
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.AreaId).HasName("PK__Area__70B8204856D625C9");

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
            entity.HasKey(e => e.BlockId).HasName("PK__Block__144215F1DA109BF0");

            entity.ToTable("Block");

            entity.Property(e => e.BlockCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.IsDeleleted).HasDefaultValue(false);

            entity.HasOne(d => d.Area).WithMany(p => p.Blocks)
                .HasForeignKey(d => d.AreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Block_Area");
        });

        modelBuilder.Entity<CommunityRoom>(entity =>
        {
            entity.HasKey(e => e.CommunityRoomId).HasName("PK__Communit__9AA1F65CFAD75445");

            entity.ToTable("CommunityRoom");

            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.RoomName).HasMaxLength(50);
        });

        modelBuilder.Entity<CommunityRoomBooking>(entity =>
        {
            entity.HasKey(e => e.CommunityRoomBookingId).HasName("PK__Communit__14F5AE4CF588523E");

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
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BEDD8876FD8");

            entity.ToTable("Department");

            entity.HasIndex(e => e.DepartmentName, "UQ__Departme__D949CC34C52625BF").IsUnique();

            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.EquipmentId).HasName("PK__Equipmen__34474479C154C696");

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
            entity.HasKey(e => e.FloorId).HasName("PK__Floor__49D1E84B61A932BB");

            entity.ToTable("Floor");

            entity.Property(e => e.IsDeleleted).HasDefaultValue(false);

            entity.HasOne(d => d.Block).WithMany(p => p.Floors)
                .HasForeignKey(d => d.BlockId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Floor_Block");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoice__D796AAB57CDF3ECF");

            entity.ToTable("Invoice");

            entity.Property(e => e.Month).HasDefaultValueSql("(datepart(month,getdate()))");
            entity.Property(e => e.Year).HasDefaultValueSql("(datepart(year,getdate()))");

            entity.HasOne(d => d.Apartment).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.ApartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_Apartment");
        });

        modelBuilder.Entity<Maintenance>(entity =>
        {
            entity.HasKey(e => e.MaintenanceId).HasName("PK__Maintena__E60542D57A5FA801");

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
                        j.HasKey("MaintenanceId", "EquipmentId").HasName("PK__Equipmen__654136ECAA78493A");
                        j.ToTable("EquipmentMaintanance");
                        j.IndexerProperty<int>("MaintenanceId").HasColumnName("MaintenanceID");
                        j.IndexerProperty<int>("EquipmentId").HasColumnName("EquipmentID");
                    });
        });

        modelBuilder.Entity<MalfunctionReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Malfunct__D5BD4805A88F51F8");

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

        modelBuilder.Entity<ManagementFeeInvoice>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ManagementFeeInvoice");

            entity.Property(e => e.ManagementFeeInvoiceId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<RepairInvoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__RepairIn__D796AAD5662206C4");

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
            entity.HasKey(e => e.RepresentativeId).HasName("PK__Represen__E25A2299A1B75C3A");

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
            entity.HasKey(e => e.ResidentId).HasName("PK__Resident__07FB00DC4C5554E0");

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
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A44DEAC4F");

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__8A2B6160AABB3091").IsUnique();

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VechicleInvoice>(entity =>
        {
            entity.HasKey(e => e.VechicleInvoiceId).HasName("PK__Vechicle__5E303E9969F815DA");

            entity.ToTable("VechicleInvoice");
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
            entity.HasKey(e => e.VehicleId).HasName("PK__Vehicle__476B54925675385C");

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
            entity.HasKey(e => e.VehicleCategoryId).HasName("PK__VehicleC__55CCA56728A56BFC");

            entity.ToTable("VehicleCategory");

            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.IsDeleleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<WaterInvoice>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("WaterInvoice");

            entity.Property(e => e.WaterInvoiceId).ValueGeneratedOnAdd();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
