using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebMobile.Data.Interfaces;
using WebMobile.Data.Models;
using WebMobile.Data.Repository;
using WebMobile.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
/// <summary>
/// 
/// </summary>
namespace WebMobile.Controllers
{
    public class IndexController : Controller
    {
        const int _ERROR_UNKNOWN = 0;
        const int _ERROR_USER_EXISTS = 1;
        const int _ERROR_NUMBER_EXISTS = 2;
        const int _ERROR_USER_NOT_EXISTS = 3;
        const int _ERROR_INCORRECT_NUMBER_FORMAT = 4;
        const int _ERROR_INCORRECT_USERINFO_FORMAT = 5;

        private readonly IPhones _phones;
        private IDictionary<string, string> ajaxResult = new Dictionary<string, string>{{"result", "true"}};

        public IndexController(IPhones iPhones)
        {
            _phones = iPhones;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            PhoneListViewModel obj = new PhoneListViewModel();
            UserRepository userRepository = new UserRepository();

            obj.allPhones = _phones.complexPhones;
            obj.allUsers = userRepository.allUsers;

            ViewBag.Title = "Phone List";

            return View(obj);
        }

        public void AjaxNewNumber()
        {
            PhoneRepository phoneRepository = new PhoneRepository();
            UserRepository userRepository = new UserRepository();

            Phone phone = new Phone
            {
                number = Request.Form["number"].ToString(),
                userId = Int16.Parse(Request.Form["uid"].ToString())
            };

            if (!phoneRepository.IsCorrectFormat(phone.number))
            {
                SetError(_ERROR_INCORRECT_NUMBER_FORMAT);
            }
            else if(phoneRepository.IsNumberExists(phone.number))
            {
                SetError(_ERROR_NUMBER_EXISTS);
            }
            else if (!userRepository.IsExists(phone.userId))
            {
                SetError(_ERROR_USER_NOT_EXISTS);
            }
            else if(!phoneRepository.AddNumber(phone))
            {
                SetError(_ERROR_UNKNOWN);
            }

            JsonAnswer();
        }

        public void AjaxNewUser()
        {
            UserRepository userRepository = new UserRepository();
            User newUser = new User {
                firstName = Request.Form["uname"].ToString(),
                surname = Request.Form["sname"].ToString(),
                fatherName = Request.Form["fname"].ToString(),
                address = Request.Form["address"].ToString()
            };

            if (!userRepository.IsCorrectInfoFormat(newUser))
            {
                SetError(_ERROR_INCORRECT_USERINFO_FORMAT);
            }
            else if (userRepository.IsUserExists(newUser))
            {
                SetError(_ERROR_USER_EXISTS);
            }
            else if (!userRepository.AddUser(newUser))
            {
                SetError(_ERROR_UNKNOWN);
            }

            JsonAnswer();
        }

        public void AjaxEditNumber()
        {
            PhoneRepository phoneRepository = new PhoneRepository();
            UserRepository userRepository = new UserRepository();

            Phone phone = new Phone
            {
                id = Int16.Parse(Request.Form["pid"].ToString()),
                number = Request.Form["number"].ToString(),
                userId = Int16.Parse(Request.Form["uid"].ToString())
            };

            if (!phoneRepository.IsExists(phone.id))
            {
                SetError(_ERROR_UNKNOWN);
            }
            else if (!phoneRepository.IsCorrectFormat(phone.number))
            {
                SetError(_ERROR_INCORRECT_NUMBER_FORMAT);
            }
            else if (phoneRepository.IsNumberExists(phone.number))
            {
                SetError(_ERROR_NUMBER_EXISTS);
            }
            else if (!userRepository.IsExists(phone.userId))
            {
                SetError(_ERROR_USER_NOT_EXISTS);
            }
            else if (!phoneRepository.SaveNumber(phone))
            {
                SetError(_ERROR_UNKNOWN);
            }

            JsonAnswer();
        }

        public void AjaxEditUser()
        {
            UserRepository userRepository = new UserRepository();
            User user = new User
            {
                id = Int16.Parse(Request.Form["id"].ToString()),
                firstName = Request.Form["uname"].ToString(),
                surname = Request.Form["sname"].ToString(),
                fatherName = Request.Form["fname"].ToString(),
                address = Request.Form["address"].ToString()
            };

            if (!userRepository.IsExists(user.id))
            {
                SetError(_ERROR_USER_NOT_EXISTS);
            }
            else if (!userRepository.IsCorrectInfoFormat(user))
            {
                SetError(_ERROR_INCORRECT_USERINFO_FORMAT);
            }
            else if (userRepository.IsUserExists(user))
            {
                SetError(_ERROR_USER_EXISTS);
            }
            else if (!userRepository.SaveUser(user))
            {
                SetError(_ERROR_UNKNOWN);
            }

            JsonAnswer();
        }


        private void SetError(int code)
        {
            ajaxResult["result"] = "false";
            ajaxResult["errorCode"] = code.ToString();
        }

        private void JsonAnswer()
        {
            string jsonData = JsonConvert.SerializeObject(ajaxResult);
            byte[] data = System.Text.Encoding.UTF8.GetBytes(jsonData);

            if (ajaxResult["result"] == "false")
            {
                HttpContext.Response.StatusCode = 403;
            }

            HttpContext.Response.ContentType = "application/json";
            HttpContext.Response.Body.Write(data, 0, data.Length);
        }
    }
}
