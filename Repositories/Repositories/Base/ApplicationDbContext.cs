using Microsoft.EntityFrameworkCore;
using Repositories.Repositories.Entities;
using System.Configuration;

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

    public virtual DbSet<MalfuntionEquipment> MalfuntionEquipments { get; set; }

    public virtual DbSet<ManagementFee> ManagementFees { get; set; }

    public virtual DbSet<ManagementFeeInvoice> ManagementFeeInvoices { get; set; }

    public virtual DbSet<ParkingConfig> ParkingConfigs { get; set; }

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

    public virtual DbSet<ViolationPenalty> ViolationPenalties { get; set; }

    public virtual DbSet<WaterFee> WaterFees { get; set; }

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
            entity.HasKey(e => e.AccountId).HasName("PK__Account__349DA5A6E521E87A");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Username, "UQ__Account__536C85E4F6538E9C").IsUnique();

            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_Role");
        });

        modelBuilder.Entity<Apartment>(entity =>
        {
            entity.HasKey(e => e.ApartmentId).HasName("PK__Apartmen__CBDF5764F2713058");

            entity.ToTable("Apartment", tb =>
                {
                    tb.HasTrigger("TRG_Apartment_Insert");
                    tb.HasTrigger("TRG_Apartment_Insert_AutoCode");
                });

            entity.Property(e => e.ApartmentCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.NumberOfPeople).HasDefaultValue(0);
            entity.Property(e => e.RepresentativeId)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.Floor).WithMany(p => p.Apartments)
                .HasForeignKey(d => d.FloorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Apartment_Floor");

            entity.HasOne(d => d.Representative).WithMany(p => p.Apartments)
                .HasForeignKey(d => d.RepresentativeId)
                .HasConstraintName("FK_Apartment_Representative");
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.AreaId).HasName("PK__Area__70B82048D64EA4D5");

            entity.ToTable("Area");

            entity.Property(e => e.AreaName).HasMaxLength(100);
            entity.Property(e => e.Location).HasMaxLength(100);
        });

        modelBuilder.Entity<Block>(entity =>
        {
            entity.HasKey(e => e.BlockId).HasName("PK__Block__144215F1E910EF9D");

            entity.ToTable("Block");

            entity.Property(e => e.BlockCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.NumberOfFloor).HasDefaultValue(0);

            entity.HasOne(d => d.Area).WithMany(p => p.Blocks)
                .HasForeignKey(d => d.AreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Block_Area");
        });

        modelBuilder.Entity<CommunityRoom>(entity =>
        {
            entity.HasKey(e => e.CommunityRoomId).HasName("PK__Communit__9AA1F65C35B19521");

            entity.ToTable("CommunityRoom");

            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.RoomName).HasMaxLength(50);
        });

        modelBuilder.Entity<CommunityRoomBooking>(entity =>
        {
            entity.HasKey(e => e.CommunityRoomBookingId).HasName("PK__Communit__14F5AE4CFF5030A9");

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
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BED311BB5CC");

            entity.ToTable("Department");

            entity.HasIndex(e => e.DepartmentName, "UQ__Departme__D949CC349FFEA9A0").IsUnique();

            entity.Property(e => e.DepartmentName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.EquipmentId).HasName("PK__Equipmen__3447447987762FFB");

            entity.Property(e => e.EquipmentId).ValueGeneratedNever();
            entity.Property(e => e.Discription).HasMaxLength(255);
            entity.Property(e => e.EquipmentName).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Hoạt động");

            entity.HasOne(d => d.Area).WithMany(p => p.Equipment)
                .HasForeignKey(d => d.AreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Equipment_Area");
        });

        modelBuilder.Entity<Floor>(entity =>
        {
            entity.HasKey(e => e.FloorId).HasName("PK__Floor__49D1E84B25438BF0");

            entity.ToTable("Floor", tb => tb.HasTrigger("TRG_Floor_Insert"));

            entity.Property(e => e.NumberOfApartment).HasDefaultValue(0);

            entity.HasOne(d => d.Block).WithMany(p => p.Floors)
                .HasForeignKey(d => d.BlockId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Floor_Block");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoice__D796AAB572BC9043");

            entity.ToTable("Invoice");

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
            entity.HasKey(e => e.MaintenanceId).HasName("PK__Maintena__E60542D5C34BA8B8");

            entity.ToTable("Maintenance");

            entity.Property(e => e.MaintenanceId).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.MaintanaceDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MaintenanceName).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Chưa hoàn thành");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Maintenances)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Maintenance_Account");

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
                        j.HasKey("MaintenanceId", "EquipmentId").HasName("PK__Equipmen__654136EC6A77F425");
                        j.ToTable("EquipmentMaintanance");
                        j.IndexerProperty<int>("MaintenanceId").HasColumnName("MaintenanceID");
                        j.IndexerProperty<int>("EquipmentId").HasColumnName("EquipmentID");
                    });
        });

        modelBuilder.Entity<MalfuntionEquipment>(entity =>
        {
            entity.HasKey(e => e.MalfuntionId).HasName("PK__Malfunti__D04DFA8B56DBBE13");

            entity.ToTable("MalfuntionEquipment");

            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.SolvingMethod).HasMaxLength(255);

            entity.HasOne(d => d.Equipment).WithMany(p => p.MalfuntionEquipments)
                .HasForeignKey(d => d.EquipmentId)
                .HasConstraintName("FK_MalfuntionEquipment_Equipment");

            entity.HasOne(d => d.RepairInvoice).WithMany(p => p.MalfuntionEquipments)
                .HasForeignKey(d => d.RepairInvoiceId)
                .HasConstraintName("FK_MalfuntionEquipment_RepairInvoice");
        });

        modelBuilder.Entity<ManagementFee>(entity =>
        {
            entity.HasKey(e => e.ManagementFeeId).HasName("PK__Manageme__F316122CD160B015");

            entity.ToTable("ManagementFee");
        });

        modelBuilder.Entity<ManagementFeeInvoice>(entity =>
        {
            entity.HasKey(e => e.ManagementFeeInvoiceId).HasName("PK__Manageme__12F39501F4C43954");

            entity.ToTable("ManagementFeeInvoice");

            entity.HasOne(d => d.Apartment).WithMany(p => p.ManagementFeeInvoices)
                .HasForeignKey(d => d.ApartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ManagementFeeInvoice_Apartment");

            entity.HasOne(d => d.Invoice).WithMany(p => p.ManagementFeeInvoices)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ManagementFeeInvoice_Invoice");

            entity.HasOne(d => d.ManagementFeeHistory).WithMany(p => p.ManagementFeeInvoices)
                .HasForeignKey(d => d.ManagementFeeHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ManagementFeeInvoice_ManagementFeeHistory");
        });

        modelBuilder.Entity<ParkingConfig>(entity =>
        {
            entity.HasKey(e => e.ConfigId).HasName("PK__ParkingC__C3BC335C60822775");

            entity.ToTable("ParkingConfig");

            entity.HasOne(d => d.Category).WithMany(p => p.ParkingConfigs)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__ParkingCo__Categ__29221CFB");
        });

        modelBuilder.Entity<Regulation>(entity =>
        {
            entity.HasKey(e => e.RegulationId).HasName("PK__Regulati__A192C7E9BDEEC2B0");

            entity.ToTable("Regulation");

            entity.Property(e => e.Category)
                .HasMaxLength(20)
                .HasDefaultValue("Khác");
            entity.Property(e => e.Content).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Priority)
                .HasMaxLength(20)
                .HasDefaultValue("Trung bình");
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<RepairInvoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__RepairIn__D796AAD5F224425E");

            entity.ToTable("RepairInvoice");

            entity.Property(e => e.InvoiceId)
                .ValueGeneratedNever()
                .HasColumnName("InvoiceID");
            entity.Property(e => e.InvoiceContent).HasMaxLength(255);
            entity.Property(e => e.InvoiceDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Chưa thanh toán");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RepairInvoices)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_RepairInvoice_Account");
        });

        modelBuilder.Entity<Representative>(entity =>
        {
            entity.HasKey(e => e.RepresentativeId).HasName("PK__Represen__E25A2299738235FD");

            entity.ToTable("Representative");

            entity.Property(e => e.RepresentativeId)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(4)
                .IsFixedLength();
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<Resident>(entity =>
        {
            entity.HasKey(e => e.ResidentId).HasName("PK__Resident__07FB00DCDBCE3510");

            entity.ToTable("Resident", tb => tb.HasTrigger("TRG_Resident_Insert"));

            entity.Property(e => e.ResidentId)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ApartmentId).HasColumnName("ApartmentID");
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(4)
                .IsFixedLength();
            entity.Property(e => e.RelationShipWithOwner).HasMaxLength(50);

            entity.HasOne(d => d.Apartment).WithMany(p => p.Residents)
                .HasForeignKey(d => d.ApartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Resident_Apartment");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A8A9A3A0A");

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__8A2B61607DB73183").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VechicleInvoice>(entity =>
        {
            entity.HasKey(e => e.VechicleInvoiceId).HasName("PK__Vechicle__5E303E990C02A7E9");

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
            entity.HasKey(e => e.VehicleId).HasName("PK__Vehicle__476B5492B46DF468");

            entity.ToTable("Vehicle");

            entity.Property(e => e.VehicleId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.VehicleOwner).HasMaxLength(50);

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
            entity.HasKey(e => e.VehicleCategoryId).HasName("PK__VehicleC__55CCA567570128F8");

            entity.ToTable("VehicleCategory");

            entity.Property(e => e.CategoryName).HasMaxLength(50);
        });

        modelBuilder.Entity<Violation>(entity =>
        {
            entity.HasKey(e => e.ViolationId).HasName("PK__Violatio__18B6DC087F93FE43");

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

        modelBuilder.Entity<ViolationPenalty>(entity =>
        {
            entity.HasKey(e => e.PenaltyId).HasName("PK__Violatio__567E06C7FEFB4DB1");

            entity.ToTable("ViolationPenalty");

            entity.Property(e => e.Fine).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.PenaltyLevel).HasMaxLength(20);
            entity.Property(e => e.PenaltyMethod).HasMaxLength(255);
            entity.Property(e => e.ProcessedDate).HasColumnType("datetime");
            entity.Property(e => e.ProcessingStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Chờ xử lý");

            entity.HasOne(d => d.Violation).WithMany(p => p.ViolationPenalties)
                .HasForeignKey(d => d.ViolationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ViolationPenalty_Violation");
        });

        modelBuilder.Entity<WaterFee>(entity =>
        {
            entity.HasKey(e => e.WaterFeeId).HasName("PK__WaterFee__6141561E8B69D269");

            entity.ToTable("WaterFee");
        });

        modelBuilder.Entity<WaterInvoice>(entity =>
        {
            entity.HasKey(e => e.WaterInvoiceId).HasName("PK__WaterInv__7E28563248CF46C1");

            entity.ToTable("WaterInvoice");

            entity.HasOne(d => d.Apartment).WithMany(p => p.WaterInvoices)
                .HasForeignKey(d => d.ApartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WaterInvoice_Apartment");

            entity.HasOne(d => d.Invoice).WithMany(p => p.WaterInvoices)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WaterInvoice_Invoice");

            entity.HasOne(d => d.WaterFee).WithMany(p => p.WaterInvoices)
                .HasForeignKey(d => d.WaterFeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WaterInvoice_WaterFee");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
