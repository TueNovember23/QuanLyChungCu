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

Create Table Apartment (
	ApartmentId int IDENTITY(1, 1) primary key, -- Id căn hộ (ẩn khỏi người dùng)
    ApartmentCode char(20), --Mã căn hộ
    ApartmentNumber int, -- Số thứ tự của căn hộ tại 1 lầu
	Area int,
    NumberOfPeople int DEFAULT 0,
	Status nvarchar(20),
	FloorId int not null,
    Constraint FK_Apartment_Floor foreign key (FloorId) references Floor(FloorId)
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
    constraint FK_Resident_Apartment foreign key (ApartmentId) references Apartment(ApartmentId)
)

create table Representative (
    RepresentativeId char(12) primary key,
    FullName nvarchar(50),
    Gender nchar(4),
    DateOfBirth date,
    Email nvarchar(100),
    PhoneNumber char(10),
    ApartmentID int not null,
    constraint FK_Representative_Apartment foreign key (ApartmentId) references Apartment(ApartmentId)
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
    ManagementFeeInvoiceId int IDENTITY(1, 1) NOT NULL,
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
<<<<<<< HEAD
    Category nvarchar(50) NOT NULL,
    Priority nvarchar(20) NOT NULL,
    IsActive bit NOT NULL DEFAULT 1,
    CreatedDate date DEFAULT GETDATE() NOT NULL
=======
    CreatedDate date DEFAULT GETDATE() NOT NULL,
    Category nvarchar(20) NOT NULL DEFAULT N'Khác',
    Priority nvarchar(20) NOT NULL DEFAULT N'Trung bình',
    IsActive bit NOT NULL DEFAULT 1,
    CONSTRAINT CK_Regulation_Category CHECK (Category IN (N'An ninh', N'Vệ sinh', N'Phòng cháy', N'Sinh hoạt', N'Khác')),
    CONSTRAINT CK_Regulation_Priority CHECK (Priority IN (N'Cao', N'Trung bình', N'Thấp'))
>>>>>>> bd1aeb35a4d7a953502912186c93c2d7a9349abc
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

-- USE [master]
-- GO
-- ALTER DATABASE [QuanLyChungCu] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
-- GO
-- USE [master]
-- GO
-- DROP DATABASE [QuanLyChungCu]
-- GO
-- EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'QuanLyChungCu'
-- GO


-------------------------------------- Nhâp dữ liệu --------------------------------------

-- Bảng Department
INSERT INTO Department (DepartmentName, NumberOfStaff, Description, IsDeleted) VALUES
(N'Quản lý tài sản', 5, N'Quản lý thiết bị và tài sản chung', 0),
(N'Dịch vụ khách hàng', 3, N'Hỗ trợ cư dân và giải quyết khiếu nại', 0);


-- Bảng Equipment
INSERT INTO Equipment (EquipmentId, EquipmentName, Discription, AreaId, Status, IsDeleted) VALUES
(1, N'Máy phát điện', N'Máy phát điện dự phòng', 1, N'Hoạt động', 0),
(2, N'Thiết bị cứu hỏa', N'Thiết bị chữa cháy cho Block A', 1, N'Hoạt động', 0),
(3, N'Máy điều hòa trung tâm', N'Máy điều hòa Block D', 2, N'Hỏng', 0);

-- Bảng RepairInvoice
INSERT INTO RepairInvoice (InvoiceID, InvoiceDate, InvoiceContent, TotalAmount, Status, CreatedBy) VALUES
(1, '2024-11-01', N'Sửa chữa máy phát điện', 5000000, N'Chưa thanh toán', 1),
(2, '2024-11-05', N'Sửa chữa thiết bị cứu hỏa', 2000000, N'Đã thanh toán', 2);

-- Bảng MalfuntionEquipment
INSERT INTO MalfuntionEquipment (RepairInvoiceId, EquipmentId, Description, SolvingMethod, RepairPrice) VALUES
(1, 1, N'Hỏng động cơ', N'Thay động cơ mới', 5000000),
(2, 2, N'H���ng bình khí', N'Thay thế bình khí', 2000000);

-- Bảng Maintenance
INSERT INTO Maintenance (MaintenanceId, MaintenanceName, MaintanaceDate, Description, Status, CreatedBy, DepartmentId) VALUES
(1, N'Kiểm tra định kỳ thiết bị', '2024-11-15', N'Kiểm tra hệ thống điện và cứu hỏa', N'Chưa hoàn thành', 1, 1),
(2, N'Bảo trì Block B', '2024-11-20', N'Kiểm tra hệ thống điều hòa', N'Đã hoàn thành', 2, 2);

-- Bảng EquipmentMaintanance
INSERT INTO EquipmentMaintanance (MaintenanceID, EquipmentID) VALUES
(1, 1),
(1, 2),
(2, 3);

-- Bảng Floor
-- INSERT INTO Floor (FloorNumber, NumberOfApartment, BlockId) VALUES
-- (1, 10, 1), (2, 10, 1), (3, 10, 1), (4, 10, 1), (5, 10, 1),
-- (1, 8, 2), (2, 8, 2), (3, 8, 2), (4, 8, 2);

-- Bảng Apartment
INSERT INTO Apartment (ApartmentCode, ApartmentNumber, Area, NumberOfPeople, Status, FloorId) VALUES
('A1-101', 101, 100, 3, N'Đã bán', 1),
('A1-102', 102, 90, 4, N'Đã bán', 1),
('A1-201', 201, 80, 2, N'Chưa bán', 2),
('B1-101', 101, 120, 5, N'Đã bán', 6),
('B1-102', 102, 150, 3, N'Đã bán', 6);

-- Bảng Resident
INSERT INTO Resident (ResidentId, FullName, Gender, DateOfBirth, RelationShipWithOwner, MoveInDate, MoveOutDate, ApartmentID) VALUES
('111111111111', N'Nguyễn Văn E', N'Nam', '2010-07-05', N'Con', '2020-01-01', NULL, 1),
('222222222222', N'Trần Thị F', N'Nữ', '1987-02-17', N'Vợ', '2019-05-10', NULL, 2),
('333333333333', N'Lê Văn G', N'Nam', '1975-03-15', N'Chủ hộ', '2018-03-05', NULL, 3),
('444444444444', N'Nguyễn Thị H', N'Nữ', '2000-11-25', N'Chủ hộ', '2021-07-07', NULL, 4);


-- Bảng Representative
INSERT INTO Representative (RepresentativeId, FullName, Gender, DateOfBirth, Email, PhoneNumber, ApartmentID) VALUES
('012345678901', N'Nguyễn Văn A', N'Nam', '1980-05-10', 'nguyenvana@example.com', '0912345678', 1),
('123456789012', N'Trần Thị B', N'Nữ', '1985-09-15', 'tranthib@example.com', '0911223344', 2),
('234567890123', N'Lê Văn C', N'Nam', '1990-12-01', 'levanc@example.com', '0922334455', 3),
('345678901234', N'Nguyễn Thị D', N'Nữ', '1995-06-20', 'nguyenthid@example.com', '0933445566', 4);


-- Bảng VehicleCategory
INSERT INTO VehicleCategory (CategoryName, MonthlyFee) VALUES
(N'Xe đạp', 50000),
(N'Xe máy', 100000),
(N'Ô tô', 200000),
(N'Xe máy điện', 120000),
(N'Ô tô điện', 250000);

-- Bảng Vehicle
INSERT INTO Vehicle (VehicleId, VehicleOwner, ApartmentId, VehicleCategoryId) VALUES
('59D1-12345', N'Nguyễn Văn A', 1, 2),
('30A-67890', N'Trần Thị B', 2, 3),
('92F1-34567', N'Lê Văn C', 3, 2),
('51G-12345', N'Nguyễn Thị D', 4, 5);

-- Bảng Invoice
INSERT INTO Invoice (CreatedDate, CreatedBy, Month, Year, TotalAmount, Status) VALUES
(GETDATE(), 1, 11, 2024, 300000, N'Chưa thanh toán'),
(GETDATE(), 3, 11, 2024, 500000, N'Đã thanh toán');


-- Bảng WaterFee
INSERT INTO WaterFee (CreatedDate, DeletedDate, Level1, Price1, Level2, Price2, Price3) VALUES
('2024-01-01', NULL, 10, 5000, 20, 7000, 10000),
('2024-07-01', NULL, 10, 6000, 20, 8000, 11000);

-- Bảng WaterInvoice
INSERT INTO WaterInvoice (StartIndex, EndIndex, NumberOfPeople, TotalAmount, InvoiceId, ApartmentId, WaterFeeId) VALUES
(100, 150, 3, 75000, 1, 1, 1),
(200, 250, 5, 125000, 2, 2, 1);

-- Bảng ManagementFee
INSERT INTO ManagementFee (CreatedDate, DeletedDate, Price) VALUES
('2024-01-01', NULL, 10000),
('2024-07-01', NULL, 12000);

-- Bảng ManagementFeeInvoice
INSERT INTO ManagementFeeInvoice (Price, TotalAmount, InvoiceId, ApartmentId, ManagementFeeHistoryId) VALUES
(10000, 1200000, 1, 1, 1),
(12000, 1440000, 2, 2, 2);


-- Bảng VechicleInvoice
INSERT INTO VechicleInvoice (TotalAmount, InvoiceId, ApartmentId) VALUES
(100000, 1, 1),
(200000, 2, 2);

-- Bảng VechicleInvoiceDetail
INSERT INTO VechicleInvoiceDetail (VechicleInvoiceId, VehicleId, Price) VALUES
(1, '59D1-12345', 100000),
(2, '30A-67890', 200000);


-- Bảng CommunityRoom
INSERT INTO CommunityRoom (RoomName, RoomSize, Location) VALUES
(N'Phòng họp Block A', 30, N'Tầng 1 Block A'),
(N'Sảnh cư dân Block B', 50, N'Tầng trệt Block B');

-- Bảng CommunityRoomBooking
INSERT INTO CommunityRoomBooking (BookingDate, StartTime, EndTime, NumberOfPeople, ApartmentId, CommunityRoomId) VALUES
('2024-11-10', '08:00', '10:00', 20, 1, 1),
('2024-11-15', '18:00', '20:00', 30, 2, 2);


-- Bảng Regulation
INSERT INTO Regulation (Title, Content, Category, Priority, IsActive, CreatedDate)
VALUES 
    (N'Quy định về phòng cháy chữa cháy', N'Các hộ dân cần tuân thủ nghiêm ngặt quy định PCCC', N'Phòng cháy', N'Cao', 1, '2024-11-01'),
    (N'Quy định về đổ rác', N'Đổ rác đúng giờ và đúng nơi quy định', N'Vệ sinh', N'Trung bình', 1, '2024-10-22'),
    (N'Quy định về an ninh', N'Đăng ký khách qua đêm với bảo vệ', N'An ninh', N'Cao', 1, '2024-08-31'),
    (N'Quy định về giữ xe', N'Gửi xe đúng vị trí được phân công', N'Sinh hoạt', N'Trung bình', 1, '2022-10-08'),
    (N'Quy định về nuôi thú cưng', N'Đăng ký thú cưng với ban quản lý', N'Khác', N'Thấp', 1, '2024-12-05'),
    (N'Quy định về sử dụng thang máy', N'Không để trẻ em đi thang máy một mình', N'An ninh', N'Cao', 1, '2024-01-15');


-- Bảng Violation
INSERT INTO Violation (ApartmentId, RegulationId, CreatedDate, Detail) VALUES
(1, 1, '2024-11-01', N'Xả rác tại khu vực công cộng'),
(2, 2, '2024-11-05', N'Hút thuốc tại hành lang tầng trệt'),
(3, 3, '2024-11-10', N'Vào khu vực hạn chế không có quyền truy cập');



-- Thêm cột IsDeleted column cho bảng Apartment
ALTER TABLE Apartment
ADD IsDeleted bit NOT NULL DEFAULT 0;
