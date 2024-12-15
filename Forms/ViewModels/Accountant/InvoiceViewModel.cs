using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forms.Views.Accountant;
using Repositories.Repositories.Entities;
using Services.DTOs.InvoiceDTO;
using Services.DTOs.LoginDTO;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Forms.ViewModels.Accountant
{
    public partial class InvoiceViewModel : ObservableObject
    {
        private readonly IInvoiceService _invoiceService;
        public LoginResponseDTO? User { get; set; }

        [ObservableProperty]
        private ObservableCollection<ResponseInvoiceDTO> totalInvoices = [];

        [ObservableProperty]
        private ObservableCollection<ResponseWaterInvoiceDTO> waterInvoices = [];

        [ObservableProperty]
        private ObservableCollection<ResponseManagementFeeInvoiceDTO> managementInvoices = [];

        [ObservableProperty]
        private ObservableCollection<ResponseVehicleInvoiceDTO> vehicleInvoices = [];

        [ObservableProperty]
        private int selectedMonth;

        [ObservableProperty]
        private int selectedYear;

        [ObservableProperty]
        private Apartment selectedApartment;

        [ObservableProperty]
        private bool isLoading;
        [ObservableProperty]
        private ObservableCollection<int> months = new ObservableCollection<int>();
        [ObservableProperty]
        private ObservableCollection<int> years = new ObservableCollection<int>();
        [ObservableProperty]
        private ObservableCollection<Apartment> apartments = new ObservableCollection<Apartment>();


        [ObservableProperty]
        private int startIndex;
        [ObservableProperty]
        private int endIndex;
        [ObservableProperty]
        private int numberOfPeople;
        [ObservableProperty]
        private double apartmentArea;
        [ObservableProperty]
        private double managementFeePrice;

        [ObservableProperty]
        private string apartmentCode;
        [ObservableProperty]
        private int invoiceId;

        public InvoiceViewModel(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
            InitializeFilters();
            _ = LoadLoadLoad();
        }

        private async Task LoadLoadLoad()
        {
            await LoadAllInvoiceTypesAsync();
            await LoadComboBoxData();
        }

        private async Task LoadComboBoxData()
        {
            try
            {
                for (int i = 1; i <= 12; i++)
                {
                    Months.Add(i);
                }

                int currentYear = DateTime.Now.Year;
                for (int i = 2010; i <= 2050; i++)
                {
                    Years.Add(i);
                }

                await LoadApartmentsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while loading combo box data: {ex.Message}");
            }
        }



        private async Task LoadApartmentsAsync()
        {
            var apartmentsList = await _invoiceService.GetApartmentsAsync();
            Apartments = new ObservableCollection<Apartment>(apartmentsList);

        }


        private void InitializeFilters()
        {
            SelectedMonth = DateTime.Now.Month;
            SelectedYear = DateTime.Now.Year;
        }

        private async Task LoadAllInvoiceTypesAsync()
        {
            IsLoading = true;
            try
            {
                var invoices = await _invoiceService.GetAllInvoices(SelectedMonth, SelectedYear);
                TotalInvoices = new ObservableCollection<ResponseInvoiceDTO>(invoices);
                // Tải các loại hóa đơn song song
                var waterTask = await _invoiceService.GetWaterInvoices(SelectedMonth, SelectedYear);
                var vehicleTask = await _invoiceService.GetVehicleInvoices(SelectedMonth, SelectedYear);
                var managementTask = await _invoiceService.GetManagementFeeInvoices(SelectedMonth, SelectedYear);

                // Cập nhật dữ liệu sau khi tải xong
                WaterInvoices = new ObservableCollection<ResponseWaterInvoiceDTO>(waterTask);
                VehicleInvoices = new ObservableCollection<ResponseVehicleInvoiceDTO>(vehicleTask);
                ManagementInvoices = new ObservableCollection<ResponseManagementFeeInvoiceDTO>(managementTask);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tải hóa đơn: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }


        [RelayCommand]
        private async Task GenerateInvoices()
        {
            IsLoading = true;
            try
            {
                var invoiceInputs = new List<InvoiceInputDTO>(); 
                await _invoiceService.GenerateInvoices(SelectedMonth, SelectedYear, invoiceInputs);
                await LoadAllInvoiceTypesAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CreateWaterInvoice()
        {
            IsLoading = true;
            try
            {
                await _invoiceService.GenerateWaterInvoices(invoiceId, new Apartment { ApartmentCode = apartmentCode }, startIndex, endIndex, numberOfPeople);
                await LoadAllInvoiceTypesAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CreateManagementFeeInvoice()
        {
            IsLoading = true;
            try
            {
                await _invoiceService.GenerateManagementFeeInvoices(invoiceId, new Apartment { ApartmentCode = apartmentCode }, apartmentArea, managementFeePrice);
                await LoadAllInvoiceTypesAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CreateVehicleInvoice()
        {
            IsLoading = true;
            try
            {
                await _invoiceService.GenerateVehicleInvoices(invoiceId, new Apartment { ApartmentCode = apartmentCode });
                await LoadAllInvoiceTypesAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void ViewDetailVehicle(int InvoiceId)
        {
            DetailVehicleInvoice f = new DetailVehicleInvoice(_invoiceService, InvoiceId);
            f.ShowDialog();
        }

        [RelayCommand]
        private async void CreateInvoice()
        {
            CreateInvoiceView f = new(_invoiceService);
            f.User = User;
            f.ShowDialog();
            await LoadLoadLoad();
        }

        [RelayCommand]
        public async Task Find()
        {
            IsLoading = true;
            try
            {
                var invoices = await _invoiceService.GetAllInvoices(SelectedMonth, SelectedYear);
                var waterTask = await _invoiceService.GetWaterInvoices(SelectedMonth, SelectedYear);
                var vehicleTask = await _invoiceService.GetVehicleInvoices(SelectedMonth, SelectedYear);
                var managementTask = await _invoiceService.GetManagementFeeInvoices(SelectedMonth, SelectedYear);

                if (SelectedApartment != null)
                {
                    invoices = invoices.Where(i => i.ApartmentCode == SelectedApartment.ApartmentCode).ToList();
                    waterTask = waterTask.Where(w => w.ApartmentCode == SelectedApartment.ApartmentCode).ToList();
                    vehicleTask = vehicleTask.Where(v => v.ApartmentCode == SelectedApartment.ApartmentCode).ToList();
                    managementTask = managementTask.Where(m => m.ApartmentCode == SelectedApartment.ApartmentCode).ToList();
                }

                TotalInvoices = new ObservableCollection<ResponseInvoiceDTO>(invoices);
                WaterInvoices = new ObservableCollection<ResponseWaterInvoiceDTO>(waterTask);
                VehicleInvoices = new ObservableCollection<ResponseVehicleInvoiceDTO>(vehicleTask);
                ManagementInvoices = new ObservableCollection<ResponseManagementFeeInvoiceDTO>(managementTask);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async void PrintInvoice(ResponseInvoiceDTO invoice)
        {
            if (invoice == null) return;

            var flowDocument = new FlowDocument();
            flowDocument.PagePadding = new Thickness(50);
            flowDocument.ColumnWidth = 1000;

            var headerParagraph = new Paragraph(new Run("HÓA ĐƠN CHI TIẾT"))
            {
                TextAlignment = TextAlignment.Center,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20)
            };
            flowDocument.Blocks.Add(headerParagraph);

            var infoParagraph = new Paragraph()
            {
                Margin = new Thickness(0, 0, 0, 20)
            };
            infoParagraph.Inlines.Add(new Run($"Mã hóa đơn: {invoice.InvoiceId}\n") { FontSize = 14 });
            infoParagraph.Inlines.Add(new Run($"Căn hộ: {invoice.ApartmentCode}\n") { FontSize = 14 });
            infoParagraph.Inlines.Add(new Run($"Tháng/Năm: {invoice.Month}/{invoice.Year}\n") { FontSize = 14 });
            infoParagraph.Inlines.Add(new Run($"Ngày tạo: {invoice.CreatedDate}\n") { FontSize = 14 });
            flowDocument.Blocks.Add(infoParagraph);

            var waterInvoice = WaterInvoices.FirstOrDefault(w => w.InvoiceId == invoice.InvoiceId);
            if (waterInvoice != null)
            {
                var waterSection = new Section()
                {
                    Margin = new Thickness(0, 0, 0, 20)
                };
                waterSection.Blocks.Add(new Paragraph(new Run("Chi tiết hóa đơn nước"))
                {
                    FontWeight = FontWeights.Bold,
                    FontSize = 16,
                    Margin = new Thickness(0, 0, 0, 10)
                });

                var table = new Table();
                table.BorderBrush = Brushes.Black;
                table.BorderThickness = new Thickness(1);
                table.CellSpacing = 0;

                for (int i = 0; i < 4; i++)
                {
                    var column = new TableColumn();
                    column.Width = new GridLength(1, GridUnitType.Star);
                    table.Columns.Add(column);
                }

                var headerRow = new TableRow();
                headerRow.Background = new SolidColorBrush(Colors.LightGray);
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Chỉ số đầu")) { TextAlignment = TextAlignment.Center }));
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Chỉ số cuối")) { TextAlignment = TextAlignment.Center }));
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Số người")) { TextAlignment = TextAlignment.Center }));
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Thành tiền")) { TextAlignment = TextAlignment.Center }));

                var dataRow = new TableRow();
                dataRow.Cells.Add(new TableCell(new Paragraph(new Run(waterInvoice.StartIndex.ToString())) { TextAlignment = TextAlignment.Center }));
                dataRow.Cells.Add(new TableCell(new Paragraph(new Run(waterInvoice.EndIndex.ToString())) { TextAlignment = TextAlignment.Center }));
                dataRow.Cells.Add(new TableCell(new Paragraph(new Run(waterInvoice.NumberOfPeople.ToString())) { TextAlignment = TextAlignment.Center }));
                dataRow.Cells.Add(new TableCell(new Paragraph(new Run(waterInvoice.TotalAmount.ToString("N0"))) { TextAlignment = TextAlignment.Right }));

                var rowGroup = new TableRowGroup();
                rowGroup.Rows.Add(headerRow);
                rowGroup.Rows.Add(dataRow);
                table.RowGroups.Add(rowGroup);

                waterSection.Blocks.Add(table);
                flowDocument.Blocks.Add(waterSection);
            }


            var managementInvoice = ManagementInvoices.FirstOrDefault(m => m.InvoiceId == invoice.InvoiceId);
            if (managementInvoice != null)
            {
                var managementSection = new Section()
                {
                    Margin = new Thickness(0, 0, 0, 20)
                };
                managementSection.Blocks.Add(new Paragraph(new Run("Chi tiết phí quản lý"))
                {
                    FontWeight = FontWeights.Bold,
                    FontSize = 16,
                    Margin = new Thickness(0, 0, 0, 10)
                });

                var table = new Table();
                table.BorderBrush = Brushes.Black;
                table.BorderThickness = new Thickness(1);
                table.CellSpacing = 0;

                for (int i = 0; i < 3; i++)
                {
                    var column = new TableColumn();
                    column.Width = new GridLength(1, GridUnitType.Star);
                    table.Columns.Add(column);
                }

                var headerRow = new TableRow();
                headerRow.Background = new SolidColorBrush(Colors.LightGray);
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Diện tích (m²)")) { TextAlignment = TextAlignment.Center }));
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Đơn giá")) { TextAlignment = TextAlignment.Center }));
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Thành tiền")) { TextAlignment = TextAlignment.Center }));

                var dataRow = new TableRow();
                dataRow.Cells.Add(new TableCell(new Paragraph(new Run(managementInvoice.Area.ToString())) { TextAlignment = TextAlignment.Center }));
                dataRow.Cells.Add(new TableCell(new Paragraph(new Run(managementInvoice.Fee.ToString())) { TextAlignment = TextAlignment.Right }));
                dataRow.Cells.Add(new TableCell(new Paragraph(new Run(managementInvoice.TotalAmount.ToString("N0"))) { TextAlignment = TextAlignment.Right }));

                var rowGroup = new TableRowGroup();
                rowGroup.Rows.Add(headerRow);
                rowGroup.Rows.Add(dataRow);
                table.RowGroups.Add(rowGroup);

                managementSection.Blocks.Add(table);
                flowDocument.Blocks.Add(managementSection);
            }

            var vehicleInvoice = VehicleInvoices.FirstOrDefault(v => v.InvoiceId == invoice.InvoiceId);
            if (vehicleInvoice != null)
            {
                var vehicleSection = new Section()
                {
                    Margin = new Thickness(0, 0, 0, 20)
                };
                vehicleSection.Blocks.Add(new Paragraph(new Run("Chi tiết phí gửi xe"))
                {
                    FontWeight = FontWeights.Bold,
                    FontSize = 16,
                    Margin = new Thickness(0, 0, 0, 10)
                });

                var vehicles = await _invoiceService.GetDetailVehicleInvoiceById(vehicleInvoice.VehicleInvoiceId);

                if (vehicles != null && vehicles.Any())
                {
                    var table = new Table();
                    table.BorderBrush = Brushes.Black;
                    table.BorderThickness = new Thickness(1);
                    table.CellSpacing = 0;

                    for (int i = 0; i < 4; i++)
                    {
                        var column = new TableColumn();
                        column.Width = new GridLength(1, GridUnitType.Star);
                        table.Columns.Add(column);
                    }

                    var headerRow = new TableRow();
                    headerRow.Background = new SolidColorBrush(Colors.LightGray);
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Biển số")) { TextAlignment = TextAlignment.Center }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Chủ sở hữu")) { TextAlignment = TextAlignment.Center }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Loại xe")) { TextAlignment = TextAlignment.Center }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Phí gửi xe")) { TextAlignment = TextAlignment.Center }));

                    var rowGroup = new TableRowGroup();
                    rowGroup.Rows.Add(headerRow);

                    foreach (var vehicle in vehicles)
                    {
                        var dataRow = new TableRow();
                        dataRow.Cells.Add(new TableCell(new Paragraph(new Run(vehicle.Id)) { TextAlignment = TextAlignment.Center }));
                        dataRow.Cells.Add(new TableCell(new Paragraph(new Run(vehicle.Owner)) { TextAlignment = TextAlignment.Left }));
                        dataRow.Cells.Add(new TableCell(new Paragraph(new Run(vehicle.Type)) { TextAlignment = TextAlignment.Center }));
                        dataRow.Cells.Add(new TableCell(new Paragraph(new Run(vehicle.Fee?.ToString("N0"))) { TextAlignment = TextAlignment.Right }));
                        rowGroup.Rows.Add(dataRow);
                    }

                    table.RowGroups.Add(rowGroup);
                    vehicleSection.Blocks.Add(table);
                }

                vehicleSection.Blocks.Add(new Paragraph(new Run($"Tổng tiền: {vehicleInvoice.TotalAmount:N0} VNĐ"))
                {
                    TextAlignment = TextAlignment.Right,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 10, 0, 0)
                });
                flowDocument.Blocks.Add(vehicleSection);
            }

            var totalSection = new Section();
            totalSection.Blocks.Add(new Paragraph(new Run($"Tổng cộng: {invoice.TotalAmount:N0} VNĐ"))
            {
                FontWeight = FontWeights.Bold,
                FontSize = 18,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            });
            flowDocument.Blocks.Add(totalSection);

            var previewWindow = new InvoicePreviewView(flowDocument);
            previewWindow.ShowDialog();
        }

    }
    
}
