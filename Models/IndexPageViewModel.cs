using System.Collections;

public class IndexPageModel
{
    public IndexPageModel(DivData[]? thisYear, List<DivData> popularStocks)
    {
        ThisYearDividends = thisYear != null ? new YearStat(DateTime.Now.Year, thisYear.Length) : null;
        NumOfCompaniesPayingDivThisYear = thisYear != null ? thisYear.GroupBy(s => s.CompanyId).ToArray().Length : null;

        if (popularStocks.Count > 0)
        {
            PopularStocks = popularStocks.OrderBy(x => DateTime.Parse(x.PaymentDate)).ToList();
        }

        if (thisYear != null)
        {
            var todaysDividends = thisYear.Where(x => DateTime.Parse(x.PaymentDate) == new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)).ToArray();
            if (todaysDividends != null && todaysDividends.Length > 0)
            {
                NumOfDividendsToday = todaysDividends.Length;
                todaysDividends = todaysDividends.OrderBy(x => x.CompanyProfile.CompanyName).ToArray();
                if (todaysDividends.Length > 8)
                {
                    todaysDividends = todaysDividends.Skip(0).Take(8).ToArray();
                }
                TodaysDividends = todaysDividends;
            }
        }

    }

    public YearStat? ThisYearDividends { get; set; }
    public List<DivData> PopularStocks { get; set; } = new List<DivData>();
    public int? NumOfCompaniesPayingDivThisYear { get; set; }
    public int? NumOfDividendsToday { get; set; }
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