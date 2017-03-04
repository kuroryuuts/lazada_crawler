using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinTrinhLibrary;
using System.Text.RegularExpressions;
using System.IO;

namespace lazada_crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Star Crawler....");
            Console.OutputEncoding = Encoding.UTF8;
            string fpath = "D:\\result.txt";//tạo file lưu trữ thông tin lấy được
            FileStream fs = new FileStream(fpath, FileMode.Create);           
            StreamWriter swriter = new StreamWriter(fs, Encoding.UTF8);
            //GET : http://www.lazada.vn/
            TinTrinhLibrary.WebClient client = new TinTrinhLibrary.WebClient(); //tạo kết nối
            for (int i = 1; i <= 100; i++) //vòng lặp tăng số trang lên, tăng i theo số trang cần lấy
            {
                string html = client.Get("http://www.lazada.vn/phu-kien-thiet-bi-xe-mo-to-xe-dia-hinh/?page=" + i, "http://www.lazada.vn/", ""); //vào trang web về thiết bị xe mo to xe dia hinh
                MatchCollection listLink = Regex.Matches(html, "<a href=\"/(.*?)\" class=\"c-product-card__name\">"); //danh sách link từng sản phẩm
                foreach (Match link in listLink) //vòng lặp để vào từng sản phẩm
                {
                    string html1 = client.Get("http://www.lazada.vn/" + link.Groups[1].Value, "http://www.lazada.vn/", "");//tạo đường link đến từng sản phẩm
                    Match motor_name = Regex.Match(html1, "title\" content=\"(.*?)\"/>"); //lấy tên sản phẩm
                    Match motor_trademark = Regex.Match(html1, "'><span>(.*?)</span></a>"); //lấy thương hiệu sản phẩm
                    Match motor_guarantee = Regex.Match(html1, "term\">(.*?)</span>", RegexOptions.Singleline); //lấy Bảo hành sản phẩm
                    Match motor_price = Regex.Match(html1, "special_price_box\">(.*?)</span>", RegexOptions.Singleline); //lấy giá về sản phẩm
                    Match motor_desc = Regex.Match(html1, "<li class=\"\"><span>(.*?)</span></li>", RegexOptions.Multiline); //lấy mô tả về sản phẩm
                    Match motor_pprice = Regex.Match(html1, " id=\"price_box\">(.*?),</span>", RegexOptions.Singleline); //lấy giá gốc của sản phẩm
                    Match motor_saving = Regex.Match(html1, "price_highlight\">(.*?)</span>", RegexOptions.Singleline); //lấy tiết kiệm của sản phẩm
                    Match motor_number = Regex.Match(html1, "stock-number\">(.*?)</span>", RegexOptions.Singleline); //lấy số lượng sản phẩm còn lại
                    Match motor_amortization = Regex.Match(html1, "\" rel=\".*?\">(.*?)»", RegexOptions.Singleline); //lấy hình thức trả góp sản phẩm
                    swriter.WriteLine("Tên Phụ kiện - Xe máy: " + motor_name.Groups[1].Value.Trim()); //in ra 
                    swriter.WriteLine("Thương hiệu: " + motor_trademark.Groups[1].Value.Trim());
                    swriter.WriteLine("Mô tả: " + motor_desc.Groups[1].Value.Trim());
                    swriter.WriteLine("Thông tin Bảo hành: " + motor_guarantee.Groups[1].Value.Trim());
                    swriter.WriteLine("Giá: " + motor_price.Groups[1].Value.Trim(), "VND");
                    swriter.WriteLine("Giá trước đây: " + motor_pprice.Groups[1].Value.Trim(), "VND");
                    swriter.WriteLine("Tiết kiệm: " + motor_saving.Groups[1].Value.Trim());
                    swriter.WriteLine("Số lượng: " + motor_number.Groups[1].Value.Trim());
                    swriter.WriteLine("Trả Góp: " + motor_amortization.Groups[1].Value.Trim());
                    swriter.WriteLine("\n====================================\n");
                }
            }
            swriter.Flush();
            fs.Close();
            Console.WriteLine("Ok");
            Console.ReadLine();
        }
    }
}

                
    
