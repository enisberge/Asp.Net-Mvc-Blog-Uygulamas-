﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyEvernote.Web.ViewModels
{
    public class NotifyViewModelBase<T>
    {
        public List<T> Items { get; set; }//hata türleri listesi
        public string Header { get; set; }
        public string Title { get; set; }
        public bool IsRedirecting { get; set; }
        public string RedirectingUrl { get; set; }
        public int RedirectingTimeout { get; set; }

        public NotifyViewModelBase()
        {//bunlar default değerler
            Header = "Yönlendiriliyorsunuz...";//default değeri
            Title = "Geçersiz İşlem";
            IsRedirecting = true;
            RedirectingUrl = "/Home/Index";
            RedirectingTimeout = 10000;
            Items = new List<T>();
        }
    }
}