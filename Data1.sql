-- Bảng Role
-- INSERT INTO Role (RoleName, Description) VALUES 
-- ('Administrator', N'Quản trị hệ thống'),
-- ('Accountant', N'Quản lý kế toán'),
-- ('ServiceSupervisor', N'Nhân viên quản lý dịch vụ');

-- Bảng Account
-- INSERT INTO Account (Username, Password, FullName, RoleId, IsDeleted) VALUES 
-- ('admin', 'admin123', N'Nguyễn Văn A', 1, 0),
-- ('accountant', 'account123', N'Lê Thị B', 2, 0),
-- ('service1', 'service123', N'Trần Văn C', 3, 0),
-- ('service2', 'service456', N'Ngô Thị D', 3, 0);

-- Bảng Department
INSERT INTO Department (DepartmentName, NumberOfStaff, Description, IsDeleted) VALUES
(N'Quản lý tài sản', 5, N'Quản lý thiết bị và tài sản chung', 0),
(N'Dịch vụ khách hàng', 3, N'Hỗ trợ cư dân và giải quyết khiếu nại', 0);

-- Bảng Area
-- INSERT INTO Area (AreaName, Location) VALUES 
-- (N'Khu Block A', N'Phía Đông'),
-- (N'Khu Block B', N'Phía Tây'),
-- (N'Khu Gửi Xe', N'Tầng hầm'),
-- (N'Khu Công Viên', N'Tầng trệt');


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
(2, 2, N'Hỏng bình khí', N'Thay thế bình khí', 2000000);

-- Bảng Maintenance
INSERT INTO Maintenance (MaintenanceId, MaintenanceName, MaintanaceDate, Description, Status, CreatedBy, DepartmentId) VALUES
(1, N'Kiểm tra định kỳ thiết bị', '2024-11-15', N'Kiểm tra hệ thống điện và cứu hỏa', N'Chưa hoàn thành', 1, 1),
(2, N'Bảo trì Block B', '2024-11-20', N'Kiểm tra hệ thống điều hòa', N'Đã hoàn thành', 2, 2);

-- Bảng EquipmentMaintanance
INSERT INTO EquipmentMaintanance (MaintenanceID, EquipmentID) VALUES
(1, 1),
(1, 2),
(2, 3);


-- Bảng Block
-- INSERT INTO Block (BlockCode, AreaId, NumberOfFloor, IsDeleted) VALUES
-- ('A1', 1, 15, 0),
-- ('A2', 1, 10, 0),
-- ('B1', 2, 12, 0),
-- ('B2', 2, 8, 0);

-- Bảng Floor
INSERT INTO Floor (FloorNumber, NumberOfApartment, BlockId) VALUES
(1, 10, 1), (2, 10, 1), (3, 10, 1), (4, 10, 1), (5, 10, 1),
(1, 8, 2), (2, 8, 2), (3, 8, 2), (4, 8, 2);

-- Bảng Apartment
INSERT INTO Apartment (ApartmentCode, ApartmentNumber, Area, NumberOfPeople, Status, FloorId) VALUES
('A1-101', 101, 100, 3, N'Đã bán', 1),
('A1-102', 102, 90, 4, N'Đã bán', 1),
('A1-201', 201, 80, 2, N'Chưa bán', 2),
('B1-101', 101, 120, 5, N'Đã bán', 6),
('B1-102', 102, 150, 3, N'Đã bán', 6);

-- Bảng Resident
INSERT INTO Resident (ResidentId, FullName, Gender, DateOfBirth, RelationShipWithOwner, MoveInDate, ApartmentID) VALUES
('111111111111', N'Nguyễn Văn E', N'Nam', '2010-07-05', N'Con', '2020-01-01', 1),
('222222222222', N'Trần Thị F', N'Nữ', '1987-02-17', N'Vợ', '2019-05-10', 2),
('333333333333', N'Lê Văn G', N'Nam', '1975-03-15', N'Chủ hộ', '2018-03-05', 3),
('444444444444', N'Nguyễn Thị H', N'Nữ', '2000-11-25', N'Chủ hộ', '2021-07-07', 4);


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
INSERT INTO Vehicle (VehicleId, VechicleOwner, ApartmentId, VehicleCategoryId) VALUES
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
INSERT INTO Regulation (Title, Content, CreatedDate) VALUES
(N'Nội quy chung cư', N'Không được xả rác bừa bãi', '2024-01-01'),
(N'Quy định an toàn', N'Không được hút thuốc tại khu vực công cộng', '2024-01-01');

-- Bảng Violation
INSERT INTO Violation (ApartmentId, RegulationId, CreatedDate, Detail) VALUES
(1, 1, '2024-11-01', N'Xả rác tại khu vực công cộng'),
(2, 2, '2024-11-05', N'Hút thuốc tại hành lang tầng trệt');
