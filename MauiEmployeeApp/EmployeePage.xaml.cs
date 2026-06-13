using MauiEmployeeApp.Models;
using MauiEmployeeApp.Services;

namespace MauiEmployeeApp;

public partial class EmployeePage : ContentPage
{
    private readonly Employee? _employee;
    private readonly DatabaseService _database;
    public EmployeePage(DatabaseService database, Employee? employee = null)
    {
        InitializeComponent();
        _database = database;
        _employee = employee ?? new Employee();
        if (employee != null )
        {
            FullNameEntry.Text = employee.FullName;
            PositionEntry.Text = employee.Position;
            SalaryEntry.Text = employee.Salary.ToString();
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FullNameEntry.Text) ||
            string.IsNullOrWhiteSpace(PositionEntry.Text) ||
            string.IsNullOrWhiteSpace(SalaryEntry.Text) ||
            !decimal.TryParse(SalaryEntry.Text, out var salary))
        {
            await DisplayAlert("Validation Error", "Please fill in all fields with valid data.", "OK");
            return;
        }

        _employee!.FullName = FullNameEntry.Text;
        _employee.Position = PositionEntry.Text;
        _employee.Salary = salary;

        if (_employee.Id == 0)
        {
            await _database.AddEmployeeAsync(_employee);
        }
        else
        {
            await _database.UpdateEmployeeAsync(_employee);
        }

        await Navigation.PopAsync(); // 🔥 Torna alla lista
    }


    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

}