using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace ThirdLogin.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
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
        public ActionResult ThirdLoginView()
        {
            string email = Request.QueryString["email"];
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.email = "未登入";
            }
            else
                ViewBag.email = email;
            return View();
        }

        [HttpPost]
        public ActionResult ThirdLoginView(string code, string state)
        {
            return View();
        }
        public ActionResult GuideView()
        {
            string code = Request.QueryString["code"];
            string state = Request.QueryString["state"];
            string ID = "";
            string name = "";
            string email = "";

            if (!String.IsNullOrEmpty(Request.QueryString["code"]) &&
                !String.IsNullOrEmpty(Request.QueryString["state"]))
            {
                string config = "https://localhost:44342/Home/GuideView";

                string client_id = "1655199286";
                string client_secret = "9422af91bdef5d91fa4254b676d09b2b";

                // 用授權碼呼叫API取得id_token
                string postData = $"grant_type=authorization_code&code={code}&redirect_uri={config}&" +
                                  $"client_id=" + client_id + "&client_secret=" + client_secret;

                string result = "";
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://api.line.me/oauth2/v2.1/token");

                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.Timeout = 800;//请求超时时间

                byte[] data = Encoding.UTF8.GetBytes(postData);
                req.ContentLength = data.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();

                //會得內容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }

                //整理JWT內容
                result = result.Replace(@"""", "").Replace("}", "");
                int i = result.IndexOf("id_token:");
                result = result.Substring(i, result.Length - i).Replace("id_token:", "");

                // 解析JWT，先安裝System.IdentityModel.Tokens.Jwt
                var handler = new JwtSecurityTokenHandler();
                var jwtPayload = handler.ReadJwtToken(result).Payload;
                ID = jwtPayload.Where(x => x.Key == "sub").ToArray()[0].Value.ToString();
                name = jwtPayload.Where(x => x.Key == "name").ToArray()[0].Value.ToString();
                email = jwtPayload.Where(x => x.Key == "email").ToArray()[0].Value.ToString();

                usermodel userData = new usermodel();
                userData.ID = ID;
                userData.User = name;
                userData.Email = email;
                userData.Type = "Line";


                string pwd = string.Empty;
                string rPwd = string.Empty;
                string status = string.Empty;
                string name1 = string.Empty;

                // 將管理者登入的 Cookie 設定成 Session Cookie
                bool isPersistent = false;
                name1 = userData.Email;
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                    email,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(30),
                    isPersistent,
                    name1,
                    FormsAuthentication.FormsCookiePath);

                string encTicket = FormsAuthentication.Encrypt(ticket);

                // Create the cookie.
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddDays(1);
                cookie.HttpOnly = true;
                Response.Cookies.Add(cookie);


                // 導回原本頁面
                return Redirect(state + "?type=LINE&email=" + email);
                //return Redirect("Index?age=16");//若Index()存在不可空型別的引數則必須傳遞引數值，後兩項若存在不可空型別的引數可參照此解決方法
            }

            return View();
        }

        //public ActionResult GuideView(string code, string state)
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult GuideView(string code, string state)
        //{
        //    return View();
        //}


    }
    public class usermodel
    {
        public string ID { get; set; }
        public string User { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
    }
}