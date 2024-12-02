using Repositories.Repositories.Entities;
using Services.DTOs.AccountDTO;
using Services.DTOs.LoginDTO;
using Services.Interfaces.AdministrativeStaffServices;
using Services.Services.AdministrativeStaffServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Forms.Views.AdministrativeStaff
{
    /// <summary>
    /// Interaction logic for AddMaintenanceView.xaml
    /// </summary>
    public partial class AddMaintenanceView : Window
    {
        private readonly IMaintananceService _maintananceService;

        private List<ResponseEquipment> _equipmentList = [];
        private List<ResponseEquipment> _selectedEquipmentList = [];

        public LoginResponseDTO? User { get; set; }

        public AddMaintenanceView(IMaintananceService service)
        {
            InitializeComponent();
            _maintananceService = service;
            _ = InitalizeAsync();
        }

        public async Task InitalizeAsync()
        {
            MaintenanceIdInput.Text = (await _maintananceService.GetNextMaintenanceId()).ToString();
            _ = LoadEquipmentData();
        }

        private async Task LoadEquipmentData()
        {
            _equipmentList = await _maintananceService.GetAllEquipment();
            EquipmentDataGrid.ItemsSource = _equipmentList;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddEquipment_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentDataGrid.SelectedItem is ResponseEquipment selectedEquipment)
            {
                _equipmentList.Remove(selectedEquipment);
                EquipmentDataGrid.ItemsSource = null;
                EquipmentDataGrid.ItemsSource = _equipmentList;

                _selectedEquipmentList.Add(selectedEquipment);
                SelectedEquipmentDataGrid.ItemsSource = null;
                SelectedEquipmentDataGrid.ItemsSource = _selectedEquipmentList;
            }
        }

        private void RemoveEquipment_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEquipmentDataGrid.SelectedItem is ResponseEquipment selectedEquipment)
            {
                _selectedEquipmentList.Remove(selectedEquipment);
                SelectedEquipmentDataGrid.ItemsSource = null; // Cập nhật DataGrid
                SelectedEquipmentDataGrid.ItemsSource = _selectedEquipmentList;

                _equipmentList.Add(selectedEquipment);
                EquipmentDataGrid.ItemsSource = null; // Cập nhật DataGrid
                EquipmentDataGrid.ItemsSource = _equipmentList;
            }
        }

        private void SearchEquipmentInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string searchText = SearchEquipmentInput.Text.Trim();

            if (int.TryParse(searchText, out int searchId))
            {
                var filteredList = _equipmentList
                    .Where(e => e.EquipmentId == searchId)
                    .ToList();

                EquipmentDataGrid.ItemsSource = null;
                EquipmentDataGrid.ItemsSource = filteredList;
            }
            else
            {
                var filteredList = _equipmentList
                    .Where(e => e.EquipmentName.ToLower().Contains(searchText.ToLower()))
                    .ToList();

                EquipmentDataGrid.ItemsSource = null;
                EquipmentDataGrid.ItemsSource = filteredList;
            }
        }

        private void AddMaintenance_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
