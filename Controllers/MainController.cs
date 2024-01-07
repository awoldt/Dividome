﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dividend_calendar.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace dividend_calendar.Controllers;

public class MainController : Controller
{
    string[] hotStocks = ["JNJ", "PG", "KO", "MSFT", "PFE", "T", "XOM", "VZ", "INTC", "MMM", "MO", "WFC", "PEP", "RILY", "IBM", "AXP", "HD", "UPS", "AAPL", "SBUX", "WMT", "LMT", "MCD", "UNH"];

    private readonly ILogger<MainController> _logger;
    private readonly IConfiguration _config;

    public MainController(ILogger<MainController> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    [HttpGet]
    [Route("/")]
    public async Task<IActionResult> Index()
    {
        try
        {
            using (var db = new Db(_config))
            {
                var nLastYear = await db.Dividends.Where(x => x.PaymentDate.Substring(0, 4) == (DateTime.Now.Year - 1).ToString()).ToArrayAsync();
                var nThisYear = await db.Dividends.Where(x => x.PaymentDate.Substring(0, 4) == DateTime.Now.Year.ToString()).ToArrayAsync();
                var popularStocksData = await db.Dividends.Where(d => hotStocks.Contains(d.Symbol)).Include(x => x.CompanyProfile).ToArrayAsync();

                List<DivData> popularStocksDisplayed = new List<DivData>();
                foreach (var x in popularStocksData)
                {
                    DateTime results;
                    if (DateTime.TryParse(x.PaymentDate, out results) && !popularStocksDisplayed.Contains(x) && results > DateTime.Now && x.CompanyProfile != null)
                    {
                        popularStocksDisplayed.Add(x);
                    }
                }

                return View(new IndexPageModel(nLastYear, nThisYear, popularStocksDisplayed));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return View("Error");
        }
    }

    [HttpGet]
    [Route("/{companySymbol}")]
    public async Task<IActionResult> CompanyPage(string companySymbol)
    {
        try
        {
            using (var db = new Db(_config))
            {
                // make sure that this symbol exists in the database
                var company = await db.Companies
                    .Include(c => c.Dividends)
                    .FirstOrDefaultAsync(c => c.CompanySymbol == companySymbol.ToUpper());

                if (company == null)
                {
                    HttpContext.Response.StatusCode = 404;
                    return View("NotFound");
                }

                // return an array of companies in the same stock exchange AND has dividends AND stock symbol starts with the same letter
                var relatedCompanies = await db.Companies.Where(c => c.Dividends.Count > 0 && c.StockExchange == company.StockExchange && c.CompanySymbol.Substring(0, 1).ToUpper() == company.CompanySymbol.Substring(0, 1).ToUpper()).ToListAsync();

                return View(new CompanyPage(company, relatedCompanies));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return View("Error");
        }
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}