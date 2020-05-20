using LinqToDB;
using LinqToDB.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMobile.Data.Models;

namespace WebMobile.Data
{
    public class ConnectionStringSettings : IConnectionStringSettings
    {
        public string ConnectionString { get; set; }
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public bool IsGlobal => false;
    }

    public class DBConnect : ILinqToDBSettings
    {
        private string connectionString;

        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

        public string DefaultConfiguration => "SqlServer";
        public string DefaultDataProvider => "SqlServer";

        public DBConnect(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                yield return
                    new ConnectionStringSettings
                    {
                        Name = "WebMobile",
                        ProviderName = "SqlServer",
                        ConnectionString = this.connectionString
                    };
            }
        }
    }

    public class DbWebMobile : LinqToDB.Data.DataConnection
    {
        public DbWebMobile() : base("WebMobile") { }

        public ITable<Phone> Phone => GetTable<Phone>();
        public ITable<User> User => GetTable<User>();
    }
}
