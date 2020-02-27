using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyEvernote.BusinessLayer.Abstract;
using MyEvernote.DataAccessLayer.EntityFramework;
using MyEvernote.Entities;

namespace MyEvernote.BusinessLayer
{
    public class CategoryManager : ManagerBase<Category>
    {
        //public override int Delete(Category category)
        //{
        //    NoteManager noteManager = new NoteManager();
        //    LikedManager likedManager = new LikedManager();
        //    CommentManager commentManager = new CommentManager();
        //    //kategori ile ilişkili notların silinmesi gerekiyor.hatta like ve yorumlar da silinebilmeli.
        //    foreach (Note note in category.Notes.ToList())
        //    {
        //        //note ile ilişkili likeları silelim
        //        foreach (Liked like in note.Likes.ToList())
        //        {
        //            likedManager.Delete(like);
        //        }
        //        //note ile ilişkili commentlerin silinmesi

        //        foreach (Comment comment in note.Comments.ToList())
        //        {
        //            commentManager.Delete(comment);
        //        }

        //        noteManager.Delete(note);
        //    }


        //    return base.Delete(category);
        //}
    }
}
