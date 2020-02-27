using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using MyEvernote.BusinessLayer;
using MyEvernote.Entities;

namespace MyEvernote.Web.Models
{
    public class CacheHelper
    {
        //cachten okuma yapan bir statik metoda ihtiyacımız var
        //cache yapısı session gibi bilgileri geçici tutar. böylece tekrar tekrar category bilgisini çekmemiş oluruz.
        public static List<Category> GetCategoriesFromCache()
        {
            var result = WebCache.Get("category-cache");

            if (result == null)
            {
                CategoryManager categoryManager = new CategoryManager();
                result = categoryManager.List();
                WebCache.Set("category-cache", result, 20, true);
            }

            return result;
        }

        public static void RemoveCategoriesFromCashe()
        {
            Remove("category-cache");
        }

        public static void Remove(string key)
        {
            WebCache.Remove(key);
        }

    }
}