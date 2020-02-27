using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Entities
{
    public class MyEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("Oluşturma Tarihi"), Required]
        public DateTime CreatedOn { get; set; }//oluşturulduğu tarih
        [DisplayName("Güncelleme Tarihi"), Required]
        public DateTime ModifiedOn { get; set; }//düzenlendiği tarih
        [DisplayName("Güncelleyen"), Required, StringLength(30)]
        public string ModifiedUsername { get; set; }//user ile ilişkilendirseydik ileride kullanıcı silersek kullanıcıyla ilişkili olan kategorilerin de silinmemesi için böyle yaptık
    }
}
