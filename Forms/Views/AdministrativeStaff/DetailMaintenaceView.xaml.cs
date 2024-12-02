using Services.Interfaces.AdministrativeStaffServices;
using System.Windows;

namespace Forms.Views.AdministrativeStaff
{
    public partial class DetailMaintenanceView : Window
    {
        private readonly IMaintananceService _maintananceService;
        private readonly int _maintenanceId;

        public DetailMaintenanceView(IMaintananceService maintananceService, int maintenanceId)
        {
            InitializeComponent();
            _maintananceService = maintananceService;
            _maintenanceId = maintenanceId;
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            var maintenance = await _maintananceService.GetMaintenanceById(_maintenanceId);
            MaintenanceIdInput.Text = maintenance.MaintenanceId.ToString();
            MaintenanceNameInput.Text = maintenance.MaintenanceName;
            MaintanaceDateInput.Text = maintenance.MaintanaceDate.ToString("dd-MM-yyyy");
            CreatedByInput.Text = maintenance.CreatedByNavigation.FullName;
            DepartmentInput.Text = maintenance.Department.DepartmentName;
            DescriptionInput.Text = maintenance.Description;
            MaintanaceStatusInput.Text = maintenance.Status;
            EquipmentDataGrid.ItemsSource = maintenance.Equipment;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
