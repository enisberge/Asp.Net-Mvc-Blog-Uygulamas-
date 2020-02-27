using MyEvernote.BusinessLayer;
using MyEvernote.Entities;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using MyEvernote.Web.Filters;
using MyEvernote.Web.Models;

namespace MyEvernote.Web.Controllers
{
    [Exc]
    [Auth]
    [AuthAdmin]
    public class CategoryController : Controller
    {

        CategoryManager categoryManager = new CategoryManager();

        public ActionResult Index()
        {
            return View(categoryManager.List());
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = categoryManager.Find(i => i.Id == id.Value);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                categoryManager.Insert(category);
                CacheHelper.RemoveCategoriesFromCashe();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = categoryManager.Find(i => i.Id == id.Value);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category _category)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                Category category = categoryManager.Find(i => i.Id == _category.Id);
                category.Title = _category.Title;
                category.Description = _category.Description;

                categoryManager.Update(category);
                CacheHelper.RemoveCategoriesFromCashe();
                return RedirectToAction("Index");
            }
            return View(_category);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = categoryManager.Find(i => i.Id == id.Value);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = categoryManager.Find(i => i.Id == id);
            categoryManager.Delete(category);
            CacheHelper.RemoveCategoriesFromCashe();
            return RedirectToAction("Index");
        }

    }
}
