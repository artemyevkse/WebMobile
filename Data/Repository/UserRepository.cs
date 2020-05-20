using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMobile.Data.Interfaces;
using WebMobile.Data.Models;

namespace WebMobile.Data.Repository
{
    public class UserRepository : IUsers
    {
        public IEnumerable<User> allUsers
        {
            get
            {
                using (var db = new DbWebMobile())
                {
                    var query = from u in db.User
                                orderby u.id ascending
                                select u;

                    return query.ToList();
                }
            }
        }

        public bool AddUser(User user)
        {
            using (var db = new DbWebMobile())
            {
                return db.Insert(user) > 0;
            }
        }

        public bool SaveUser(User user)
        {
            using (var db = new DbWebMobile())
            {
                return db.Update(user) > 0;
            }
        }

        public bool IsExists(int id)
        {
            using (var db = new DbWebMobile())
            {
                var query = from u in db.User
                            where u.id == id
                            select u;

                return (query.Count() > 0);
            }
        }

        public bool IsUserExists(User user)
        {
            using (var db = new DbWebMobile())
            {
                var users = from u in db.User.Take(1)
                                where u.firstName == user.firstName
                                where u.surname == user.surname
                                where u.fatherName == user.fatherName
                                where u.address == user.address
                            select u;

                return (users.Count() > 0);
            }
        }

        public bool IsCorrectInfoFormat(User user)
        {
            if (user.firstName.Length < 3 || user.firstName.Length > 32
                || user.surname.Length < 3 || user.surname.Length > 32
                || user.fatherName.Length < 3 || user.fatherName.Length > 32
                || user.address.Length < 2 || user.address.Length > 256)
            {
                return false;
            }

            return true;
        }
    }
}
