using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Redis_Distributed_Caching.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using StackExchange.Redis;

namespace Redis_Distributed_Caching.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private IDistributedCache _distributedCache;
        public HomeController(ILogger<HomeController> logger, IDistributedCache distributedCache)
        {
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public async Task<IActionResult> Index()
        {
            // _distributedCache.Remove("sehirler");
            Console.WriteLine("VERİTABANIDNAN ÇEKİYOR" + DateTime.Now);
            // Veritabanı bağlantı dizesi
            string connString = "Data Source=DESKTOP-MBGVKF7;Initial Catalog=NationalAddressDB;Integrated Security=True;Trusted_Connection=True;";

            // Veritabanı sorgusu
            string query = "SELECT * FROM Sehirler order by SehirAdi asc ";

            // Veritabanı bağlantısı oluşturun
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connString);

            // Komut nesnesi oluşturun ve sorguyu belirtin
            SqlCommand cmd = new SqlCommand(query, conn);

            // Verileri DataTable nesnesine doldurun
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            List<City> list = new List<City>();

            foreach (DataRow row in dt.Rows)
            {

                City city = new City();
                city.Id = row["SehirId"].ToString();
                city.City_Name = row["SehirAdi"].ToString();
                list.Add(city);
            }

            var options = new DistributedCacheEntryOptions()
                     .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            string joinedList = string.Join(",", list);
            byte[] sehirlerBytes = Encoding.UTF8.GetBytes(joinedList);

            return View(list);

        }

         [HttpPost]
         public JsonResult GetIlceler(string sehirId)
         {
            string connString = "Data Source=DESKTOP-MBGVKF7;Initial Catalog=NationalAddressDB;Integrated Security=True;Trusted_Connection=True;";
            // Veritabanı sorgusu
            string queryIlce = "SELECT * FROM Ilceler order by IlceAdi asc ";

            // Veritabanı bağlantısı oluşturun
            System.Data.SqlClient.SqlConnection connıLCE = new System.Data.SqlClient.SqlConnection(connString);

            // Komut nesnesi oluşturun ve sorguyu belirtin
            SqlCommand cmdIlce = new SqlCommand(queryIlce, connıLCE);

            // Verileri DataTable nesnesine doldurun
            DataTable dtIlce = new DataTable();
            SqlDataAdapter daIlce = new SqlDataAdapter(cmdIlce);
            daIlce.Fill(dtIlce);

            List<Ilce> listIlce = new List<Ilce>();

            foreach (DataRow row in dtIlce.Rows)
            {

                Ilce ilce = new Ilce();
                ilce.Id = row["ilceId"].ToString();
                ilce.Ilce_Name = row["IlceAdi"].ToString();
                ilce.Il_Id = row["SehirId"].ToString();

                listIlce.Add(ilce);
            }

            var optionsIlce = new DistributedCacheEntryOptions()
                     .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            string joinedListIlce = string.Join(",", listIlce);
            byte[] sehirlerBytesIlce = Encoding.UTF8.GetBytes(joinedListIlce);


            Console.WriteLine("VERİTABANIDNAN ÇEKTİ" + DateTime.Now);
            return Json(listIlce);
        }

        [HttpPost]
        public JsonResult GetSemtler(string sehirId)
        {
            string connString = "Data Source=DESKTOP-MBGVKF7;Initial Catalog=NationalAddressDB;Integrated Security=True;Trusted_Connection=True;";
            // Veritabanı sorgusu
            string queryIlce = "SELECT * FROM Ilceler order by IlceAdi asc ";

            // Veritabanı bağlantısı oluşturun
            System.Data.SqlClient.SqlConnection connıLCE = new System.Data.SqlClient.SqlConnection(connString);

            // Komut nesnesi oluşturun ve sorguyu belirtin
            SqlCommand cmdIlce = new SqlCommand(queryIlce, connıLCE);

            // Verileri DataTable nesnesine doldurun
            DataTable dtIlce = new DataTable();
            SqlDataAdapter daIlce = new SqlDataAdapter(cmdIlce);
            daIlce.Fill(dtIlce);

            List<Ilce> listIlce = new List<Ilce>();

            foreach (DataRow row in dtIlce.Rows)
            {

                Ilce ilce = new Ilce();
                ilce.Id = row["ilceId"].ToString();
                ilce.Ilce_Name = row["IlceAdi"].ToString();
                ilce.Il_Id = row["SehirId"].ToString();

                listIlce.Add(ilce);
            }

            var optionsIlce = new DistributedCacheEntryOptions()
                     .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            string joinedListIlce = string.Join(",", listIlce);
            byte[] sehirlerBytesIlce = Encoding.UTF8.GetBytes(joinedListIlce);


            Console.WriteLine("VERİTABANIDNAN ÇEKTİ" + DateTime.Now);
            return Json(listIlce);
        }


        [HttpPost]
        public async Task<IActionResult> SendMessage( string message)
        {
            var redis = await ConnectionMultiplexer.ConnectAsync("localhost:1453");
            var db = redis.GetDatabase();
            //ISubscriber sub = redis.GetSubscriber();
            await db.PublishAsync("mychannel", message);
       

            return Ok();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}