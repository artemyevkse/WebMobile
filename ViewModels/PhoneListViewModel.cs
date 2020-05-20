using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebMobile.Data.Models;

namespace WebMobile.ViewModels
{
    public class PhoneListViewModel
    {
        public IEnumerable<Phone> allPhones { get; set; }
        public IEnumerable<User> allUsers { get; set; }
    }
}
