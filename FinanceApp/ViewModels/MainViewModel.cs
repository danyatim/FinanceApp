using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Models;
using FinanceApp.Services;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting; // SolidColorPaint
using SkiaSharp;
using System.Globalization; // <— добавили

namespace FinanceApp.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly ITransactionService _tx;
    private readonly IDateRangeService _ranges;
    private readonly IReferenceService _refs;

    [ObservableProperty] private DateRange period;
    [ObservableProperty] private TimeGrouping grouping = TimeGrouping.Daily;

    [ObservableProperty] private DateTime fromDate;
    [ObservableProperty] private DateTime toDate;

    [ObservableProperty] private decimal incomeTotal;
    [ObservableProperty] private decimal expenseTotal;
    [ObservableProperty] private decimal profitTotal;

    [ObservableProperty] private ISeries[] series = Array.Empty<ISeries>();
    [ObservableProperty] private Axis[] xAxes = Array.Empty<Axis>();
    [ObservableProperty] private Axis[] yAxes = Array.Empty<Axis>();

    public MainViewModel(ITransactionService tx, IDateRangeService ranges, IReferenceService refs)
    {
        _tx = tx; _ranges = ranges; _refs = refs;

        var rng = _ranges.CurrentMonth();
        Period = rng;
        FromDate = rng.From;
        ToDate = rng.To;

        SetupAxes(); // инициализируем оси
    }

    partial void OnFromDateChanged(DateTime value) => Period = new DateRange(value, ToDate);
    partial void OnToDateChanged(DateTime value) => Period = new DateRange(FromDate, value);

    partial void OnGroupingChanged(TimeGrouping value)
    {
        UpdateAxes();     // перенастроить ось X под новую группировку
        _ = LoadAsync();  // перезагрузить серии
    }

    public void UpdateAxes()
    {
        var ru = CultureInfo.GetCultureInfo("ru-RU");

        long stepTicks = Grouping switch
        {
            TimeGrouping.Daily => TimeSpan.FromDays(1).Ticks,
            TimeGrouping.Weekly => TimeSpan.FromDays(7).Ticks,
            TimeGrouping.Monthly => TimeSpan.FromDays(30).Ticks,   // упрощённо
            TimeGrouping.Yearly => TimeSpan.FromDays(365).Ticks,  // упрощённо
            _ => TimeSpan.FromDays(1).Ticks
        };

        string format = Grouping switch
        {
            TimeGrouping.Monthly => "MMM yy",
            TimeGrouping.Yearly => "yyyy",
            _ => "dd.MM"
        };

        var labelPaint = new SolidColorPaint(SKColor.Parse("#000000")); // черный
        var sepPaint = new SolidColorPaint(SKColor.Parse("#D1D5DB")) { StrokeThickness = 1 }; // линии сетки

        string SafeLabel(double v)
        {
            if (double.IsNaN(v) || double.IsInfinity(v)) return string.Empty;
            // округление и ограничение диапазона ticks
            double min = DateTime.MinValue.Ticks;
            double max = DateTime.MaxValue.Ticks;
            var clamped = Math.Max(min, Math.Min(max, v));
            var ticks = (long)Math.Round(clamped);
            var dt = new DateTime(ticks);
            return dt.ToString(format, ru);
        }

        XAxes =
        [
            new Axis
        {
            Labeler  = SafeLabel,
            UnitWidth = stepTicks,
            MinStep   = stepTicks,
            LabelsPaint = labelPaint,
            SeparatorsPaint = sepPaint
        }
        ];

        YAxes =
        [
            new Axis
        {
            Labeler  = val => val.ToString("C0", ru),
            LabelsPaint = labelPaint,
            SeparatorsPaint = sepPaint
        }
        ];
    }

    [RelayCommand]
    private void SetupAxes() => UpdateAxes();

    [RelayCommand]
    public async Task LoadAsync()
    {
        try
        {
            IsBusy = true;
            IncomeTotal = await _tx.SumAsync(Period, TransactionDirection.Income);
            ExpenseTotal = await _tx.SumAsync(Period, TransactionDirection.Expense);
            ProfitTotal = IncomeTotal - ExpenseTotal;

            var inc = await _tx.SeriesAsync(Period, TransactionDirection.Income, Grouping);
            var exp = await _tx.SeriesAsync(Period, TransactionDirection.Expense, Grouping);

            Series =
            [
                new LineSeries<DateTimePoint>
                {
                    Name = "Выручка",
                    Values = inc.Select(p => new DateTimePoint(p.Bucket, (double)p.Sum)).ToArray(),
                    GeometrySize = 4
                },
                new LineSeries<DateTimePoint>
                {
                    Name = "Расходы",
                    Values = exp.Select(p => new DateTimePoint(p.Bucket, (double)p.Sum)).ToArray(),
                    GeometrySize = 4
                }
            ];

            var allBuckets = inc.Select(p => p.Bucket).Concat(exp.Select(p => p.Bucket)).ToList();
            if (allBuckets.Count > 0)
            {
                long stepTicks = Grouping switch
                {
                    TimeGrouping.Daily => TimeSpan.FromDays(1).Ticks,
                    TimeGrouping.Weekly => TimeSpan.FromDays(7).Ticks,
                    TimeGrouping.Monthly => TimeSpan.FromDays(30).Ticks,
                    TimeGrouping.Yearly => TimeSpan.FromDays(365).Ticks,
                    _ => TimeSpan.FromDays(1).Ticks
                };

                var minTicks = allBuckets.Min().Ticks;
                var maxTicks = allBuckets.Max().Ticks;

                var axis = (XAxes != null && XAxes.Length > 0) ? XAxes[0] : new Axis();
                axis.MinLimit = minTicks - stepTicks;   // небольшой отступ
                axis.MaxLimit = maxTicks + stepTicks;

                // переустанавливаем массив, чтобы MAUI/LiveCharts уловили обновление ссылки
                XAxes = new[] { axis };
            }
        }
        finally { IsBusy = false; }
    }

    [RelayCommand] private Task NavigateRevenueAsync() => Shell.Current.GoToAsync(nameof(Views.RevenuePage));
    [RelayCommand] private Task NavigateExpenseAsync() => Shell.Current.GoToAsync(nameof(Views.ExpensePage));
    [RelayCommand] private Task NavigateProfitAsync() => Shell.Current.GoToAsync(nameof(Views.ProfitPage));

    [RelayCommand]
    private async Task AddTransactionAsync()
    {
        var mainPage = Application.Current?.Windows[0].Page;
        if (mainPage != null)
        {
            var popup = new Popups.AddTransactionPopup(_refs);
            var result = await mainPage.ShowPopupAsync(popup);
            if (result is Models.Transaction t)
            {
                await _tx.AddAsync(t);
                await LoadAsync();
            }
        }

    }
}
