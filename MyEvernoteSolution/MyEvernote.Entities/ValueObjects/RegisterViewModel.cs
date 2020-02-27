using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyEvernote.Entities.ValueObjects
{
    public class RegisterViewModel
    {
        [DisplayName("Kullanıcı Adı"), Required(ErrorMessage = "{0} alanı boş geçilemez !"), StringLength(25, ErrorMessage = "{0} max. {1} karakter olmalı !")]
        public string Username { get; set; }
        [DisplayName("E-posta"), Required(ErrorMessage = "{0} alanı boş geçilemez !"), StringLength(70, ErrorMessage = "{0} max. {1} karakter olmalı !"), EmailAddress(ErrorMessage = "Lütfen {0} alanı için geçerli bir e-posta adresi giriniz !")]
        public string Email { get; set; }
        [DisplayName("Şifre"), Required(ErrorMessage = "{0} alanı boş geçilemez !"), StringLength(25, ErrorMessage = "{0} max. {1} karakter olmalı !")]
        public string Password { get; set; }
        [DisplayName("Şifre Tekrar"), Required(ErrorMessage = "{0} alanı boş geçilemez !"), StringLength(25, ErrorMessage = "{0} max. {1} karakter olmalı !"), Compare("Password", ErrorMessage = "{0} ile {1} eşleşmiyor !")]
        public string RePassword { get; set; }
    }
}