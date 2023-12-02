using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Xml.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.WebJobs.Description;

namespace GetStockPrice
{
    public static class GetResponse
    {
        private readonly static string myAPIKey = "TEJXDAEZW4W2RXRP";   //ref url: https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol=IBM&apikey=demo
        private static string favouriteStock = "IBM";

        [FunctionName("GetResponse")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string responseMessage = string.Empty;
            log.LogInformation("HTTP trigger function processed a stock quote request.");
            
            try
            {
                string stockName = req.Query["symbol"];

                string sysmbol = String.IsNullOrEmpty(stockName) ? favouriteStock : stockName;

                string url = string.Format("https://www.alphavantage.co/query?function=OVERVIEW&symbol={0}&apikey={1}", sysmbol, myAPIKey);

                log.LogInformation(string.Format("Url: {0}", url));

                // Call Your  API
                HttpClient newClient = new HttpClient();
                HttpRequestMessage newGetStockRequest = new HttpRequestMessage(HttpMethod.Get, url);

                //Read Server Response
                HttpResponseMessage response = await newClient.SendAsync(newGetStockRequest);

                var json = await response.Content.ReadAsStringAsync();
                //Deserialize your json data and feed it to in class object
                var StockDetail = JsonConvert.DeserializeObject<StockModel>(json);

                //Below parsing is addtional, suit for a few attributes and an alternate method to get property value
                //You can comment the below code and related to variable 'details'
                var details = JObject.Parse(json);

                //After Json deserialization into class, read property from class value
                var ebiTDA           = StockDetail.EBITDA;
                var dividendPerShare = StockDetail.DividendPerShare;
                var ePS              = StockDetail.EPS;
                var fiftyTwoWeekHigh = StockDetail.FiftyTwoWeekHigh;

                log.LogInformation(string.Format("Stock: {0}", details["Symbol"]));
                log.LogInformation("------------------");
                log.LogInformation(string.Format("EBITDA: {0}", details["EBITDA"]));
                log.LogInformation("------------------");
                log.LogInformation(string.Format("dividendPerShare: {0}", details["DividendPerShare"]));
                log.LogInformation("------------------");
                log.LogInformation(string.Format("EPS: {0}", details["EPS"]));
             

                //Return Stock Details
                responseMessage = String.Format("dividendPerShare: {0},  EBITDA: {1}, EPS: {2}, 52WeeksHigh: {3}", dividendPerShare, ebiTDA, ePS, fiftyTwoWeekHigh);
                log.LogInformation(responseMessage);
                return new OkObjectResult(responseMessage);
            }
            catch (Exception ex)
            {
                responseMessage = "Error occured: " + ex.Message;
                return new NotFoundObjectResult(responseMessage);
            }

        }
    }

    public class StockModel
    {
        public string symbol { get; set; }
        public string Name { get; set; }
        public string CIK {get; set; }
        public string Exchange { get; set; }
        public string Currency { get; set; }
        public string Country { get; set; }
        public string Sector { get; set; }
        public string Industry { get; set; }
        public string PERatio {  get; set; }
        public string PEGRatio {  get; set; }
        public string BookValue {  get; set; }
        public string DividendPerShare { get; set; }
        public string DividendYield {  get; set; }
        public string EPS {  get; set; }
        public string EBITDA { get; set; }
        public string RevenuePerShareTTM { get; set; }
        public string RevenueTTM { get; set; }
        public string GrossProfitTTM { get; set; }
        public string QuarterlyEarningsGrowthYOY { get; set; }
        public string QuarterlyRevenueGrowthYOY { get; set; }
        public string AnalystTargetPrice { get; set; }
        public string TrailingPE {  get; set; }
        public string ForwardPE { get; set; }
        public string PriceToSalesRatioTTM { get; set; }
        public string PriceToBookRatio {  get; set; }

        [JsonProperty("52WeekHigh")]
        public string FiftyTwoWeekHigh { get; }

        [JsonProperty("52WeekLow")]
        public string FiftyTwoWeekLow { get; }

        [JsonProperty("50DayMovingAverage")]
        public string FiftyDayMovingAverage { get; }

        [JsonProperty("200DayMovingAverage")]
        public string TwoHundredDayMovingAverage { get; }
        public string SharesOutstanding { get; }
 
    }
}
