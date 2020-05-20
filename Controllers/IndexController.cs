using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using WebMobile.Data.Interfaces;
using WebMobile.Data.Models;
using WebMobile.Data.Repository;
using WebMobile.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebMobile.Controllers
{
    public class IndexController : Controller
    {
        const int __ERROR_UNKNOWN = 0;
        const int __ERROR_USER_EXISTS = 1;
        const int __ERROR_NUMBER_EXISTS = 2;
        const int __ERROR_USER_NOT_EXISTS = 3;
        const int __ERROR_INCORRECT_NUMBER_FORMAT = 4;
        const int __ERROR_INCORRECT_USERINFO_FORMAT = 5;

        private readonly IPhones _phones;
        private IDictionary<string, string> _ajaxResult = null;

        public IndexController(IPhones iPhones)
        {
            _phones = iPhones;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var descriptor = filterContext.ActionDescriptor as ControllerActionDescriptor;

            if (descriptor.ActionName.IndexOf("Ajax") == 0) {
                _ajaxResult = new Dictionary<string, string> { { "result", "true" } };
            }

            base.OnActionExecuting(filterContext);
        }

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

            if (!phoneRepository.IsCorrectFormat(phone.number)) {
                _SetError(__ERROR_INCORRECT_NUMBER_FORMAT);
            } else if (phoneRepository.IsNumberExists(phone.number)) {
                _SetError(__ERROR_NUMBER_EXISTS);
            } else if (!userRepository.IsExists(phone.userId)) {
                _SetError(__ERROR_USER_NOT_EXISTS);
            } else if (!phoneRepository.AddNumber(phone)) {
                _SetError(__ERROR_UNKNOWN);
            }

            _JsonAnswer();
        }

        public void AjaxNewUser()
        {
            UserRepository userRepository = new UserRepository();

            User newUser = new User
            {
                firstName = Request.Form["uname"].ToString(),
                surname = Request.Form["sname"].ToString(),
                fatherName = Request.Form["fname"].ToString(),
                address = Request.Form["address"].ToString()
            };

            if (!userRepository.IsCorrectInfoFormat(newUser)) {
                _SetError(__ERROR_INCORRECT_USERINFO_FORMAT);
            } else if (userRepository.IsUserExists(newUser)) {
                _SetError(__ERROR_USER_EXISTS);
            } else if (!userRepository.AddUser(newUser)) {
                _SetError(__ERROR_UNKNOWN);
            }

            _JsonAnswer();
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

            if (!phoneRepository.IsExists(phone.id)) {
                _SetError(__ERROR_UNKNOWN);
            } else if (!phoneRepository.IsCorrectFormat(phone.number)) {
                _SetError(__ERROR_INCORRECT_NUMBER_FORMAT);
            } else if (phoneRepository.IsNumberExists(phone.number)) {
                _SetError(__ERROR_NUMBER_EXISTS);
            } else if (!userRepository.IsExists(phone.userId)) {
                _SetError(__ERROR_USER_NOT_EXISTS);
            } else if (!phoneRepository.SaveNumber(phone)) {
                _SetError(__ERROR_UNKNOWN);
            }

            _JsonAnswer();
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

            if (!userRepository.IsExists(user.id)) {
                _SetError(__ERROR_USER_NOT_EXISTS);
            } else if (!userRepository.IsCorrectInfoFormat(user)) {
                _SetError(__ERROR_INCORRECT_USERINFO_FORMAT);
            } else if (userRepository.IsUserExists(user)) {
                _SetError(__ERROR_USER_EXISTS);
            } else if (!userRepository.SaveUser(user)) {
                _SetError(__ERROR_UNKNOWN);
            }

            _JsonAnswer();
        }


        private void _SetError(int code)
        {
            _ajaxResult["result"] = "false";
            _ajaxResult["errorCode"] = code.ToString();
            HttpContext.Response.StatusCode = 403;
        }

        private void _JsonAnswer()
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_ajaxResult));

            HttpContext.Response.ContentType = "application/json";
            HttpContext.Response.Body.Write(data, 0, data.Length);
        }
    }
}