﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Entities
{
    [Table("EvernoteUsers")]
    public class EvernoteUser : MyEntityBase
    {
        [DisplayName("Ad"), StringLength(25, ErrorMessage = "{0} alanı max {1} karakter olmalıdır.")]
        public string Name { get; set; }
        [DisplayName("Soyad"), StringLength(25)]
        public string Surname { get; set; }
        [DisplayName("Kullanıcı Adı"), Required(ErrorMessage = "{0} alanı gereklidir."), StringLength(25, ErrorMessage = "{0} alanı max {1} karakter olmalıdır.")]
        public string Username { get; set; }
        [DisplayName("E-posta"), Required(ErrorMessage = "{0} alanı gereklidir."), StringLength(70, ErrorMessage = "{0} alanı max {1} karakter olmalıdır.")]
        public string Email { get; set; }
        [DisplayName("Şifre"), Required(ErrorMessage = "{0} alanı gereklidir."), StringLength(100, ErrorMessage = "{0} alanı max {1} karakter olmalıdır.")]
        public string Password { get; set; }

        //scafoldcolumn otomatik view oluştururken istenmeyen viewların oluşturulmamasını sağlıyor
        [StringLength(30), ScaffoldColumn(false)]// images/user_12.jpg userın idsi gelecek
        public string ProfileImageFileName { get; set; }
        [DisplayName("Is Active")]
        public bool IsActive { get; set; }
        [Required, ScaffoldColumn(false)]
        public Guid ActivateGuid { get; set; }//aktivasyon mailine gidecek olan kod
        [DisplayName("Is Admin")]
        public bool IsAdmin { get; set; }

        public virtual List<Note> Notes { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<Liked> Likeds { get; set; }
    }
}
