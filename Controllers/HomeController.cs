using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Com.CloudRail.SI;
using Com.CloudRail.SI.ServiceCode.Commands.CodeRedirect;
using Com.CloudRail.SI.Services;
using Coinbase.ObjectModel;
using Coinbase.Serialization;
using Coinbase.Converters;
using Coinbase;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using apiparttwo.Models;
using Newtonsoft.Json.Linq;

namespace apiparttwo.Controllers
{
    
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Bitcoin", Value = "BTC", Selected = true });
            items.Add(new SelectListItem { Text = "Etherum", Value = "ETH" });

           

            ViewBag.cryptos = items;
            return View();
        }
        public async Task<ActionResult> demo()
        {

            return View();
            
        }
        public async Task<ActionResult> getPrice(string crypto= "BTC", int unit=1)
        {
            string GetForums = string.Format("https://min-api.cryptocompare.com/data/top/exchanges?fsym={0}&tsym=USD",crypto);
            // string list;
            List<string> forums = new List<string>();
            List<ForumPrice> pricelist = new List<ForumPrice>();
            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(GetForums);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(GetForums);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();

                    //List<ForumPrice> pricelist = new List<ForumPrice>();
                    var apidata = JsonConvert.DeserializeObject<JObject>(data);
                    JObject jObject = new JObject(apidata);
                    forums = jObject.SelectTokens("Data[*].exchange").Select(x => (string)x).ToList();
                    foreach(string forum in forums)
                    {
                        ForumPrice forumPrice = new ForumPrice();
                        forumPrice.Name = forum;
                        forumPrice.Price = await ForumPrice(forum,crypto,unit);
                        pricelist.Add(forumPrice);
                       

                    }



                }

                return View(pricelist);
            }
        }
        public async Task<double> ForumPrice(string forum,string crypto,int unit)
        {
           // string GetForums = "https://min-api.cryptocompare.com/data/price?fsym=BTC&tsyms=USD&e=Bitfinex&extraParams=your_app_name";
            string url = String.Format("https://min-api.cryptocompare.com/data/price?fsym={1}&tsyms=USD&e={0}&extraParams=your_app_name",forum,crypto);
            //List<string> forums = new List<string>();
            Double price;
            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();

                    //List<ForumPrice> pricelist = new List<ForumPrice>();
                    var apidata = JsonConvert.DeserializeObject<JObject>(data);
                    JObject jObject = new JObject(apidata);
                    price = Convert.ToDouble(jObject.SelectToken("USD"));




                }
                else
                {
                    price = 0;
                }

                return price;
            }

        }
        

        
       

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}