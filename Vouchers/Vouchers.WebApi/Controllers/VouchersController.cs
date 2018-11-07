using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Vouchers.WebApi.Requests;

namespace Vouchers.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VouchersController : ControllerBase
    {
        protected IMongoDatabase _database;
        private IMongoCollection<BsonDocument> _vouchers;

        public VouchersController(IMongoDatabase database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _vouchers = _database.GetCollection<BsonDocument>("Vouchers");
        }

        // GET: api/Vouchers/RTGHG-VF34
        [HttpGet("{voucherCode}", Name = "Get")]
        public async Task<IActionResult> Get(string voucherCode)
        {
            var filter = new BsonDocument("VoucherCode", voucherCode);
            var result = await _vouchers.Find(filter).Project<dynamic>("{_id:0, VoucherCode:1, Money:1}").SingleOrDefaultAsync();
            if(result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // POST: api/Vouchers
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] CreateVoucherRequest request)
        {
            var voucherCode = GenerateVoucherCode();
            var filter = new BsonDocument("VoucherCode", voucherCode);
            var result = await _vouchers.Find(filter).SingleOrDefaultAsync();
            while (result != null)
            {
                voucherCode = GenerateVoucherCode();
                filter = new BsonDocument("voucherCode", voucherCode);
                result = await _vouchers.Find(filter).SingleOrDefaultAsync();
            }

            if(request.Money.Amount < 0 || !new string[] {"GBP", "UAH", "USD", "EUR"}.Contains(request.Money.Currency))
            {
                return BadRequest("Invalid data. Please check data you typed and try again.");
            }

            var document = new BsonDocument("VoucherCode", voucherCode)
                .Set("Money", BsonSerializer.Deserialize<BsonDocument>(JsonConvert.SerializeObject(request.Money)));

            await _vouchers.InsertOneAsync(document);
            return Ok(new { VoucherCode = voucherCode, Money = request.Money });
        }

        // POST: api/vouchers/payment
        [HttpPost("payment")]
        public async Task<IActionResult> PostPayment([FromBody] PaymentRequest request)
        {
            var filter = new BsonDocument("VoucherCode", request.VoucherCode);
            var result = await _vouchers.Find(filter).SingleOrDefaultAsync();
            if (result != null)
            {
                if(result.Contains("IsUsed"))
                {
                    var isVoucherUsed = BsonSerializer.Deserialize<bool>(result["IsUsed"].ToString());
                    if (isVoucherUsed)
                    {
                        return BadRequest($"Voucher with code {request.VoucherCode} is already used.");
                    }
                }

                var voucherMoney = BsonSerializer.Deserialize<Money>(result["Money"].ToJson());
                if(voucherMoney.Currency == request.Currency)
                {
                    if (voucherMoney.Amount >= request.Amount)
                    {
                        var update = Builders<BsonDocument>.Update
                            .Set("IsUsed", true);
                        await _vouchers.FindOneAndUpdateAsync(filter, update);
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("The amount of the voucher is not enough to pay.");
                    }
                }
                else
                {
                    return BadRequest("Payment in a currency other than the voucher currency is not possible.");
                }
            }
            else
                return NotFound();
        }

        [NonAction]
        private static string GenerateVoucherCode()
        {
            var random = new Random();
            char[] keys = "ABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890".ToCharArray();
            var firstPart =  Enumerable
                .Range(1, 5) 
                .Select(k => keys[random.Next(0, keys.Length - 1)]) 
                .Aggregate("", (e, c) => e + c);

            var secondPart = Enumerable
                .Range(1, 4)
                .Select(k => keys[random.Next(0, keys.Length - 1)]) 
                .Aggregate("", (e, c) => e + c);

            return $"{firstPart}-{secondPart}";
        }
    }
}
