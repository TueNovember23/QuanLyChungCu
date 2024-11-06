create database QuanLyChungCu
go
use QuanLyChungCu
go
-- drop database QuanLyChungCu
CREATE TABLE Role (
    RoleId INT PRIMARY KEY IDENTITY(1, 1),
    RoleName VARCHAR(50) UNIQUE NOT NULL,
    Description VARCHAR(255)
);

CREATE TABLE Account (
    Username NVARCHAR(50) PRIMARY KEY,
    Password NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    RoleId INT NOT NULl,
    IsDeleted BIT DEFAULT 0 NOT NULL,
    CONSTRAINT FK_Account_Role FOREIGN KEY (RoleId) REFERENCES Role(RoleID) 
);

CREATE TABLE Area (
    AreaId INT PRIMARY KEY IDENTITY(1, 1),
    AreaName VARCHAR(100) NOT NULL,
    Location VARCHAR(100)
);

CREATE TABLE Department (
    DepartmentId INT PRIMARY KEY IDENTITY(1, 1),
    DepartmentName VARCHAR(100) UNIQUE NOT NULL,
    NumberOfStaff INT DEFAULT 0 NOT NULL,
    Description VARCHAR(255),
    IsDeleted BIT DEFAULT 0 NOT NULL
);

CREATE TABLE Equipment (
    EquipmentId INT PRIMARY KEY,
    EquipmentName VARCHAR(100) NOT NULL,
    Discription VARCHAR(255),
    AreaId INT NOT NULL,
    Status VARCHAR(50) NOT NULL DEFAULT N'Hoạt động', -- Trạng thái thiết bị (hoạt động, hỏng),
    IsDeleted BIT DEFAULT 0 NOT NULL,
    CONSTRAINT FK_Equipment_Area FOREIGN KEY (AreaID) REFERENCES Area(AreaID)
);

-- Báo cáo hư hỏng thiết bị
CREATE TABLE MalfunctionReport (
    ReportId INT PRIMARY KEY,
    ReportDate DATE DEFAULT GETDATE() NOT NULL,
    ReportContent VARCHAR(255),
    ReportStatus VARCHAR(50) DEFAULT N'Chưa xử lý' NOT NULL, -- Trạng thái báo cáo (đã xử lý, chưa xử lý)
    EquipmentId INT NOT NULL,
    CONSTRAINT FK_MalfunctionReport_Equipment FOREIGN KEY (EquipmentID) REFERENCES Equipment(EquipmentID)
);

CREATE TABLE RepairInvoice (
    InvoiceID INT PRIMARY KEY,
    InvoiceDate DATE DEFAULT GETDATE() NOT NULL,
    InvoiceContent VARCHAR(255),
    SolvingMethod VARCHAR(255), 
    RepairPrice FLOAT NOT NULL,
    Status VARCHAR(50) DEFAULT N'Chưa thanh toán' NOT NULL, -- Trạng thái hóa đơn (đã thanh toán, chưa thanh toán)
    MalfunctionReportID INT,
    DepartmentId INT,
    CONSTRAINT FK_RepairInvoice_Department FOREIGN KEY (DepartmentID) REFERENCES Department(DepartmentID),
    CONSTRAINT FK_RepairInvoice_MalfunctionReport FOREIGN KEY (MalfunctionReportID) REFERENCES MalfunctionReport(ReportId)
);


CREATE TABLE Maintenance (
    MaintenanceId INT PRIMARY KEY,
    MaintenanceName VARCHAR(100) NOT NULL,
	MaintanaceDate DATE DEFAULT GETDATE() NOT NULL,
    Description VARCHAR(255),
    Status VARCHAR(50) DEFAULT N'Chưa hoàn thành', -- Trạng thái bảo trì (đã hoàn thành, chưa hoàn thành)
    DepartmentId INT NOT NULL,
    CONSTRAINT FK_Maintenance_Department FOREIGN KEY (DepartmentID) REFERENCES Department(DepartmentID)
);

CREATE TABLE EquipmentMaintanance (
	MaintenanceID INT,
	EquipmentID INT,
	PRIMARY KEY (MaintenanceID, EquipmentID),
    CONSTRAINT FK_EquipmentMaintanance_Maintenance FOREIGN KEY (MaintenanceID) REFERENCES Maintenance(MaintenanceID),
    CONSTRAINT FK_EquipmentMaintanance_Equipment FOREIGN KEY (EquipmentID) REFERENCES Equipment(EquipmentID),
)

create table Block (
	BlockId INT identity(1, 1) primary key,
	BlockCode CHAR(2),
    AreaId int NOT NULL,
    NumberOfFloor int DEFAULT 0,
    IsDeleleted bit DEFAULT 0,
    CONSTRAINT FK_Block_Area FOREIGN KEY (AreaId) REFERENCES Area(AreaID)
)


create table Floor (
    FloorId int identity(1, 1) primary key,
    FloorNumber int NOT NULL,
    NumberOfApartment int DEFAULT 0,
    BlockId int not null,
    constraint FK_Floor_Block foreign key (BlockId) references Block(BlockId)
)

Create Table Apartment (
	ApartmentId int primary key, -- Id căn hộ (ẩn khỏi người dùng)
    ApartmentCode char(20), --Mã căn hộ
    ApartmentNumber int, -- Số thứ tự của căn hộ tại 1 lầu
	Area int,
    NumberOfPeople int,
	Status nvarchar(20),
	FloorId int not null,
    Constraint FK_Apartment_Floor foreign key (FloorId) references Floor(FloorId)
)

create table Resident (
    ResidentId char(12) primary key,
    FullName nvarchar(50) NOT NULL,
    Gender char(6),
    DateOfBirth date,
    RelationShipWithOwner nvarchar(50),
    MoveInDate date,
    ApartmentID int not null,
    constraint FK_Resident_Apartment foreign key (ApartmentId) references Apartment(ApartmentId)
)

create table Representative (
    RepresentativeId char(12) primary key,
    FullName nvarchar(50),
    Gender char(6),
    DateOfBirth date,
    Email nvarchar(100),
    PhoneNumber char(10),
    ApartmentID int not null,
    constraint FK_Representative_Apartment foreign key (ApartmentId) references Apartment(ApartmentId)
)

create table VehicleCategory (
    VehicleCategoryId int identity(1, 1) primary key,
    CategoryName nvarchar(50),
    MonthlyFee FLOAT,
    IsDeleleted BIT DEFAULT 0
)

create table Vehicle (
    VehicleId char(20) PRIMARY KEY,
    ApartmentId int not null,
    VehicleCategoryId int not null,
    constraint FK_Vehicle_VehicleCategory foreign key (VehicleCategoryId) references VehicleCategory(VehicleCategoryId),
    constraint FK_Vehicle_Apartment foreign key (ApartmentId) references Apartment(ApartmentId)
)

create table Invoice (
    InvoiceId int identity(1, 1) primary key,
    CreatedDate date DEFAULT GETDATE(),
    CreatedBy NVARCHAR(50) not null,
    Month int DEFAULT MONTH(GETDATE()),
    Year int DEFAULT YEAR(GETDATE()),
    TotalAmount FLOAT,
    Status nvarchar(20) DEFAULT N'Chưa thanh toán',
    CONSTRAINT FK_Invoice_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Account(Username)
)

CREATE TABLE WaterInvoice (
    WaterInvoiceId int IDENTITY(1, 1) Not NULL,
    StartIndex int not NULL,
    EndIndex int not null,
    Level  int not null,
    Price FLOAT not null,
    TotalAmount FLOAT,
    InvoiceId int not null,
    ApartmentId int not null,
    CONSTRAINT FK_WaterInvoice_Invoice FOREIGN KEY (InvoiceId) REFERENCES Invoice(InvoiceId),
    CONSTRAINT FK_WaterInvoice_Apartment FOREIGN KEY (ApartmentId) REFERENCES Apartment(ApartmentId)
)

CREATE TABLE WaterFeeHistory (
    CreatedDate date,
    DeletedDate date,
    -- Định mức bậc 1 (m3/người)
    Level1 int,
    Price1 FLOAT,
    -- Định mức bậc 2 (m3/người)
    Level2 int,
    Price2 FLOAT,
    -- Định mức bậc 3 (m3/người)
    Level3 int,
    Price3 FLOAT,
)

create table ManagementFeeHistory (
    CreatedDate date,
    DeletedDate date,
    Price FLOAT
)

CREATE TABLE ManagementFeeInvoice (
    ManagementFeeInvoiceId int IDENTITY(1, 1) NOT NULL,
    Price FLOAT not null,
    TotalAmount FLOAT,
    InvoiceId int not null,
    ApartmentId int not null,
    constraint FK_ManagementFeeInvoice_Invoice foreign key (InvoiceId) references Invoice(InvoiceId),
    constraint FK_ManagementFeeInvoice_Apartment foreign key (ApartmentId) references Apartment(ApartmentId)
)

CREATE TABLE VechicleInvoice (
    VechicleInvoiceId int IDENTITY(1, 1) PRIMARY KEY,
    TotalAmount FLOAT,
    InvoiceId int not null,
    ApartmentId int not null,
    constraint FK_VechicleInvoice_Invoice foreign key (InvoiceId) references Invoice(InvoiceId),
    constraint FK_VechicleInvoice_Apartment foreign key (ApartmentId) references Apartment(ApartmentId)
)

CREATE TABLE VechicleInvoiceDetail (
    VechicleInvoiceId int not null,
    VehicleId char(20) not null,
    Price FLOAT not null,
    constraint FK_VechicleInvoiceDetail_VechicleInvoice foreign key (VechicleInvoiceId) references VechicleInvoice(VechicleInvoiceId),
    constraint FK_VechicleInvoiceDetail_Vehicle foreign key (VehicleId) references Vehicle(VehicleId)
)

create table CommunityRoom (
    CommunityRoomId int IDENTITY(1, 1) PRIMARY KEY,
    RoomName nvarchar(50) NOT NULL,
    RoomSize int DEFAULT 0 NOT NULL, -- Sức chứa người
    Location nvarchar(100)
)

create table CommunityRoomBooking (
    CommunityRoomBookingId int identity(1, 1) primary key,
    BookingDate date DEFAULT GETDATE() NOT NULL,
    StartTime time,
    EndTime time,
    NumberOfPeople int DEFAULT 1 Not NULL,
    ApartmentId int not null,
    CommunityRoomId int not null,
    constraint FK_CommunityRoomBooking_Apartment foreign key (ApartmentId) references Apartment(ApartmentId),
    constraint FK_CommunityRoomBooking_CommunityRoom foreign key (CommunityRoomId) references CommunityRoom(CommunityRoomId)
)

CREATE TABLE Regulation (
    RegulationId int IDENTITY(1, 1) PRIMARY KEY,
    Title nvarchar(100) NOT NULL,
    Content nvarchar(255) NOT NULL,
    CreatedDate date DEFAULT GETDATE() NOT NULL
)

CREATE TABLE Violation (
    ViolationId int IDENTITY(1, 1) PRIMARY KEY,
    ApartmentId int not null,
    RegulationId int not null,
    CreatedDate date DEFAULT GETDATE() NOT NULL,
    Detail nvarchar(255),
    constraint FK_Violation_Apartment foreign key (ApartmentId) references Apartment(ApartmentId),
    constraint FK_Violation_Regulation foreign key (RegulationId) references Regulation(RegulationId)
)
GO

-- viết trigger tự động cập nhật số tầng của block khi thêm tầng
CREATE TRIGGER TRG_Floor_Insert
ON Floor
AFTER INSERT
AS
BEGIN
    UPDATE Block
    SET NumberOfFloor = NumberOfFloor + i.FloorCount
    FROM (
        SELECT BlockId, COUNT(*) AS FloorCount
        FROM inserted
        GROUP BY BlockId
    ) AS i
    WHERE Block.BlockId = i.BlockId;
END;
GO


-- viết trigger cập nhật số căn hộ của tầng khi thêm căn hộ
CREATE TRIGGER TRG_Apartment_Insert
ON Apartment
AFTER INSERT
AS
BEGIN
    UPDATE Floor
    SET NumberOfApartment = NumberOfApartment + i.ApartmentCount
    FROM (
        SELECT FloorId, COUNT(*) AS ApartmentCount
        FROM inserted
        GROUP BY FloorId
    ) AS i
    WHERE Floor.FloorId = i.FloorId;
END;
GO

INSERT INTO ROLE (RoleName, Description) VALUES ('Administrator', N'Quản trị hệ thống');
INSERT INTO ROLE (RoleName, Description) VALUES ('Accountant', N'Quản lý chung cư');
INSERT INTO ROLE (RoleName, Description) VALUES ('ServiceSupervisor', N'Nhân viên quản lý chung cư');

INSERT INTO Account (Username, Password, FullName, RoleId) VALUES ('ad', '1', N'Nhân viên hành chính 1', 1);
INSERT INTO Account (Username, Password, FullName, RoleId) VALUES ('ac', '1', N'Kế toán 1', 2);
INSERT INTO Account (Username, Password, FullName, RoleId) VALUES ('se', '1', N'Dịch vụ 1', 3);

insert into area (AreaName, Location) values (N'Khu vực Block E', 'Block E')
insert into area (AreaName, Location) values (N'Khu vực Block D', 'Block D')
insert into area (AreaName, Location) values (N'Khu vực Gửi xe', N'Phía trước Block D')
insert into area (AreaName, Location) values (N'Khu vực Khuôn viên', N'Khuôn viên chung cư')

insert into BLOCK (BlockCode, AreaId) values ('E1', 1)
insert into BLOCK (BlockCode, AreaId) values ('E2', 1)
insert into BLOCK (BlockCode, AreaId) values ('D1', 2)
insert into BLOCK (BlockCode, AreaId) values ('D2', 2)


insert INTO Floor (FloorNumber, BlockId) VALUES (1, 1), (2, 1), (3, 1), (4, 1), (5, 1), (6, 1), (7, 1), (8, 1)
insert into floor (FloorNumber, BlockId) values (1, 2), (2, 2), (3, 2), (4, 2), (5, 2), (6, 2), (7, 2), (8, 2)
insert into floor (FloorNumber, BlockId) values (1, 3), (2, 3), (3, 3), (4, 3), (5, 3), (6, 3), (7, 3), (8, 3)
insert into floor (FloorNumber, BlockId) values (1, 4), (2, 4), (3, 4), (4, 4), (5, 4), (6, 4), (7, 4), (8, 4)

