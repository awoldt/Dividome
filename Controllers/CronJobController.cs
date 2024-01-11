
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/* 
    The point of this controller is to update all dividend data stored
    in database 

    An external cron job will hit this endpoint every 24hrs (midnight UTC)
*/

public class CronJobController : Controller
{
    private readonly IConfiguration _config;

    private readonly Db _db;

    public CronJobController(IConfiguration config, Db db)
    {
        _config = config;
        _db = db;
    }

    [HttpPost]
    [Route("/api/generate-dividend-data")]
    public async Task<string> GenerateDividendData()
    {
        try
        {
            var headers = HttpContext.Request.Headers;
            if (headers == null || !headers.ContainsKey("secret_header") || headers["secret_header"] != _config["secret_header"])
            {
                HttpContext.Response.StatusCode = 400;
                return "Request denied.";
            }

            DivData[]? q1DividendsLastYear = null;
            DivData[]? q2DividendsLastYear = null;
            DivData[]? q3DividendsLastYear = null;
            DivData[]? q4DividendsLastYear = null;

            DivData[]? q1DividendsThisYear = null;
            DivData[]? q2DividendsThisYear = null;
            DivData[]? q3DividendsThisYear = null;
            DivData[]? q4DividendsThisYear = null;

            DivData[]? ALL_DIVDEND_DATA = null;

            using (HttpClient client = new HttpClient())
            {
                /*  
                    There needs to be multiple requests made to API so that all data can be returned
                    api wont allow a single massive range of dates and return all of them

                    An effecient way of doing this is to fetch dividend data from all 4 quarters of the year
                */

                // last year's dividend data
                HttpResponseMessage q1LastYear = await client.GetAsync($"{_config["stock_api_url"]}/api/v3/stock_dividend_calendar?from={DateTime.Now.Year - 1}-01-01&to={DateTime.Now.Year - 1}-03-31&apikey={_config["stock_api_key"]}");
                HttpResponseMessage q2LastYear = await client.GetAsync($"{_config["stock_api_url"]}/api/v3/stock_dividend_calendar?from={DateTime.Now.Year - 1}-04-01&to={DateTime.Now.Year - 1}-06-30&apikey={_config["stock_api_key"]}");
                HttpResponseMessage q3LastYear = await client.GetAsync($"{_config["stock_api_url"]}/api/v3/stock_dividend_calendar?from={DateTime.Now.Year - 1}-07-01&to={DateTime.Now.Year - 1}-09-30&apikey={_config["stock_api_key"]}");
                HttpResponseMessage q4LastYear = await client.GetAsync($"{_config["stock_api_url"]}/api/v3/stock_dividend_calendar?from={DateTime.Now.Year - 1}-10-01&to={DateTime.Now.Year - 1}-12-31&apikey={_config["stock_api_key"]}");

                if (q1LastYear.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Stream q1Data = await q1LastYear.Content.ReadAsStreamAsync();
                    q1DividendsLastYear = (await JsonSerializer.DeserializeAsync<DivData[]>(q1Data)).Where(x => x.Dividend != null && x.PaymentDate != null).ToArray();
                }
                if (q2LastYear.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Stream q2Data = await q2LastYear.Content.ReadAsStreamAsync();
                    q2DividendsLastYear = (await JsonSerializer.DeserializeAsync<DivData[]>(q2Data)).Where(x => x.Dividend != null && x.PaymentDate != null).ToArray();
                }
                if (q3LastYear.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Stream q3Data = await q3LastYear.Content.ReadAsStreamAsync();
                    q3DividendsLastYear = (await JsonSerializer.DeserializeAsync<DivData[]>(q3Data)).Where(x => x.Dividend != null && x.PaymentDate != null).ToArray();
                }
                if (q4LastYear.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Stream q4Data = await q4LastYear.Content.ReadAsStreamAsync();
                    q4DividendsLastYear = (await JsonSerializer.DeserializeAsync<DivData[]>(q4Data)).Where(x => x.Dividend != null && x.PaymentDate != null).ToArray();
                }

                // this year's dividend data
                HttpResponseMessage q1ThisYear = await client.GetAsync($"{_config["stock_api_url"]}/api/v3/stock_dividend_calendar?from={DateTime.Now.Year}-01-01&to={DateTime.Now.Year}-03-31&apikey={_config["stock_api_key"]}");
                HttpResponseMessage q2ThisYear = await client.GetAsync($"{_config["stock_api_url"]}/api/v3/stock_dividend_calendar?from={DateTime.Now.Year}-04-01&to={DateTime.Now.Year}-06-30&apikey={_config["stock_api_key"]}");
                HttpResponseMessage q3ThisYear = await client.GetAsync($"{_config["stock_api_url"]}/api/v3/stock_dividend_calendar?from={DateTime.Now.Year}-07-01&to={DateTime.Now.Year}-09-30&apikey={_config["stock_api_key"]}");
                HttpResponseMessage q4ThisYear = await client.GetAsync($"{_config["stock_api_url"]}/api/v3/stock_dividend_calendar?from={DateTime.Now.Year}-10-01&to={DateTime.Now.Year}-12-31&apikey={_config["stock_api_key"]}");

                if (q1ThisYear.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Stream q1Data = await q1ThisYear.Content.ReadAsStreamAsync();
                    q1DividendsThisYear = (await JsonSerializer.DeserializeAsync<DivData[]>(q1Data)).Where(x => x.Dividend != null && x.PaymentDate != null).ToArray();
                }
                if (q2ThisYear.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Stream q2Data = await q2ThisYear.Content.ReadAsStreamAsync();
                    q2DividendsThisYear = (await JsonSerializer.DeserializeAsync<DivData[]>(q2Data)).Where(x => x.Dividend != null && x.PaymentDate != null).ToArray();
                }
                if (q3ThisYear.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Stream q3Data = await q3ThisYear.Content.ReadAsStreamAsync();
                    q3DividendsThisYear = (await JsonSerializer.DeserializeAsync<DivData[]>(q3Data)).Where(x => x.Dividend != null && x.PaymentDate != null).ToArray();
                }
                if (q4ThisYear.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Stream q4Data = await q4ThisYear.Content.ReadAsStreamAsync();
                    q4DividendsThisYear = (await JsonSerializer.DeserializeAsync<DivData[]>(q4Data)).Where(x => x.Dividend != null && x.PaymentDate != null).ToArray();
                }
            }

            Console.WriteLine($"\nDIVIDEND DATA GENERATION RESULTS:\nq1 last year - {q1DividendsLastYear.Length}\nq2 last year - {q2DividendsLastYear.Length}\nq3 last year - {q3DividendsLastYear.Length}\nq4 last year - {q4DividendsLastYear.Length}\nq1 this year - {q1DividendsThisYear.Length}\nq2 this year - {q2DividendsThisYear.Length}\nq3 this year - {q3DividendsThisYear.Length}\nq4 this year - {q4DividendsThisYear.Length}\n");

            ALL_DIVDEND_DATA = q1DividendsLastYear.Concat(q2DividendsLastYear).Concat(q3DividendsLastYear).Concat(q4DividendsLastYear).Concat(q1DividendsThisYear).Concat(q2DividendsThisYear).Concat(q3DividendsThisYear).Concat(q4DividendsThisYear).ToArray();

            if (ALL_DIVDEND_DATA != null && ALL_DIVDEND_DATA.Length > 0)
            {
                int recordsAdded = 0;
                List<DivData> validDividendData = new List<DivData>();

                // loop through all dividend data and only add data that is associated with a company stored
                foreach (var x in ALL_DIVDEND_DATA)
                {
                    var companyProfile = _db.Companies.FirstOrDefault(c => c.CompanySymbol == x.Symbol);
                    if (companyProfile != null)
                    {
                        recordsAdded++;
                        x.CompanyProfile = companyProfile;
                        validDividendData.Add(x);
                    }
                }

                await _db.Dividends.AddRangeAsync(validDividendData);

                // remove all records 
                var allRecords = await _db.Dividends.ToListAsync();
                _db.Dividends.RemoveRange(allRecords);

                Console.WriteLine($"Successfully removed {allRecords.Count} dividend records, preparing to add new ones");

                // save changes
                await _db.SaveChangesAsync();
                Console.WriteLine("SUCCESSFULLY UPDATED ALL RECORDS IN DATABASE\nTHERE ARE " + recordsAdded + " NOW STORED");

                HttpContext.Response.StatusCode = 200;
                return "Successfully updated " + recordsAdded + " records!";
            }

            HttpContext.Response.StatusCode = 200;
            return "ALL_DIVDEND_DATA was either null or had no dividend data to update\ndatabase was not updated.";
        }
        catch (Exception err)
        {
            return err.ToString();
        }
    }
}

public class CompanyJsonData
{
    public string symbol { get; set; }
    public string name { get; set; }
    public string exchangeShortName { get; set; }
    public string type { get; set; }
}