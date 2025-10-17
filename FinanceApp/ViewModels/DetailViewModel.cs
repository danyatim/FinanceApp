using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Models;
using FinanceApp.Services;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Globalization;

namespace FinanceApp.ViewModels;

public abstract partial class DetailViewModel : BaseViewModel
{
    protected readonly ITransactionService _tx;
    protected readonly IDateRangeService _ranges;
    protected readonly IReferenceService _refs;

    [ObservableProperty] private DateRange period;
    [ObservableProperty] private TimeGrouping grouping = TimeGrouping.Daily;

    [ObservableProperty] private DateTime fromDate;
    [ObservableProperty] private DateTime toDate;

    [ObservableProperty] private string? selectedAccount;
    [ObservableProperty] private string? selectedSource;
    [ObservableProperty] private Transaction? selectedTransaction;

    [ObservableProperty] private List<string> accountNames = new();
    [ObservableProperty] private List<string> sourceNames = new();

    [ObservableProperty] private List<Transaction> items = new();
    //[ObservableProperty] private Dictionary<string, decimal> sourcesTotals = new();
    [ObservableProperty] private List<TopItem> sourcesTotals = new();

    [ObservableProperty] private ISeries[] series = Array.Empty<ISeries>();

    [ObservableProperty]
    private Axis[] xAxes =
    [
        new Axis
        {
            Labeler = v => new DateTime((long)v).ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("ru-RU")),
            UnitWidth = TimeSpan.FromDays(1).Ticks,
            MinStep = TimeSpan.FromDays(1).Ticks
        }
    ];

    [ObservableProperty]
    private Axis[] yAxes =
    [
        new Axis { Labeler = v => v.ToString("C0", CultureInfo.GetCultureInfo("ru-RU")) }
    ];

    protected abstract TransactionDirection? DirectionForList { get; }

    protected DetailViewModel(ITransactionService tx, IDateRangeService ranges, IReferenceService refs)
    {
        _tx = tx; _ranges = ranges; _refs = refs;
        var rng = _ranges.CurrentMonth();
        Period = rng;
        FromDate = rng.From;
        ToDate = rng.To;
        UpdateAxes();
    }

    partial void OnFromDateChanged(DateTime value) => Period = new DateRange(value, ToDate);
    partial void OnToDateChanged(DateTime value) => Period = new DateRange(FromDate, value);

    partial void OnGroupingChanged(TimeGrouping value)
    {
        UpdateAxes();
        _ = LoadAsync();
    }

    private async Task LoadLookupsAsync()
    {
        var acc = await _refs.GetAccountsAsync();
        AccountNames = acc.Select(a => a.Name).ToList();

        var src = await _refs.GetSourcesAsync(DirectionForList);
        SourceNames = src.Select(s => s.Name).ToList();
    }

    [RelayCommand]
    public void UpdateAxes()
    {
        var ru = CultureInfo.GetCultureInfo("ru-RU");

        long stepTicks = Grouping switch
        {
            TimeGrouping.Daily => TimeSpan.FromDays(1).Ticks,
            TimeGrouping.Weekly => TimeSpan.FromDays(7).Ticks,
            TimeGrouping.Monthly => TimeSpan.FromDays(30).Ticks,
            TimeGrouping.Yearly => TimeSpan.FromDays(365).Ticks,
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
    public async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            await LoadLookupsAsync();

            if (DirectionForList.HasValue)
            {
                Items = await _tx.GetAsync(Period, DirectionForList, SelectedAccount, SelectedSource);
                SourcesTotals = await _tx.SumBySourceAsync(Period, DirectionForList.Value, SelectedAccount);
                var seriesData = await _tx.SeriesAsync(Period, DirectionForList, Grouping);
                Series = new ISeries[]
                {
                    new LineSeries<DateTimePoint>
                    {
                        Values = seriesData.Select(p => new DateTimePoint(p.Bucket, (double)p.Sum)).ToArray(),
                        Name = DirectionForList == TransactionDirection.Income ? "Выручка" : "Расходы",
                        AnimationsSpeed = TimeSpan.Zero
                    }
                };
            }
            else
            {
                var inc = await _tx.SeriesAsync(Period, TransactionDirection.Income, Grouping);
                var exp = await _tx.SeriesAsync(Period, TransactionDirection.Expense, Grouping);
                Series = new ISeries[]
                {
                    new LineSeries<DateTimePoint>{ Name="Выручка", Values = inc.Select(p => new DateTimePoint(p.Bucket, (double)p.Sum)).ToArray() },
                    new LineSeries<DateTimePoint>{ Name="Расходы", Values = exp.Select(p => new DateTimePoint(p.Bucket, (double)p.Sum)).ToArray() }
                };

                var incItems = await _tx.SumBySourceAsync(Period, TransactionDirection.Income, SelectedAccount);
                var expItems = await _tx.SumBySourceAsync(Period, TransactionDirection.Expense, SelectedAccount);
                // Формируем объединенную коллекцию источников
                var allSources = incItems.Select(item => item.Name)
                    .Union(expItems.Select(item => item.Name))
                    .Distinct();

                // Создаем список TopItem с разницей между доходами и расходами по каждому источнику
                SourcesTotals = allSources
                    .Select(source => new TopItem
                    {
                        Name = source,
                        Summ = (incItems.FirstOrDefault(i => i.Name == source)?.Summ ?? 0) -
                               (expItems.FirstOrDefault(e => e.Name == source)?.Summ ?? 0)
                    })
                    .OrderByDescending(item => item.Summ)
                    .ToList();

                // Используйте sourcesTotals далее по необходимости
                // Например, присвоить свойству Items или другому свойству

                var accountDisplay = string.IsNullOrWhiteSpace(SelectedAccount) ? "Все счета" : SelectedAccount;
            }
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    public async Task DeleteTransactionAsync()
    {
        if (SelectedTransaction is null || SelectedTransaction.Id <= 0) return;

        IsBusy = true;
        try
        {
            await _tx.DeleteAsync(SelectedTransaction);
            await LoadAsync();
        }
        finally { IsBusy = false; }
    }
}
