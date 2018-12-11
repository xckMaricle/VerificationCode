using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VerificationCode.Test.Models;

namespace VerificationCode.Test.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [VerificationCode(CodeField: "Code")]
        [HttpPost]
        public IActionResult Login(LoginModel login)
        {
            if (ModelState.IsValid)
            {
                if (login.UserName == "admin" && login.PassWord == "admin")
                {
                    return Json(new { msg = "登陆成功!", status = "ok" });
                }
                else
                {
                    return Json(new { msg = "账号密码错误!", status = "error" });
                }
            }
            return Json(new { msg = ModelState.Where(w => w.Value.Errors.Any() == true).FirstOrDefault().Value.Errors.FirstOrDefault()?.ErrorMessage, status = "error" });
        }
    }
}
