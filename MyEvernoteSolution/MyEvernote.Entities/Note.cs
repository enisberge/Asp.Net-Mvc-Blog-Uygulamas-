using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MyEvernote.Entities
{
    [Table("Notes")]
    public class Note : MyEntityBase
    {
        [DisplayName("Not Başlığı"), Required, StringLength(60)]
        public string Title { get; set; }
        [DisplayName("Not Metni"), Required, MaxLength, AllowHtml]
        public string Text { get; set; }
        [DisplayName("Konu Resmi"),StringLength(30)]// images/user_12.jpg userın idsi gelecek
        public string NoteImageFileName { get; set; }
        [DisplayName("Taslak")]
        public bool IsDraft { get; set; }//taslak halinde mi?
        [DisplayName("Beğenilme")]
        public int LikeCount { get; set; }//beğeni sayısı
        [DisplayName("Kategori")]
        public int CategoryId { get; set; }

        public virtual EvernoteUser Owner { get; set; }
        public virtual Category Category { get; set; }

        public virtual List<Comment> Comments { get; set; }
        public virtual List<Liked> Likes { get; set; }

        public Note()
        {
            Comments = new List<Comment>();
            Likes = new List<Liked>();
        }
    }
}
