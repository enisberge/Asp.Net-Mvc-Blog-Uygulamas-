using System.Collections.Generic;
using MyEvernote.Entities;
using MyEvernote.Web.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyEvernote.BusinessLayer;
using MyEvernote.Web.Filters;

namespace MyEvernote.Web.Controllers
{
    [Exc]
    public class NoteController : Controller
    {
        NoteManager noteManager = new NoteManager();
        CategoryManager categoryManager = new CategoryManager();
        LikedManager likedManager = new LikedManager();

        [Auth]
        public ActionResult Index()
        {//note tablosuna select atıyoruz. categorye join atıyoruz include ile
            var notes = noteManager.ListQueryable().Include("Category").Include("Owner")
                .Where(i => i.Owner.Id == CurrentSession.User.Id).OrderByDescending(i => i.ModifiedOn);
            return View(notes.ToList());
        }
        [Auth]
        public ActionResult MyLikedNotes()
        {
            var notes = likedManager.ListQueryable()
                .Include("LikedUser").Include("Note")
                .Where(i => i.LikedUser.Id == CurrentSession.User.Id)
                .Select(i => i.Note)
                .Include("Category")
                .Include("Owner")
                 .OrderByDescending(i => i.ModifiedOn);


            return View("Index", notes.ToList());

        }

        [Auth]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Note note = noteManager.Find(i => i.Id == id.Value);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }
        [Auth]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title");
            return View();
        }
        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Note note, HttpPostedFileBase NoteImage)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");
            if (ModelState.IsValid)
            {
                note.Owner = CurrentSession.User;

                if (NoteImage != null && (NoteImage.ContentType == "image/jpeg" || NoteImage.ContentType == "image/jpg" || NoteImage.ContentType == "image/png"))
                {

                    string filename = $"{note.Id}.{NoteImage.ContentType.Split('/')[1]}";
                    NoteImage.SaveAs(Server.MapPath($"~/images/note/{filename}"));
                    note.NoteImageFileName = filename;
                }
                noteManager.Insert(note);
                return RedirectToAction("Index");

            }
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);
            return View(note);
        }
        [Auth]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = noteManager.Find(i => i.Id == id.Value);
            if (note == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);
            return View(note);
        }
        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Note note, HttpPostedFileBase NoteImage)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");


            if (ModelState.IsValid)
            {
                Note dbNote = noteManager.Find(i => i.Id == note.Id);
                dbNote.IsDraft = note.IsDraft;
                dbNote.CategoryId = note.CategoryId;
                dbNote.Text = note.Text;
                dbNote.Title = note.Title;


                if (NoteImage != null && (NoteImage.ContentType == "image/jpeg" || NoteImage.ContentType == "image/jpg" || NoteImage.ContentType == "image/png"))
                {

                    string filename = $"{note.Id}.{NoteImage.ContentType.Split('/')[1]}";
                    NoteImage.SaveAs(Server.MapPath($"~/images/note/{filename}"));
                    dbNote.NoteImageFileName = filename;
                }

                noteManager.Update(dbNote);

                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);
            return View(note);
        }
        [Auth]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Note note = noteManager.Find(i => i.Id == id.Value);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }
        [Auth]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Note note = noteManager.Find(i => i.Id == id);
            noteManager.Delete(note);

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult GetLiked(int[] ids)
        {
            if (CurrentSession.User != null)
            {
                List<int> likedNoteIds = likedManager
                    .List(i => i.LikedUser.Id == CurrentSession.User.Id && ids.Contains(i.Note.Id)).Select(i => i.Note.Id)
                    .ToList();
                return Json(new { result = likedNoteIds });
            }
            else
            {
                return Json(new { result = new List<int>() });
            }


        }

        [HttpPost]
        public ActionResult SetLikeState(int noteid, bool liked)
        {
            int res = 0;

            if (CurrentSession.User == null)
                return Json(new { hasError = true, errorMessage = "Beğenme işlemi için giriş yapmalısınız.", result = 0 });

            Liked like =
                likedManager.Find(x => x.Note.Id == noteid && x.LikedUser.Id == CurrentSession.User.Id);

            Note note = noteManager.Find(x => x.Id == noteid);

            if (like != null && liked == false)
            {
                res = likedManager.Delete(like);
            }
            else if (like == null && liked == true)
            {
                res = likedManager.Insert(new Liked()
                {
                    LikedUser = CurrentSession.User,
                    Note = note
                });
            }

            if (res > 0)
            {
                if (liked)
                {
                    note.LikeCount++;
                }
                else
                {
                    note.LikeCount--;
                }

                res = noteManager.Update(note);

                return Json(new { hasError = false, errorMessage = string.Empty, result = note.LikeCount });
            }

            return Json(new { hasError = true, errorMessage = "Beğenme işlemi gerçekleştirilemedi.", result = note.LikeCount });
        }

        public ActionResult GetNoteText(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Note note = noteManager.Find(i => i.Id == id.Value);
            if (note == null)
            {
                return HttpNotFound();
            }

            return PartialView("_PartialNoteText", note);
        }
    }
}
