public class CompanyPage
{
    public CompanyPage(CompanyProfile c, List<CompanyProfile>? relatedCompanies)
    {
        Name = c.CompanyName;
        Symbol = c.CompanySymbol;
        Exchange = c.StockExchange;
        RelatedStocks = GetRelatedStocks(relatedCompanies, c);
        CompanyLogo = c.CompanyLogo ?? null;

        if (c.Dividends.Count > 0)
        {
            OrganizeDividendData(c.Dividends.ToArray());
            HasDividends = true;
        }
        if (c.CompanyDescription != null)
        {
            Description = c.CompanyDescription;
        }
    }

    public string Name { get; set; }
    public string Symbol { get; set; }
    public string Exchange { get; set; }
    public string[]? Description { get; set; }
    public DivData[]? UpcomingDividends { get; set; }
    public DivData[]? PastDividends { get; set; }
    public DivData? TodaysDividend { get; set; }
    public bool HasDividends { get; set; } = false;
    public CompanyProfile[]? RelatedStocks { get; set; }
    public string? CompanyLogo { get; set; }

    private void OrganizeDividendData(DivData[] data)
    {
        /* 
            This function will loop through all dividend data related to company
            and store in either:

            1. UpcomingDividends
            2. PastDividends
            3. TodaysDividend
        */

        List<DivData> upcomingPayments = new List<DivData>();
        List<DivData> recentPayments = new List<DivData>();

        var currentDate = DateTime.Now;
        foreach (var x in data)
        {
            try
            {
                // upcoming
                if (DateTime.Parse(x.PaymentDate) > currentDate)
                {
                    upcomingPayments.Add(x);
                }
                // recent
                else if (DateTime.Parse(x.PaymentDate) < currentDate)
                {
                    recentPayments.Add(x);
                }
                // today
                else
                {
                    TodaysDividend = x;
                }
            }
            catch (Exception err)
            {
                continue;
            }
        }

        UpcomingDividends = upcomingPayments.Count > 0 ? upcomingPayments.ToArray().OrderBy(x => DateTime.Parse(x.PaymentDate)).Reverse().ToArray() : null;
        PastDividends = recentPayments.Count > 0 ? recentPayments.ToArray().OrderBy(x => DateTime.Parse(x.PaymentDate)).ToArray() : null;
    }

    public string CleanDate(string? d)
    {
        /*
            Will return a date string as yyyy-MM-dd format to human readable
            ex: 1998-05-25 => May 23, 1998
        */
        if (d == null) return "null";
        DateTime x = DateTime.ParseExact(d, "yyyy-MM-dd", null);
        return x.ToString("MMM d, yyyy");
    }

    private CompanyProfile[]? GetRelatedStocks(List<CompanyProfile>? relatedStocks, CompanyProfile currentCompany)
    {
        if (relatedStocks == null) return null;

        try
        {
            List<CompanyProfile> r = new List<CompanyProfile>();

            // find the index of the current copmany in the related stocks array 
            // and then return the 5 stocks after that

            int startingIndex = relatedStocks.FindIndex(0, relatedStocks.Count - 1, c => c.CompanySymbol.ToUpper() == currentCompany.CompanySymbol.ToUpper());
            int companyAt = startingIndex + 1 < relatedStocks.Count ? startingIndex + 1 : 0;

            for (int i = 0; i < 5; i++)
            {
                if (companyAt + 1 <= relatedStocks.Count)
                {
                    r.Add(relatedStocks[companyAt]);
                    companyAt++;
                }
                else
                {
                    r.Add(relatedStocks[0]);
                    companyAt = 0;
                }
            }

            return r.ToArray();
        }
        catch (Exception err)
        {
            return null;
        }
    }

    static public HeadTagMetadata GenerateCompanyProfilePageHeaderTags(string companyName, string companySymbol, bool hasDividend, int numOfDividends)
    {
        string description = hasDividend ? $"{companyName} ({companySymbol.ToUpper()}) has paid out {numOfDividends} dividends this year. Find dates and amounts payed out to shareholders along with related stocks." : $"{companyName} ({companySymbol.ToUpper()}) does not pay dividends. Check out reltated stocks that pay dividends.";

        return new HeadTagMetadata(companyName, companySymbol, description);
    }
}

public class DividendData
{
    public decimal dividend { get; set; }
    public string recordDate { get; set; }
    public string paymentDate { get; set; }
    public string declarationDate { get; set; }
}

public class HeadTagMetadata
{
    public HeadTagMetadata(string companyName, string companySymbol, string metaDescription)
    {
        Title = $"{companyName} ({companySymbol}) - Dividend Calendar";
        CanonicalTag = $"https://dividome.com/{companySymbol.ToLower()}";
        Description = metaDescription;

        OgMetaTags = new OpenGraphProperties
        {
            Title = $"Discover {companyName}'s dividend payouts for the current year, including details such as payment dates and dividend amounts",
            Image = $"https://491292639630-dividome.s3.amazonaws.com/imgs/{companySymbol}.png",
            Url = $"https://dividome.com/{companySymbol.ToLower()}"
        };

        TwitterOgMetaTags = new TwitterOpenGraphProperties
        {
            Title = $"{companyName}'s ({companySymbol}) dividend payouts",
            Image = $"https://491292639630-dividome.s3.amazonaws.com/imgs/{companySymbol}.png",
            Description = $"{companyName}'s dividend payouts for the current calendar year, including past and upcoming dividend payments"
        };
    }


    public string Title { get; set; }
    public string Description { get; set; }
    public string CanonicalTag { get; set; }
    public OpenGraphProperties OgMetaTags { get; set; }
    public TwitterOpenGraphProperties TwitterOgMetaTags { get; set; }
}

public class OpenGraphProperties
{
    public string Title { get; set; }
    public string Image { get; set; }
    public string Url { get; set; }
}

public class TwitterOpenGraphProperties
{
    public string Title { get; set; }
    public string Image { get; set; }
    public string Description { get; set; }
}
