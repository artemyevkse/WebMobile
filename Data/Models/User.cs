using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using LinqToDB.Mapping;

namespace WebMobile.Data.Models
{
    public class User
    {
        [PrimaryKey, Identity]
        public int id { set; get; }

        [Column(Name = "FirstName"), NotNull]
        public string firstName { set; get; }
        [Column(Name = "Surname"), NotNull]
        public string surname { set; get; }
        [Column(Name = "FatherName"), NotNull]
        public string fatherName { set; get; }

        [Column(Name = "Address"), NotNull]
        public string address { set; get; }

        public List<User> users { set; get; }
    }
}