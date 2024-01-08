public class TodaysDividendsModel
{
    public TodaysDividendsModel(DivData[]? todaysDividends, int dividendsTomorrow)
    {
        if (todaysDividends != null)
        {
            NumOfDividendsToday = todaysDividends.Length;
            TodaysDividends = todaysDividends.OrderBy(x => x.CompanyProfile.CompanyName).ToArray();

            decimal h = (decimal)todaysDividends[0].Dividend;
            DivData company = todaysDividends[0];
            // get the company paying the higest dividend of the day to display
            foreach (var x in todaysDividends)
            {
                if (x.Dividend > h)
                {
                    h = (decimal)x.Dividend;
                    company = x;
                }
            }
            HighestPayingDividend = company;

            NumOfDividendsTomorrow = dividendsTomorrow;
        }
    }


    public int NumOfDividendsToday { get; set; } = 0;
    public DivData[]? TodaysDividends { get; set; }
    public DivData? HighestPayingDividend { get; set; }
    public int NumOfDividendsTomorrow { get; set; }
}