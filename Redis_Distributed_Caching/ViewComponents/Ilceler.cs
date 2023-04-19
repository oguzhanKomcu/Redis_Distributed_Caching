using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Redis_Distributed_Caching.Models;
using System.Data.SqlClient;
using System.Data;
using System.Text;

namespace Redis_Distributed_Caching.ViewComponents
{
    public class Ilceler : ViewComponent
    {


        public IViewComponentResult Invoke(string sehirAdi)
        {



            string connString = "Data Source=DESKTOP-MBGVKF7;Initial Catalog=NationalAddressDB;Integrated Security=True;Trusted_Connection=True;";
            // Veritabanı sorgusu
            string queryIlce = $"SELECT * FROM Ilceler where SehirAdi = '{sehirAdi}' order by IlceAdi asc ";

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



            return View(listIlce);
        }




    }
}
