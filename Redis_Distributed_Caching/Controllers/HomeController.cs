﻿using Microsoft.AspNetCore.Mvc;
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
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json.Linq;
using Redis_Distributed_Caching.ViewComponents;

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
            //// _distributedCache.Remove("sehirler");
            //Console.WriteLine("VERİTABANIDNAN ÇEKİYOR" + DateTime.Now);
            //// Veritabanı bağlantı dizesi
            //string connString = "Data Source=DESKTOP-MBGVKF7;Initial Catalog=NationalAddressDB;User ID=sa;Password=1510oguz;Integrated Security=True;Trusted_Connection=True;";

            //// Veritabanı sorgusu
            //string query = "SELECT * FROM Sehirler order by SehirAdi asc "; //PlakaNo

            //// Veritabanı bağlantısı oluşturun
            //System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connString);

            //// Komut nesnesi oluşturun ve sorguyu belirtin
            //SqlCommand cmd = new SqlCommand(query, conn);

            //// Verileri DataTable nesnesine doldurun
            //DataTable dt = new DataTable();
            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //da.Fill(dt);

            //List<City> list = new List<City>();

            //foreach (DataRow row in dt.Rows)
            //{

            //    City city = new City();
            //    city.Id = row["SehirId"].ToString();
            //    city.City_Name = row["SehirAdi"].ToString();
            //    list.Add(city);
            //}

            //var options = new DistributedCacheEntryOptions()
            //         .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            //string joinedList = string.Join(",", list);
            //byte[] sehirlerBytes = Encoding.UTF8.GetBytes(joinedList);

            //return View(list);
            // Check if the "list" data is already in cache
            string cacheKey = "sehirler";
            string cacheData = await _distributedCache.GetStringAsync(cacheKey);

            if (cacheData != null)
            {
                // If the data is in cache, return it
                List<City> cachedList = JsonConvert.DeserializeObject<List<City>>(cacheData);
                return View(cachedList);
            }
            else
            {
                // If the data is not in cache, retrieve it from the database
                Console.WriteLine("VERİTABANIDNAN ÇEKİYOR" + DateTime.Now);
                string connString = "Data Source=DESKTOP-MBGVKF7;Initial Catalog=NationalAddressDB;User ID=sa;Password=1510oguz;Integrated Security=True;Trusted_Connection=True;";
                string query = "SELECT * FROM Sehirler order by SehirAdi asc ";
                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
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

                // Store the "list" data in cache
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                string serializedList = JsonConvert.SerializeObject(list);
                byte[] listBytes = Encoding.UTF8.GetBytes(serializedList);
                await _distributedCache.SetAsync(cacheKey, listBytes, options);

                return View(list);

            }
        }

        [HttpGet]
        public async Task<JsonResult> GetIlceler(int ilid)
        {
            //string connString = "Data Source=DESKTOP-MBGVKF7;Initial Catalog=NationalAddressDB;User ID=sa;Password=1510oguz;Integrated Security=True;Trusted_Connection=True;";
            //// Veritabanı sorgusu
            //string queryIlce = $"SELECT * FROM Ilceler where  SehirId = {ilid} order by IlceAdi asc ";

            //// Veritabanı bağlantısı oluşturun
            //System.Data.SqlClient.SqlConnection connıLCE = new System.Data.SqlClient.SqlConnection(connString);

            //// Komut nesnesi oluşturun ve sorguyu belirtin
            //SqlCommand cmdIlce = new SqlCommand(queryIlce, connıLCE);

            //// Verileri DataTable nesnesine doldurun
            //DataTable dtIlce = new DataTable();
            //SqlDataAdapter daIlce = new SqlDataAdapter(cmdIlce);
            //daIlce.Fill(dtIlce);

            //List<Ilce> listIlce = new List<Ilce>();

            //foreach (DataRow row in dtIlce.Rows)
            //{

            //    Ilce ilce = new Ilce();
            //    ilce.Id = row["ilceId"].ToString();
            //    ilce.Ilce_Name = row["IlceAdi"].ToString();
            //    ilce.Il_Id = row["SehirId"].ToString();

            //    listIlce.Add(ilce);
            //}

            //var optionsIlce = new DistributedCacheEntryOptions()
            //         .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            //string joinedListIlce = string.Join(",", listIlce);
            //byte[] sehirlerBytesIlce = Encoding.UTF8.GetBytes(joinedListIlce);
            //string ilceler = "<select class=\"form-select\" id=\"ilce-select2\">" +
            //    " <option selected>İlçe Seçiniz</option> ";
            //foreach (var item in listIlce)
            //{
            //   ilceler += $"<option value =\"{item.Id}\">{item.Ilce_Name}</option>";
            //}

            //     ilceler += "</select>";

            //Console.WriteLine("VERİTABANIDNAN ÇEKTİ" + DateTime.Now);
            //return Json(ilceler);

            var cacheKey = $"ilceler_{ilid}";
            var cachedIlceler = await _distributedCache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedIlceler))
            {
                Console.WriteLine("CACHE'DEN ÇEKİLDİ" + DateTime.Now);
                return Json(cachedIlceler);
            }

            string connString = "Data Source=DESKTOP-MBGVKF7;Initial Catalog=NationalAddressDB;User ID=sa;Password=1510oguz;Integrated Security=True;Trusted_Connection=True;";

            // Veritabanı sorgusu
            string queryIlce = $"SELECT * FROM Ilceler where  SehirId = {ilid} order by IlceAdi asc ";

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

            string ilceler = "<select class=\"form-select\" id=\"ilce-select2\">" +
                             " <option selected>İlçe Seçiniz</option> ";

            foreach (var item in listIlce)
            {
                ilceler += $"<option value =\"{item.Id}\">{item.Ilce_Name}</option>";
            }

            ilceler += "</select>";

            Console.WriteLine("VERİTABANIDNAN ÇEKİLDİ" + DateTime.Now);

            var optionsIlce = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            await _distributedCache.SetStringAsync(cacheKey, ilceler, optionsIlce);

            return Json(ilceler);

        }

        [HttpGet]
        public async Task<JsonResult> GetSemtler(string ilceid)
        {
            var cacheKey = $"semtMahalleler_{ilceid}";
            var cachedSemtMahalleler= await _distributedCache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedSemtMahalleler))
            {
                Console.WriteLine("CACHE'DEN ÇEKİLDİ" + DateTime.Now);
                return Json(cachedSemtMahalleler);
            }


            string connString = "Data Source=DESKTOP-MBGVKF7;Initial Catalog=NationalAddressDB;User ID=sa;Password=1510oguz;Integrated Security=True;Trusted_Connection=True;";
            // Veritabanı sorgusu
            string queryIlce = $"SELECT * from SemtMah where ilceId ={ilceid} order by SemtAdi asc ";

            // Veritabanı bağlantısı oluşturun
            System.Data.SqlClient.SqlConnection connıLCE = new System.Data.SqlClient.SqlConnection(connString);

            // Komut nesnesi oluşturun ve sorguyu belirtin
            SqlCommand cmdIlce = new SqlCommand(queryIlce, connıLCE);

            // Verileri DataTable nesnesine doldurun
            DataTable dtIlce = new DataTable();
            SqlDataAdapter daIlce = new SqlDataAdapter(cmdIlce);
            daIlce.Fill(dtIlce);

            List<SemtMah> listSemtMah = new List<SemtMah>();

            foreach (DataRow row in dtIlce.Rows)
            {

                SemtMah semtMah = new SemtMah();
                semtMah.SemtMahId = row["SemtMahId"].ToString();
                semtMah.SemtAdi = row["SemtAdi"].ToString();
                semtMah.PostaKodu = row["PostaKodu"].ToString();
                semtMah.ilceId = row["ilceId"].ToString();
                semtMah.MahalleAdi = row["MahalleAdi"].ToString();


                listSemtMah.Add(semtMah);
            }




            string ilceler = "<select class=\"form-select\" id=\"semtMahId-select\">" +
              " <option selected>Mahalle Seçiniz</option> ";
            foreach (var item in listSemtMah)
            {
                ilceler += $"<option value =\"{item.SemtMahId}\">{item.MahalleAdi}</option>";
            }

            ilceler += "</select>";

            Console.WriteLine("VERİTABANIDNAN ÇEKTİ" + DateTime.Now);


            var optionsIlce = new DistributedCacheEntryOptions()
                     .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            await _distributedCache.SetStringAsync(cacheKey, ilceler);
            return Json(ilceler);
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

        [HttpPost]
        public async Task<JsonResult> SaveAddress(string sehirId, string IlceId, string SemtMahId)
        {

            string connString = "Data Source=DESKTOP-MBGVKF7;Initial Catalog=NationalAddressDB;User ID=sa;Password=1510oguz;Trusted_Connection=True;";
            // Veritabanı sorgusu

            string queryAddAddress = $"INSERT INTO Adresler (Id, IlceId, SehirId, SemtMahId, UserId) VALUES ((SELECT MAX(Id) + 1 FROM Adresler),{IlceId}, {sehirId},{SemtMahId}, 1)";
            // Veritabanı bağlantısı oluşturun
            System.Data.SqlClient.SqlConnection connıLCE = new System.Data.SqlClient.SqlConnection(connString);

            // Komut nesnesi oluşturun ve sorguyu belirtin
            SqlCommand cmdIlce = new SqlCommand(queryAddAddress, connıLCE);

            connıLCE.Open();
            var result = cmdIlce.ExecuteNonQuery();
            connıLCE.Close();




            return Json("true");
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