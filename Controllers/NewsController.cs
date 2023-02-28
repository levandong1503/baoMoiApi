using BaoMoiAPI.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace BaoMoiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly Models.BaoMoiDbContext baoMoiDbContext;
        private readonly ILogger<NewsController> logger;
        public NewsController(BaoMoiDbContext baoMoiDbContext, ILogger<NewsController> logger)
        {
            this.baoMoiDbContext = baoMoiDbContext;
            this.logger = logger;
        }
        [HttpGet()]
        public IActionResult GetAll([FromQuery] int page, [FromQuery] int size)
        {
            Response.Headers["Access-Control-Allow-Origin"] = "*";
            Response.Headers["infor"] = "le van dong";
            var news = baoMoiDbContext.News.Skip((page - 1) * size).Take(size).Select(n => new { Id = n.Id, Title = n.Title }).ToList();
            return Ok(new
            {
                currentPage = page,
                totalPage = Math.Ceiling(Convert.ToDouble(baoMoiDbContext.News.Count())/size),
                data = news
            }) ;
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            logger.LogInformation("ban dang doc bai bao" + id);
            Response.Headers["Access-Control-Allow-Origin"] = "*";
            var news = baoMoiDbContext.News.Where(n => n.Id == id).FirstOrDefault();
            if (news == null)
            {
                return BadRequest(new { message = "id is not found" });
            }
            return Ok(new { Id = news.Id, 
                Title = news.Title, 
                Img = news.Img, 
                CreateAt = news.CreateAt.Value.ToString("dd/MM/yyy"),
                CreateBy = news.CreateBy,
                Description = news.Description,
                CateId = news.CateId,
                Content = news.Content
            });
        }
        [HttpPost]
        //[EnableCors("MyPolicy")]    
        public IActionResult Create([FromBody] News news)
        {
            logger.LogInformation("ban dang them bai bao: " + news.Title);
            Response.Headers["Access-Control-Allow-Origin"] = "http://127.0.0.1:5500";
            Response.Headers.Append("Origin", "localhost");
            Response.Headers.Append("allowCredentials", "true");
            Response.Headers.Append("Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT");
            Response.Headers["Access-Control-Allow-Credentials"] = "true";
            if (news == null)
            {
                return BadRequest(new { message = "Not data" });
            }
            else if (news.Title == null) {
                return BadRequest(new { message = "not title news " });
            }
            else if (Request.Cookies["username"] == null)
            {  // username

                return BadRequest(new { message = "you don't login : " + Request.Cookies.Count() });
            }

            string Acc = Request.Cookies["username"];
            news.CreateBy = baoMoiDbContext.Accounts.Where(a => a.Username == Acc).FirstOrDefault().Name;
            news.CreateAt = DateTime.Now;
            var abc = (from acc in baoMoiDbContext.Accounts
                       where acc.Username == Acc
                       select acc).FirstOrDefault();
            news.CreateAt = DateTime.Now;
            baoMoiDbContext.News.Add(news);
            int i = baoMoiDbContext.SaveChanges();
            return Ok(new { message = "add success", numberAdd = 1 });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            logger.LogInformation("ban dang xoa bai bao: " + id);
            Response.Headers["Access-Control-Allow-Origin"] = "*";
            News del = baoMoiDbContext.News.Where(n => n.Id == id).FirstOrDefault();
            if (del == null)
            {
                return BadRequest(new { message = "data is fail" });
            }
            baoMoiDbContext.News.Remove(del);
            int i = baoMoiDbContext.SaveChanges();
            return Ok(new { numberAdd = 1 });
        }

        [HttpPut]
        public IActionResult Update(int? id, News news)
        {
            //int id = 0;
            //bool isParse = int.TryParse(Request.Query["id"],out id);
            logger.LogInformation("ban dang sua bai bao: " + id);
            Response.Headers["Access-Control-Allow-Origin"] = "http://127.0.0.1:5500";
            if (id == null)
            {
                return BadRequest(new { message = "ban chua truyen id: " + Request.Query["id"] });
            }
            if (news == null)
            {
                return BadRequest();
            }
            else if (string.IsNullOrEmpty(news.Title ))
            {
                return BadRequest();
            }
            var newsupd = baoMoiDbContext.News.Where(n => n.Id == id).FirstOrDefault();
            if (newsupd == null)
            {
                return BadRequest();
            }
            var CateForNews = baoMoiDbContext.Categories.Where(n => n.Id == news.CateId).FirstOrDefault();
            if (CateForNews == null)
            {
                return BadRequest();
            }
            
            newsupd.Title = news.Title;
            newsupd.CreateAt = DateTime.Now;
            //newsupd.CreateBy = news.CreateBy;
            newsupd.CateId = news.CateId;
            newsupd.Description = news.Description;
            newsupd.Content = news.Content;
            if (!string.IsNullOrEmpty(news.Img))
            {
                newsupd.Img = news.Img;
            }
            int i = baoMoiDbContext.SaveChanges();
            return Ok(new { id = id, newsBind = news });
        }


        [HttpGet("home")]
        public async Task<IActionResult> GetHome()
        {
            logger.LogInformation("dang load trang chu");
            Response.Headers["access-control-allow-origin"] = "http://127.0.0.1:5500";


            var HotNews = await baoMoiDbContext.News.OrderByDescending(n => n.CreateAt).Take(5).Select(p => new {
                Id = p.Id,
                Title = p.Title,
                Img = p.Img,
                CreateAt = p.CreateAt.Value.ToString("dd/MM/yyyy"),
                CreateBy = p.CreateBy,
                Description = p.Description,
                CateId = p.CateId

            }).ToListAsync();
            List<Category> LstCate =  baoMoiDbContext.Categories.ToList();
            
            int?[] lstShow = new int?[3];
            int ranvalue, cnt = 0;  Random rnd = new Random();
            //Newtonsoft.Json.JsonConvert.SerializeObject(LstCate)
            Dictionary<string, object> dic = new Dictionary<string, object>();
            do
            {

                ranvalue = rnd.Next(0,LstCate.Count());
                //logger.LogCritical(ranvalue.ToString());
                bool isDup = false;
                foreach (var i in lstShow)
                {
                    if (i == ranvalue)
                    {
                        isDup = true;
                        break;
                    }
                }
                if (!isDup)
                {
                    lstShow[cnt] = ranvalue;
                    cnt++;
                }
            } while (cnt < 3);

            ArrayList arrNews = new ArrayList();
            foreach (var i in lstShow)
            {
                var lstNewsforI = baoMoiDbContext.News.Where(p => p.CateId == LstCate[i.Value].Id).OrderByDescending(n => n.CreateAt)
                    .Select(p => new {
                        Id = p.Id, 
                        Title = p.Title, 
                        Img = p.Img, 
                        CreateAt = p.CreateAt.Value.ToString("dd/MM/yyyy"), 
                        CreateBy = p.CreateBy,
                        Description = p.Description, 
                        CateId = p.CateId 
                    }).Take(3)
                    .ToArray();
                arrNews.Add(new
                {
                    category = LstCate[i.Value].Name,
                    data = lstNewsforI
                });
            }

            var res = new 
            {
                hotnews = HotNews,
                dataCate = arrNews
            };

            return Ok(res);
    }


        [HttpGet("search")]
        public IActionResult Search(string title)
        {
            Response.Headers["Access-Control-Allow-Origin"] = "*";
            Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS";
            List<News> LstNews = baoMoiDbContext.News.ToList();
            if(title == null)
            {
                return Ok(LstNews);

            }
            var news = LstNews.Where(n => n.Title.convertToUnSign().ToLower().Contains(title.convertToUnSign().ToLower())).Select(p => new {
                Id = p.Id,
                Title = p.Title,
                Img = p.Img,
                CreateAt = p.CreateAt.Value.ToString("dd/MM/yyyy"),
                CreateBy = p.CreateBy,
                Description = p.Description,
                CateId = p.CateId

            }).ToList();

            return Ok(news);
        }

        [HttpGet("searchunsign")]
        public IActionResult SearchUnSign(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return BadRequest(new { message = "chuoi cua ban nhap khong dung" });
            }
            var lstTitle = baoMoiDbContext.News.Select(p => p.Title).ToList();
            var qr = from item in lstTitle
                     where item.convertToUnSign().ToLower().Contains(title.convertToUnSign().ToLower())
                     select item;
            var qrTest = from item in lstTitle
                         select item.convertToUnSign().ToLower();
            return Ok(new { result = qr.ToList(), test = qrTest.ToList() }) ;
        }

        [HttpOptions]
        public IActionResult Check()
        {
            //int? a = null;
            //Console.Write(a.Value);
            //logger.LogInformation("check call api api news");
            Response.Headers["access-control-allow-headers"] = "content-type,mode, access-control-allow-origin";
            //Response.Headers["Access-Control-Max-Age"] = "86400";
            Response.Headers["access-control-allow-methods"] = "POST , GET , PUT , OPTIONS";
            Response.Headers["access-control-allow-origin"] = "http://127.0.0.1:5500";
            Response.Headers["Access-Control-Allow-Credentials"] = "true";
            return Ok(new { message = "may k chay a" });
        }

        [HttpOptions("{id}")]
        public IActionResult CheckDlete()
        {
            //int? a = null;
            //Console.Write(a.Value);
            //logger.LogInformation("check call api api news");
            Response.Headers["access-control-allow-headers"] = "access-control-allow-origin,content-type";
            //Response.Headers["Access-Control-Max-Age"] = "86400";
            Response.Headers["access-control-allow-methods"] = "POST , GET , OPTIONS, DELETE";
            Response.Headers["access-control-allow-origin"] = "*";
            //Response.Headers["Access-Control-Allow-Credentials"] = "true";
            return Ok(new { message = "may k chay a" });
        }
    }


    public static class ResponiveHome
    {
        public static string convertToUnSign(this string strVietnamese)
        {
            const string FindText = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ #%&*:|.";
            const string ReplText = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY-       ";
            int index = -1;
            while ((index = strVietnamese.IndexOfAny(FindText.ToCharArray())) != -1)
            {
                int index2 = FindText.IndexOf(strVietnamese[index]);
                strVietnamese = strVietnamese.Replace(strVietnamese[index], ReplText[index2]);
            }
            return strVietnamese;
        }
        public static string ConvertToVietnameseNotSignature(string strVietnamese)
        {
            const string FindText = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ #%&*:|.";
            const string ReplText = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY-       ";
            int index = -1;
            while ((index = strVietnamese.IndexOfAny(FindText.ToCharArray())) != -1)
            {
                int index2 = FindText.IndexOf(strVietnamese[index]);
                strVietnamese = strVietnamese.Replace(strVietnamese[index], ReplText[index2]);
            }
            return strVietnamese;
        }
        //public List<News> hotnews { set; get; }
        //public Dictionary<string, object> dataCate { set; get; }
    } 




}
