public class IndexPageModel
{
    public IndexPageModel(DivData[]? lastYear, DivData[]? thisYear, List<DivData> popularStocks)
    {
        LastYearDividends = lastYear != null ? new YearStat(DateTime.Now.Year - 1, lastYear.Length - lastYear.Length % 10) : null;
        ThisYearDividends = thisYear != null ? new YearStat(DateTime.Now.Year, thisYear.Length) : null;
        NumOfCompaniesPayingDivThisYear = thisYear != null ? thisYear.GroupBy(s => s.CompanyId).ToArray().Length : null;

        if (popularStocks.Count > 0)
        {
            PopularStocks = popularStocks.OrderBy(x => DateTime.Parse(x.PaymentDate)).ToList();
        }

        if (thisYear != null)
        {
            var todaysDividends = thisYear.Where(x => DateTime.Parse(x.PaymentDate) == new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)).ToArray();
            if (todaysDividends.Length > 1)
            {
                TodaysDividends = todaysDividends;
            }
        }

    }

    public YearStat? LastYearDividends { get; set; }
    public YearStat? ThisYearDividends { get; set; }
    public List<DivData> PopularStocks { get; set; } = new List<DivData>();
    public int? NumOfCompaniesPayingDivThisYear { get; set; }
    public DivData[]? TodaysDividends { get; set; }
}

public class YearStat
{
    public YearStat(int y, int n)
    {
        Year = y;
        NumOfDividends = n;
    }

    public int Year { get; set; }
    public int NumOfDividends { get; set; }
}