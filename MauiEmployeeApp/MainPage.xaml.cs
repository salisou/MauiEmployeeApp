using MauiEmployeeApp.Models;
using MauiEmployeeApp.Services;
using System.Collections.ObjectModel;

namespace MauiEmployeeApp
{
    public partial class MainPage : ContentPage
    {
        private readonly DatabaseService _database;
        private readonly ObservableCollection<Employee> _employees = new();

        public MainPage(DatabaseService database)
        {
            InitializeComponent();
            _database = database;
            EmployeeList.ItemsSource = _employees;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadData(SearchBar.Text);
        }

        private async Task LoadData(string? filter = null)
        {
            var employees = await _database.GetEmployeesAsync();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = filter.ToLowerInvariant();
                employees = employees
                    .Where(e =>
                        e.FullName.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                        e.Position.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                _employees.Clear();
                foreach (var emp in employees)
                    _employees.Add(emp);
            });
        }

        private async void OnDelete_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is Employee emp)
            {
                bool confirm = await DisplayAlertAsync("Confirm Delete",
                    $"Are you sure you want to delete {emp.FullName}?",
                    "Yes", "No");

                if (!confirm)
                    return;

                await _database.DeleteEmployeeAsync(emp.Id);
                await LoadData(SearchBar.Text);
            }
        }

        private void OnItemDoubleTapped(object sender, TappedEventArgs e)
        {
            if (sender is VisualElement element && element.BindingContext is Employee emp)
            {
                Navigation.PushAsync(new EmployeePage(_database, emp));
            }
        }

        private async void OnAddEmployee_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EmployeePage(_database));
        }

        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            await LoadData(e.NewTextValue);
        }
    }
}
