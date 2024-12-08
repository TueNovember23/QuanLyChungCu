create database QuanLyChungCu
go
use QuanLyChungCu
go

CREATE TABLE Role (
    RoleId INT PRIMARY KEY IDENTITY(1, 1),
    RoleName VARCHAR(50) UNIQUE NOT NULL,
    Description NVARCHAR(255)
);

CREATE TABLE Account (
    AccountId INT PRIMARY KEY IDENTITY(1, 1),
    Username VARCHAR(50) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    RoleId INT NOT NULl,
    IsDeleted BIT DEFAULT 0 NOT NULL,
    CONSTRAINT FK_Account_Role FOREIGN KEY (RoleId) REFERENCES Role(RoleID) 
);

CREATE TABLE Department (
    DepartmentId INT PRIMARY KEY IDENTITY(1, 1),
    DepartmentName NVARCHAR(100) UNIQUE NOT NULL,
    NumberOfStaff INT DEFAULT 0 NOT NULL,
    Description NVARCHAR(255),
    IsDeleted BIT DEFAULT 0 NOT NULL
);

CREATE TABLE Area (
    AreaId INT PRIMARY KEY IDENTITY(1, 1),
    AreaName NVARCHAR(100) NOT NULL,
    Location NVARCHAR(100)
);

CREATE TABLE Equipment (
    EquipmentId INT PRIMARY KEY,
    EquipmentName NVARCHAR(100) NOT NULL,
    Discription NVARCHAR(255),
    AreaId INT NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT N'Hoạt động', -- Trạng thái thiết bị (hoạt động, hỏng),
    IsDeleted BIT DEFAULT 0 NOT NULL,
    CONSTRAINT FK_Equipment_Area FOREIGN KEY (AreaID) REFERENCES Area(AreaID)
);

-- Hóa đơn sửa chữa (join với báo cáo hư hỏng để lấy thông tin thiết bị hỏng, phương án sửa và giá)
CREATE TABLE RepairInvoice (
    InvoiceID INT PRIMARY KEY,
    InvoiceDate DATE DEFAULT GETDATE() NOT NULL,
    InvoiceContent NVARCHAR(255),
    TotalAmount FLOAT NOT NULL,
    Status NVARCHAR(50) DEFAULT N'Chưa thanh toán' NOT NULL, -- Trạng thái hóa đơn (đã thanh toán, chưa thanh toán)
    CreatedBy INT, -- Người tạo hóa đơn
    CONSTRAINT FK_RepairInvoice_Account FOREIGN KEY (CreatedBy) REFERENCES Account(AccountId)
);

-- Thiết bị hư hỏng (bảng n-n giữa thiêt bị và báo cáo hư hỏng)
CREATE TABLE MalfuntionEquipment (
    MalfuntionId INT PRIMARY KEY IDENTITY(1, 1),
    RepairInvoiceId INT,
    EquipmentId INT,
    Description NVARCHAR(100),
    SolvingMethod NVARCHAR(255), -- Phương pháp sửa chữa
    RepairPrice FLOAT NOT NULL,
    CONSTRAINT FK_MalfuntionEquipment_Equipment FOREIGN KEY (EquipmentID) REFERENCES Equipment(EquipmentID),
    CONSTRAINT FK_MalfuntionEquipment_RepairInvoice FOREIGN KEY (RepairInvoiceID) REFERENCES RepairInvoice(InvoiceID)
)

-- Lịch bảo trì
CREATE TABLE Maintenance (
    MaintenanceId INT PRIMARY KEY,
    MaintenanceName NVARCHAR(100) NOT NULL,
	MaintanaceDate DATE DEFAULT GETDATE() NOT NULL,
    Description NVARCHAR(255),
    Status NVARCHAR(50) DEFAULT N'Chưa hoàn thành', -- Trạng thái bảo trì (đã hoàn thành, chưa hoàn thành)
    CreatedBy INT NOT NULL, -- Người tạo bảo trì
    DepartmentId INT NOT NULL, -- Bộ phận thực hiện bảo trì
    CONSTRAINT FK_Maintenance_Department FOREIGN KEY (DepartmentID) REFERENCES Department(DepartmentID),
    CONSTRAINT FK_Maintenance_Account FOREIGN KEY (CreatedBy) REFERENCES Account(AccountId)
);

-- Bảng n-n giữa thiết bị và bảo trì
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
    IsDeleted bit DEFAULT 0,
    CONSTRAINT FK_Block_Area FOREIGN KEY (AreaId) REFERENCES Area(AreaID)
)

create table Floor (
    FloorId int identity(1, 1) primary key,
    FloorNumber int NOT NULL,
    NumberOfApartment int DEFAULT 0,
    BlockId int not null,
    constraint FK_Floor_Block foreign key (BlockId) references Block(BlockId)
)

create table Representative (
    RepresentativeId char(12) primary key,
    FullName nvarchar(50),
    Gender nchar(4),
    DateOfBirth date,
    Email nvarchar(100),
    PhoneNumber char(10)
)

Create Table Apartment (
	ApartmentId int IDENTITY(1, 1) primary key, -- Id căn hộ (ẩn khỏi người dùng)
    ApartmentCode char(20), --Mã căn hộ
    ApartmentNumber int, -- Số thứ tự của căn hộ tại 1 lầu
	Area int,
    NumberOfPeople int DEFAULT 0,
    RepresentativeId char(12),
	Status nvarchar(20),
	FloorId int not null,
    Constraint FK_Apartment_Floor foreign key (FloorId) references Floor(FloorId),
    Constraint FK_Apartment_Representative foreign key (RepresentativeId) references Representative(RepresentativeId)
)

create table Resident (
    ResidentId char(12) primary key,
    FullName nvarchar(50) NOT NULL,
    Gender nchar(4),
    DateOfBirth date,
    RelationShipWithOwner nvarchar(50),
    MoveInDate date,
    MoveOutDate date,
    ApartmentID int not null,
    constraint FK_Resident_Apartment foreign key (ApartmentId) references Apartment(ApartmentId),
)

create table VehicleCategory (
    VehicleCategoryId int identity(1, 1) primary key,
    CategoryName nvarchar(50),
    MonthlyFee FLOAT
)

create table Vehicle (
    VehicleId char(20) PRIMARY KEY, -- Biển số xe
    VehicleOwner nvarchar(50), -- Tên chủ xe,   tên cũ là VechicleOwner, vừa sửa thành VehicleOwner
    Status nvarchar(20), -- Trạng thái xe (đang gửi, đã hủy)
    ApartmentId int not null,
    VehicleCategoryId int not null,
    constraint FK_Vehicle_VehicleCategory foreign key (VehicleCategoryId) references VehicleCategory(VehicleCategoryId),
    constraint FK_Vehicle_Apartment foreign key (ApartmentId) references Apartment(ApartmentId)
)

create table Invoice (
    InvoiceId int identity(1, 1) primary key,
    CreatedDate date DEFAULT GETDATE(),
    CreatedBy int NOT NULL,
    Month int DEFAULT MONTH(GETDATE()),
    Year int DEFAULT YEAR(GETDATE()),
    TotalAmount FLOAT,
    Status nvarchar(20) DEFAULT N'Chưa thanh toán',
    CONSTRAINT FK_Invoice_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Account(AccountId)
)

-- Giá nước
CREATE TABLE WaterFee (
    WaterFeeId int IDENTITY(1, 1) PRIMARY KEY,
    CreatedDate date,
    DeletedDate date, -- Khi xóa thì thêm ngày xóa
    Level1 int, -- Định mức bậc 1 (m3/người)
    Price1 FLOAT,
    Level2 int, -- Định mức bậc 2 (m3/người)
    Price2 FLOAT,
    Price3 FLOAT -- Giá nước vượt định mức 2
)

CREATE TABLE WaterInvoice (
    WaterInvoiceId int IDENTITY(1, 1) PRIMARY KEY,
    StartIndex int not NULL,
    EndIndex int not null,
    NumberOfPeople int not null, -- Lưu lại để tránh số người thay đổi
    TotalAmount FLOAT,
    InvoiceId int not null,
    ApartmentId int not null,
    WaterFeeId int not null,
    CONSTRAINT FK_WaterInvoice_Invoice FOREIGN KEY (InvoiceId) REFERENCES Invoice(InvoiceId),
    CONSTRAINT FK_WaterInvoice_Apartment FOREIGN KEY (ApartmentId) REFERENCES Apartment(ApartmentId),
    CONSTRAINT FK_WaterInvoice_WaterFee FOREIGN KEY (WaterFeeId) REFERENCES WaterFee(WaterFeeId)
)

create table ManagementFee (
    ManagementFeeId int identity(1, 1) primary key,
    CreatedDate date,
    DeletedDate date,
    Price FLOAT -- Đơn giá theo m2
)

CREATE TABLE ManagementFeeInvoice (
    ManagementFeeInvoiceId int IDENTITY(1, 1) PRIMARY KEY,
    Price FLOAT not null,
    TotalAmount FLOAT,
    InvoiceId int not null,
    ApartmentId int not null,
    ManagementFeeHistoryId int not null,
    constraint FK_ManagementFeeInvoice_Invoice foreign key (InvoiceId) references Invoice(InvoiceId),
    constraint FK_ManagementFeeInvoice_Apartment foreign key (ApartmentId) references Apartment(ApartmentId),
    constraint FK_ManagementFeeInvoice_ManagementFeeHistory foreign key (ManagementFeeHistoryId) references ManagementFee(ManagementFeeId)
)

CREATE TABLE VechicleInvoice (
    VechicleInvoiceId int IDENTITY(1, 1) PRIMARY KEY,
    TotalAmount FLOAT,
    InvoiceId int not null,
    ApartmentId int not null,
    constraint FK_VechicleInvoice_Invoice foreign key (InvoiceId) references Invoice(InvoiceId),
    constraint FK_VechicleInvoice_Apartment foreign key (ApartmentId) references Apartment(ApartmentId)
)

-- Chi tiết hóa đơn xe (bảng n-n giữa hóa đơn xe và xe)
CREATE TABLE VechicleInvoiceDetail (
    VechicleInvoiceId int not null,
    VehicleId char(20) not null,
    Price FLOAT not null, -- Giá tiền lấy từ bảng VehicleCategory
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
    CreatedDate date DEFAULT GETDATE() NOT NULL,
    Category nvarchar(20) NOT NULL DEFAULT N'Khác',
    Priority nvarchar(20) NOT NULL DEFAULT N'Trung bình',
    IsActive bit NOT NULL DEFAULT 1,
    CONSTRAINT CK_Regulation_Category CHECK (Category IN (N'An ninh', N'Vệ sinh', N'Phòng cháy', N'Sinh hoạt', N'Khác')),
    CONSTRAINT CK_Regulation_Priority CHECK (Priority IN (N'Cao', N'Trung bình', N'Thấp'))
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

-- cập nhật số tầng của block khi thêm tầng
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


-- cập nhật số căn hộ của tầng khi thêm căn hộ
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

-- cập nhật số người ở của căn hộ khi thêm cư dân
CREATE TRIGGER TRG_Resident_Insert
ON Resident
AFTER INSERT
AS
BEGIN
    UPDATE Apartment
    SET NumberOfPeople = NumberOfPeople + i.ResidentCount
    FROM (
        SELECT ApartmentID, COUNT(*) AS ResidentCount
        FROM inserted
        GROUP BY ApartmentID
    ) AS i
    WHERE Apartment.ApartmentId = i.ApartmentID;
END;
GO

-- tự động tạo mã căn hộ
CREATE TRIGGER TRG_Apartment_Insert_AutoCode
ON Apartment
AFTER INSERT
AS
BEGIN
    UPDATE Apartment
    SET ApartmentCode = CONCAT(
        f.FloorNumber, '.', i.ApartmentNumber, ' Block ', b.BlockCode
    )
    FROM inserted i
    INNER JOIN Floor f ON i.FloorId = f.FloorId
    INNER JOIN Block b ON f.BlockId = b.BlockId
    WHERE Apartment.ApartmentId = i.ApartmentId;
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
GO

-- Chèn 24 tầng cho mỗi block
BEGIN
    DECLARE @BlockId INT;
    DECLARE @FloorNumber INT;

    SET @BlockId = 1; -- ID của block bắt đầu
    SET @FloorNumber = 1; -- Số tầng bắt đầu

    WHILE @BlockId <= (SELECT MAX(BlockId) FROM Block) -- Lặp qua tất cả các block
    BEGIN
        SET @FloorNumber = 1; -- Reset số tầng

        WHILE @FloorNumber <= 10 -- Mỗi block có 24 tầng
        BEGIN
            INSERT INTO Floor (FloorNumber, BlockId)
            VALUES (@FloorNumber, @BlockId);

            SET @FloorNumber = @FloorNumber + 1;
        END

        SET @BlockId = @BlockId + 1; -- Chuyển sang block tiếp theo
    END;
END;
GO

BEGIN
    DECLARE @FloorId INT;
    DECLARE @MaxFloorId INT;
    DECLARE @ApartmentNumber INT;

    SET @FloorId = (SELECT MIN(FloorId) FROM Floor); -- ID tầng bắt đầu
    SET @MaxFloorId = (SELECT MAX(FloorId) FROM Floor); -- ID tầng kết thúc

    WHILE @FloorId <= @MaxFloorId -- Lặp qua tất cả các tầng
    BEGIN
        SET @ApartmentNumber = 1; -- Reset số thứ tự căn hộ

        WHILE @ApartmentNumber <= 10 -- Mỗi tầng có 10 căn hộ
        BEGIN
            INSERT INTO Apartment (ApartmentNumber, Area, Status, FloorId)
            VALUES (
                @ApartmentNumber, -- Số thứ tự căn hộ
                100,              -- Diện tích căn hộ, giả sử mặc định là 100
                N'Đã bán',         -- Trạng thái mặc định là "Trống"
                @FloorId          -- Tầng tương ứng
            );

            SET @ApartmentNumber = @ApartmentNumber + 1; -- Tăng số thứ tự căn hộ
        END;

        SET @FloorId = @FloorId + 1; -- Chuyển sang tầng tiếp theo
    END;
END;
GO

INSERT INTO Resident (ResidentId, FullName, Gender, DateOfBirth, RelationShipWithOwner, MoveInDate, MoveOutDate, ApartmentId)
VALUES ('123445678912', N'Nguyễn Thị D', N'Nữ', '1965-01-01', N'Chủ hộ', '2015-03-01', '2023-01-03', 3),
('123445678913', N'Nguyễn Thị E', N'Nữ', '1995-01-01', N'Con', '2015-03-01', NULL, 3),
('123445678914', N'Nguyễn Văn F', N'Nam', '1995-01-01', N'Em', '2015-03-01', NULL, 3),
('123456789012', N'Nguyễn Văn A', N'Nam', '1990-01-01', N'Chủ hộ', '2015-01-01', NULL, 1),
('123345678910', N'Nguyễn Thị B', N'Nữ', '1991-01-01', N'Vợ', '2015-01-01', NULL, 1),
('123445678910', N'Nguyễn Thị B', N'Nữ', '2015-01-01', N'Con', '2015-03-01', NULL, 1),
('123445678911', N'Nguyễn Văn C', N'Nam', '1960-01-01', N'Cha', '2015-03-01', '2023-01-03', 1),
('123445678912', N'Nguyễn Thị D', N'Nữ', '1965-01-01', N'Chủ hộ', '2015-03-01', '2023-01-03', 3),
('123445678913', N'Nguyễn Thị E', N'Nữ', '1995-01-01', N'Con', '2015-03-01', NULL, 3),
('123445678914', N'Nguyễn Văn F', N'Nam', '1995-01-01', N'Em', '2015-03-01', NULL, 3),
('105456789012', N'Nguyễn Thị X', N'Nữ', '1991-01-01', N'Chủ hộ', '2015-01-01', NULL, 2)
UPDATE Apartment
SET NumberOfPeople = 3
WHERE ApartmentId = 1

INSERT INTO Representative (RepresentativeId, FullName, Gender, DateOfBirth, Email, PhoneNumber)
VALUES
('123456789012', N'Nguyễn Văn A', N'Nam', '1990-01-01', 'nguyenvana@mail.com', '0123456789'),
('105456789012', N'Nguyễn Thị X', N'Nữ', '1991-01-01', 'nguyenthix@mail.com', '0987123456'),
('123445678912', N'Nguyễn Thị D', N'Nữ', '1965-01-01', 'nguyenthid@mail.com', '0987123455')

UPDATE Apartment
SET RepresentativeId = '123456789012' WHERE ApartmentId = 1
UPDATE Apartment
SET RepresentativeId = '105456789012' WHERE ApartmentId = 2
UPDATE Apartment
SET RepresentativeId = '123445678912' WHERE ApartmentId = 3

INSERT INTO Department (DepartmentName, NumberOfStaff, Description) 
VALUES
(N'Kế toán', 5, N'Phòng kế toán'),
(N'Kỹ thuật', 10, N'Phòng kỹ thuật'),
(N'An ninh', 15, N'Phòng an ninh'),
(N'Vệ sinh', 20, N'Phòng vệ sinh'),
(N'Hành chính', 25, N'Phòng hành chính')

INSERT INTO Maintenance (MaintenanceId, MaintenanceName, MaintanaceDate, Description, CreatedBy, DepartmentId)
VALUES
    (1, N'Bảo trì thang máy', '2024-1-1', N'Bảo trì thang máy', 1, 2),
    (2, N'Bảo trì hệ thống điện', '2024-1-1', N'Bảo trì hệ thống điện hàng tháng', 1, 2),
    (3, N'Bảo trì hệ thống nước', '2024-1-1', N'Bảo trì hệ thống nước hàng tháng', 1, 2),
    (4, N'Bảo trì hệ thống thông gió', '2024-1-1', N'Bảo trì hệ thống thông gió hàng tháng', 1, 2),
    (5, N'Bảo trì hệ thống an ninh', '2024-1-1', N'Bảo trì hệ thống an ninh hàng tháng', 1, 2),
    (6, N'Vệ sinh khu vực chung', '2024-1-1', N'Vệ sinh khu vực chung hàng tháng', 1, 4);

INSERT INTO Equipment (EquipmentId, EquipmentName, Discription, AreaId, Status)
VALUES
(1, N'Máy bơm nước', N'Dùng để bơm nước cho toàn bộ tòa nhà', 1, N'Hoạt động'),
(2, N'Máy phát điện', N'Cung cấp điện dự phòng cho tòa nhà', 2, N'Hoạt động'),
(3, N'Hệ thống chiếu sáng', N'Đèn chiếu sáng hành lang', 3, N'Hỏng'),
(4, N'Hệ thống camera', N'Camera an ninh', 2, N'Hoạt động'),
(5, N'Thiết bị chữa cháy', N'Bình chữa cháy', 4, N'Hoạt động'),
(6, N'Hệ thống thoát nước', N'Ống thoát nước', 4, N'Hỏng'),
(7, N'Thang máy', N'Thang máy block E', 1, N'Hoạt động');

-- GO
-- ALTER DATABASE [QuanLyChungCu] SET  SINGLE_USER WITH ROFLLBACK IMMEDIATE
-- GO
-- USE [master]
-- GO
-- DROP DATABASE [QuanLyChungCu]
-- GO
-- EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'QuanLyChungCu'
-- GO

