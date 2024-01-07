using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

public class Db : DbContext
{
    private readonly IConfiguration _configuration;

    public Db(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DbSet<CompanyProfile> Companies { get; set; }
    public DbSet<DivData> Dividends { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration["db_connection_string"]);
    }
}

/*  
    set all properties (excluding PK an FK) as nullable
    this will prevent errors during json deserilization from data fetched from API
*/

public class CompanyProfile
{
    [Key]
    [Column("id")]
    public int CompanyId { get; set; }

    [Column("company_name")]
    public string? CompanyName { get; set; }

    [Column("company_symbol")]
    public string? CompanySymbol { get; set; }

    [Column("company_description")]
    public string[]? CompanyDescription { get; set; }

    [Column("company_logo")]
    public string? CompanyLogo { get; set; }

    [Column("stock_exchange")]
    public string? StockExchange { get; set; }

    public ICollection<DivData> Dividends { get; set; } // if count is 0, company has no dividends
}

public class DivData
{
    [Key]
    [Column("id")]
    public int DividendId { get; set; }

    [Column("symbol")]
    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    [Column("dividend")]
    [JsonPropertyName("dividend")]
    public decimal? Dividend { get; set; }

    [Column("record_date")]
    [JsonPropertyName("recordDate")]
    public string? RecordDate { get; set; }

    [Column("payment_date")]
    [JsonPropertyName("paymentDate")]
    public string? PaymentDate { get; set; }

    [Column("declaration_date")]
    [JsonPropertyName("declarationDate")]
    public string? DeclarationDate { get; set; }

    // Foreign key property
    [ForeignKey("CompanyProfile")]
    [Column("company_id")]
    public int CompanyId { get; set; }

    // Navigation property for the parent company
    [InverseProperty("Dividends")]
    public CompanyProfile CompanyProfile { get; set; }
}
