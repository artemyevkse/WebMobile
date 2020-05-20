using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMobile.Data.Interfaces;
using WebMobile.Data.Models;

namespace WebMobile.Data.Repository
{
    public class PhoneRepository : IPhones
    {
        public IEnumerable<Phone> phones
        {
            get
            {
                using (var db = new DbWebMobile()) {
                    var query = from p in db.Phone
                                orderby p.number descending
                                select p;

                    return query.ToList();
                }
            }
        }

        public IEnumerable<Phone> complexPhones
        {
            get
            {
                using (var db = new DbWebMobile()) {
                    var query = from p in db.Phone
                                join u in db.User on p.userId equals u.id
                                orderby p.number descending
                                select Phone.Build(p, u);

                    return query.ToList();
                }
            }
        }

        public bool IsNumberExists(string phoneNumber)
        {
            using (var db = new DbWebMobile()) {
                var query = from p in db.Phone.Take(1)
                            where p.number == phoneNumber
                            select p;

                return (query.Count() > 0);
            }
        }

        public bool IsExists(int id)
        {
            using (var db = new DbWebMobile()) {
                var query = from p in db.Phone
                            where p.id == id
                            select p;

                return (query.Count() > 0);
            }
        }

        public bool AddNumber(Phone phone)
        {
            using (var db = new DbWebMobile()) {
                return db.Insert(phone) > 0;
            }
        }

        public bool SaveNumber(Phone phone)
        {
            using (var db = new DbWebMobile()) {
                return db.Update(phone) > 0;
            }
        }

        public bool IsCorrectFormat(string number)
        {
            if (number.Length != 11) {
                return false;
            }

            for (int i = 0; i < 11; i++) {
                if (!Char.IsDigit(number[i])) {
                    return false;
                }
            }

            return true;
        }
    }
}