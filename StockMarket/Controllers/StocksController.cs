using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using StockMarket.Models;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace StockMarket.Controllers
{
    [Route("api/[controller]")]
    public class StocksController : Controller
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IAppSettings _appSettings;

        public StocksController(IDistributedCache distributedCache, IAppSettings appSettings)
        {
            _distributedCache = distributedCache;
            _appSettings = appSettings;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var redis = ConnectionMultiplexer.Connect(_appSettings.RedisConnection);
            var server = redis.GetServer(_appSettings.RedisConnection);
            var keys = server.Keys();
            var stocks = new List<Stock>();
            foreach (var key in keys)
            {
                var code = key.ToString();
                var jsonValue = await _distributedCache.GetStringAsync(code.Substring(3));//db0(code) start from 3 index
                var stock = JsonConvert.DeserializeObject<Stock>(jsonValue);
                stocks.Add(stock);
            }
            return JsonConvert.SerializeObject(stocks);
        }


        // GET api/<controller>/5
        [HttpGet("{code}")]
        public async Task<string> Get(string code)
        {
            var jsonValue = await _distributedCache.GetStringAsync(code);
            var stock = JsonConvert.DeserializeObject<Stock>(jsonValue);
            return JsonConvert.SerializeObject(stock);
        }

        // POST api/<controller>
        [HttpPost]
        public async void Post()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var value = await reader.ReadToEndAsync();
                var stock = JsonConvert.DeserializeObject<Stock>(value);
                var code = stock.Code;
                await _distributedCache.SetStringAsync(code, value);
            }
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}