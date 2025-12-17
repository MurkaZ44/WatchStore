using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace Kursach;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Инициализация базы данных
        // Microsoft.EntityFrameworkCore.Sqlite автоматически инициализирует SQLite
        using var context = new AppDbContext();
        context.Database.EnsureCreated();
    }
}