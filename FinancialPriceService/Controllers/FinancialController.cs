using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class FinancialController : ControllerBase
{
    private static readonly List<string> _symbols = new List<string> { "EURUSD", "USDJPY", "BTCUSD" };

    [HttpGet("instruments")]
    public ActionResult<IEnumerable<string>> GetInstruments()
    {
        return Ok(_symbols);
    }

    [HttpGet("price/{symbol}")]
    public async Task<ActionResult<decimal>> GetPrice(string symbol)
    {
        string fromCurrency, toCurrency;
        if (symbol == "EURUSD")
        {
            fromCurrency = "EUR";
            toCurrency = "USD";
        }
        else if (symbol == "USDJPY")
        {
            fromCurrency = "USD";
            toCurrency = "JPY";
        }
        else if (symbol == "BTCUSD")
        {
            fromCurrency = "BTC";
            toCurrency = "USD";
        }
        else
        {
            return BadRequest("Invalid symbol");
        }
        // didn't use environment variables to make it easier for you to test it
        var apiKey = "M27RVYKLER0F0YXS"; 
        var client = new RestClient("https://www.alphavantage.co");
        var request = new RestRequest($"query?function=CURRENCY_EXCHANGE_RATE&from_currency={fromCurrency}&to_currency={toCurrency}&apikey={apiKey}", Method.Get);
        var response = await client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            return StatusCode((int)response.StatusCode, response.StatusDescription);
        }

        try
        {
            var jsonResponse = JsonDocument.Parse(response.Content);
            if (jsonResponse.RootElement.TryGetProperty("Realtime Currency Exchange Rate", out var exchangeRateElement) &&
                exchangeRateElement.TryGetProperty("5. Exchange Rate", out var exchangeRateValue))
            {
                var exchangeRateString = exchangeRateValue.GetString();
                if (decimal.TryParse(exchangeRateString, NumberStyles.Any, CultureInfo.InvariantCulture, out var exchangeRate))
                {
                    return Ok(exchangeRate);
                }
                else
                {
                    return BadRequest("Invalid exchange rate format.");
                }
            }
            else
            {
                return NotFound("Exchange rate information not found in the response.");
            }
        }
        catch (JsonException)
        {
            return BadRequest("Invalid JSON response.");
        }
    }
}
