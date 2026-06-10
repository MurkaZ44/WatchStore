using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kursach.Model.Repositories;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Windows;
using Kursach.Model.Interfaces;
using Kursach.Services;
using OxyPlot.Legends;
using Kursach.Model.Models;

namespace Kursach.ViewModel;

// Вспомогательный класс для хранения данных о продажах продукта

public partial class ReportsViewModel : ObservableObject
{
    private readonly SaleRepository _saleRepository;
    private readonly AppDbContext _context;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private PlotModel _plotModel;

    [ObservableProperty]
    private PlotModel _salesDynamicsPlotModel;

    [ObservableProperty]
    private bool _hasData;

    [ObservableProperty]
    private decimal _totalRevenue;

    [ObservableProperty]
    private int _totalSales;

    [ObservableProperty]
    private decimal _averageCheck;

    [ObservableProperty]
    private DateTime _startDate = DateTime.Now.AddMonths(-1);

    [ObservableProperty]
    private DateTime _endDate = DateTime.Now;

    public IRelayCommand GenerateReportCommand { get; }

    public ReportsViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
        _context = new AppDbContext();
        _saleRepository = new SaleRepository(_context);
        
        PlotModel = new PlotModel { Title = "Загрузка данных..." };
        SalesDynamicsPlotModel = new PlotModel { Title = "Загрузка данных..." };
        
        GenerateReportCommand = new AsyncRelayCommand(LoadSalesAsync);
        
        LoadInitialDataAsync();
    }

    public ReportsViewModel() : this(new DialogService())
    {
    }

    private async void LoadInitialDataAsync()
    {
        try
        {
            await LoadSalesAsync();
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", true);
        }
    }

    private async Task LoadSalesAsync()
    {
        try
        {
            var allSales = await _saleRepository.GetAllAsync();
            
            var sales = allSales
                .Where(s => s.Date.Date >= StartDate.Date && s.Date.Date <= EndDate.Date)
                .ToList();

            _totalSales = sales.Count;
            
            var groupedByProduct = sales
                .GroupBy(s => s.Product?.Model ?? "Unknown Model")
                .Select(g => new ProductSaleData
                {
                    ProductModel = g.Key,
                    TotalQuantity = g.Sum(s => s.Quantity),
                    TotalRevenue = g.Sum(s => s.Price * s.Quantity)
                })
                .OrderByDescending(g => g.TotalQuantity)
                .Take(10)
                .ToList();

            
            HasData = groupedByProduct.Any();
            if (HasData)
            {
                TotalRevenue = groupedByProduct.Sum(g => g.TotalRevenue);
                AverageCheck = _totalSales > 0 ? Math.Round(TotalRevenue / _totalSales, 2) : 0m;
            }
            else
            {
                TotalRevenue = 0m;
                TotalSales = 0;
                AverageCheck = 0m;
            }

            var newPlotModel = CreateProductSalesPlot(groupedByProduct);
            PlotModel = newPlotModel;

            var newSalesDynamicsPlotModel = CreateSalesDynamicsPlot(sales);
            SalesDynamicsPlotModel = newSalesDynamicsPlotModel;
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage($"Ошибка при загрузке отчёта: {ex.Message}", "Ошибка", true);
        }
    }

    //Метод принимает List<ProductSaleData>
    private PlotModel CreateProductSalesPlot(List<ProductSaleData> groupedByProduct)
    {
        var plotModel = new PlotModel { Title = "Продажи часов по типу" };

        if (!groupedByProduct.Any())
        {
            plotModel.Title = "Недостаточно данных для построения графика";
            return plotModel;
        }

        var categories = groupedByProduct.Select(g => g.ProductModel).ToList();
        
        plotModel.Axes.Add(new CategoryAxis
        {
            Position = AxisPosition.Left,
            ItemsSource = categories,
            Title = "Тип часов",
            MajorGridlineStyle = LineStyle.None,
            MinorGridlineStyle = LineStyle.None,
            GapWidth = 0.1
        });

        plotModel.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Bottom,
            Title = "Количество проданных",
            Minimum = 0,
            MinimumPadding = 0,
            MaximumPadding = 0.06,
            AbsoluteMinimum = 0,
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot,
            MajorGridlineColor = OxyColor.FromRgb(230, 230, 230),
            MinorGridlineColor = OxyColor.FromRgb(240, 240, 240)
        });

        var barSeries = new BarSeries
        {
            Title = "Количество продаж",
            FillColor = OxyColor.FromRgb(65, 105, 225),
            StrokeColor = OxyColor.FromRgb(65, 105, 225),
            StrokeThickness = 1,
            LabelFormatString = "{0}", // Показывает значение на столбике
            LabelPlacement = LabelPlacement.Inside,
            TextColor = OxyColors.White
        };

        foreach (var item in groupedByProduct)
        {
            barSeries.Items.Add(new BarItem((double)item.TotalQuantity));
        }

        plotModel.Series.Add(barSeries);
        
        return plotModel;
    }

    private PlotModel CreateSalesDynamicsPlot(List<Kursach.Model.Models.Sale> sales)
    {
        var plotModel = new PlotModel { Title = "Динамика продаж" };

        if (!sales.Any())
        {
            plotModel.Title = "Недостаточно данных для построения графика динамики";
            return plotModel;
        }

        var salesByMonth = sales
            .GroupBy(s => new { s.Date.Year, s.Date.Month })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month)
            .Select(g => new
            {
                Period = new DateTime(g.Key.Year, g.Key.Month, 1),
                TotalRevenue = g.Sum(s => s.Price * s.Quantity),
                TotalQuantity = g.Sum(s => s.Quantity)
            })
            .ToList();

        if (!salesByMonth.Any())
        {
            plotModel.Title = "Недостаточно данных для построения графика динамики";
            return plotModel;
        }

        var revenueSeries = new LineSeries
        {
            Title = "Выручка",
            Color = OxyColor.FromRgb(0, 128, 0),
            MarkerType = MarkerType.Circle,
            MarkerSize = 4,
            MarkerStroke = OxyColor.FromRgb(0, 128, 0),
            MarkerFill = OxyColor.FromRgb(0, 128, 0),
            LabelFormatString = "{1:N2} ₽", // Показывает значение на точке
            LabelMargin = 10
        };

        var quantitySeries = new LineSeries
        {
            Title = "Количество продаж",
            Color = OxyColor.FromRgb(255, 140, 0),
            MarkerType = MarkerType.Triangle,
            MarkerSize = 4,
            MarkerStroke = OxyColor.FromRgb(255, 140, 0),
            MarkerFill = OxyColor.FromRgb(255, 140, 0),
            LabelFormatString = "{1}", // Показывает значение на точке
            LabelMargin = 10
        };

        foreach (var item in salesByMonth)
        {
            var dateValue = DateTimeAxis.ToDouble(item.Period);
            revenueSeries.Points.Add(new DataPoint(dateValue, (double)item.TotalRevenue));
            quantitySeries.Points.Add(new DataPoint(dateValue, (double)item.TotalQuantity));
        }

        plotModel.Series.Add(revenueSeries);
        plotModel.Series.Add(quantitySeries);

        // Используем DateTimeAxis для оси X
        plotModel.Axes.Add(new DateTimeAxis
        {
            Position = AxisPosition.Bottom,
            Title = "Период",
            StringFormat = "MMM yyyy",
            IntervalType = DateTimeIntervalType.Months,
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot,
            Angle = -45 // Вернем небольшой угол для лучшей читаемости, если дат много
        });

        plotModel.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Left,
            Title = "Значение",
            Minimum = 0,
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot,
            MajorGridlineColor = OxyColor.FromRgb(230, 230, 230)
        });

        plotModel.Legends.Add(new Legend()
        {
            LegendPosition = LegendPosition.TopRight,
            LegendOrientation = LegendOrientation.Horizontal,
            LegendPlacement = LegendPlacement.Outside,
            LegendBackground = OxyColor.FromArgb(200, 255, 255, 255)
        });

        return plotModel;
    }
}