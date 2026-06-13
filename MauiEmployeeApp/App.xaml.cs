using MauiEmployeeApp.Services;
using Microsoft.Extensions.DependencyInjection;
using SQLitePCL;

namespace MauiEmployeeApp
{
    public partial class App : Application
    {
        public App(DatabaseService db)
        {
            InitializeComponent();
            Batteries_V2.Init();
            _ = db.InitializeDatabaseAync();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}