using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using LinqToDB.Mapping;

namespace WebMobile.Data.Models
{
    public class Phone
    {
        [PrimaryKey, Identity]
        public int id { set; get; }

        [Column(Name = "Number"), NotNull]
        public string number { set; get; }

        [Column(Name = "UserId"), NotNull]
        public int userId { set; get; }

        public virtual User user { set; get; }
        public List<Phone> phones { set; get; }


        public static Phone Build(Phone phone, User user)
        {
            if (phone != null)
            {
                phone.user = user;
            }

            return phone;
        }
    }
}
