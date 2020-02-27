using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Common.Helpers
{
    public class ConfigHelper//tip dönüşümü yaparak bize bir şeyler getirecek class
    {
        public static T Get<T>(string key)
        {
            //burada cast işlemi yapılacak port numarası istersek int vereceğimiz için int tipinde değer döndürecek string istediğimizde string
            return (T)Convert.ChangeType(ConfigurationManager.AppSettings[key], typeof(T));
        }

    }
}
