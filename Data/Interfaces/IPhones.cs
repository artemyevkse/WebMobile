using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebMobile.Data.Models;

namespace WebMobile.Data.Interfaces
{
    public interface IPhones
    {
        IEnumerable<Phone> phones { get; }
        IEnumerable<Phone> complexPhones { get; }
    }
}
