using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyEvernote.BusinessLayer;
using MyEvernote.BusinessLayer.Result;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ValueObjects;
using MyEvernote.Web.Filters;
using MyEvernote.Web.Models;
using MyEvernote.Web.ViewModels;
using PagedList;

namespace MyEvernote.Web.Controllers
{
    [Exc]
    public class HomeController : Controller
    {
        private NoteManager noteManager = new NoteManager();
        private CategoryManager categoryManager = new CategoryManager();
        private EvernoteUserManager evernoteUserManager = new EvernoteUserManager();

        // GET: Home
        public ActionResult Index(int page = 1)
        {


            //category controller üzerinden gelen view talebi ve model
            //if (TempData["categoryNotes"] != null)
            //{
            //    return View(TempData["categoryNotes"] as List<Note>);
            //}




            return View(noteManager.ListQueryable().Where(i => i.IsDraft == false).OrderByDescending(i => i.ModifiedOn)
                .ToPagedList(page, 6)); //c# tarafından sıralama

            //return View(noteManager.GetAllNoteQueryable().OrderByDescending(i => i.ModifiedOn).ToList()); sql tarafından sıralama
        }

        public ActionResult ByCategory(int? id,int page=1)
        {
            if (id == null)
            {
                return
                    new HttpStatusCodeResult(HttpStatusCode
                        .BadRequest); //sayfa bulunamadı gibi bir hata dödürüyor.idsi yoksa kötü bir istek gelmiştir.
            }


            //Category category = categoryManager.Find(i => i.Id == id.Value);
            //if (category == null)
            //{
            //    return HttpNotFound();
            //    //return RedirectToAction("Index", "Home");bu şekilde anasayfaya yönlendirebiliriz
            //}

           

            return View("Index", noteManager.ListQueryable().Where(i => i.IsDraft == false && i.CategoryId == id)
                .OrderByDescending(i => i.ModifiedOn).ToPagedList(page, 6));
        }

        public ActionResult MostLiked(int page=1)
        {
            return View("Index", noteManager.ListQueryable().OrderByDescending(i => i.LikeCount).ToPagedList(page, 6));
        }

        public ActionResult About()
        {
            return View();
        }
        [Auth]
        public ActionResult ShowProfile()
        {
            //kullanıcının bilgisini çekip databaseden sorgulayıp evernote nesnesi olarak sayfaya vermemiz gerekiyor

            BusinessLayerResult<EvernoteUser> result = evernoteUserManager.GetUserById(CurrentSession.User.Id);


            if (result.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObject = new ErrorViewModel()
                {
                    Title = "Hata Oluştu !",
                    Items = result.Errors,
                };

                return View("Error", errorNotifyObject);
            }

            return View(result.Result);
        }
        [Auth]
        public ActionResult EditProfile()
        {
            BusinessLayerResult<EvernoteUser> result = evernoteUserManager.GetUserById(CurrentSession.User.Id);
            if (result.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObject = new ErrorViewModel()
                {
                    Title = "Hata Oluştu !",
                    Items = result.Errors,
                };

                return View("Error", errorNotifyObject);
            }

            return View(result.Result);

        }
        [Auth]
        [HttpPost]
        public ActionResult EditProfile(EvernoteUser model, HttpPostedFileBase ProfileImage)
        {
            ModelState.Remove("ModifiedUsername");//bunu validation kontrolünü yapma çünkü biz zaten bunu veriyoruz


            if (ModelState.IsValid)
            {
                if (ProfileImage != null && (ProfileImage.ContentType == "image/jpeg" || ProfileImage.ContentType == "image/jpg" || ProfileImage.ContentType == "image/png"))
                {
                    string filename = $"user_{model.Id}.{ProfileImage.ContentType.Split('/')[1]}";
                    ProfileImage.SaveAs(Server.MapPath($"~/images/{filename}"));
                    model.ProfileImageFileName = filename;
                }

                BusinessLayerResult<EvernoteUser> result = evernoteUserManager.UpdateProfile(model);
                if (result.Errors.Count > 0)
                {
                    ErrorViewModel errorNotifyObject = new ErrorViewModel()
                    {
                        Items = result.Errors,
                        Title = "Profil Güncellenemedi.",
                        RedirectingUrl = "EditProfile"
                    };

                    return View("Error", errorNotifyObject);
                }
                //profil güncellendiği için session da güncellendi çünkü eskimiş oluyor
                CurrentSession.Set<EvernoteUser>("login", result.Result);
                return RedirectToAction("ShowProfile");
            }

            return View(model);
        }
        [Auth]
        public ActionResult DeleteProfile()
        {
            BusinessLayerResult<EvernoteUser> result = evernoteUserManager.RemoveUserById(CurrentSession.User.Id);
            if (result.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObject = new ErrorViewModel()
                {
                    Items = result.Errors,
                    Title = "Profil Silinemedi.",
                    RedirectingUrl = "ShowProfile"
                };
                return View("Error", errorNotifyObject);
            }

            Session.Clear();

            return RedirectToAction("Index");
        }

        public ActionResult TestNotify()
        {
            ErrorViewModel model = new ErrorViewModel()
            {
                Header = "Yönlendirme...",
                Title = "Ok Test",
                RedirectingTimeout = 10000,
                Items = new List<ErrorMessageObject>()
                {
                    new ErrorMessageObject()
                    {
                        Message = "Test Başarılı"
                    },
                    new ErrorMessageObject()
                    {
                        Message = "Test Başarılı 2"
                    }
                }
            };

            return View("Error", model);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)//model doğruysa
            {
                BusinessLayerResult<EvernoteUser> result = evernoteUserManager.LoginUser(model);

                if (result.Errors.Count > 0)
                {
                    result.Errors.ForEach(i => ModelState.AddModelError("", i.Message));

                    if (result.Errors.Find(i => i.Code == ErrorMessageCode.UserIsNotActive) != null)
                    {

                        ViewBag.SetLink = "E-Posta Gönder";
                    }

                    return View(model);
                }

                CurrentSession.Set<EvernoteUser>("login", result.Result);//sessiona bilgi saklama
                return RedirectToAction("Index");//yönlendirme

            }

            return View();
        }

        public ActionResult Register()
        {


            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {


            if (ModelState.IsValid) //modelin kuralları geçerli mi?
            {

                BusinessLayerResult<EvernoteUser> result = evernoteUserManager.RegisterUser(model);

                if (result.Errors.Count > 0)
                {
                    result.Errors.ForEach(i => ModelState.AddModelError("", i.Message));
                    return View(model);
                }

                //EvernoteUser user = null;
                //try
                //{
                //    user = evernoteUserManager.RegisterUser(model);
                //}
                //catch (Exception ex)
                //{
                //    ModelState.AddModelError("", ex.Message);

                //}


                //if (model.Username == "eberge")
                //{
                //    ModelState.AddModelError("", "Kullanıcı adı kullanılıyor.");
                //}

                //if (model.Email == "enisberge@gmail.com")
                //{

                //    ModelState.AddModelError("", "E-posta adresi kullanılıyor.");
                //}

                //foreach (var item in ModelState)
                //{
                //    if (item.Value.Errors.Count > 0)
                //    {
                //        return View(model);
                //    }



                //if (User == null)
                //{
                //    return View(model);
                //}
                OkViewModel notifyObject = new OkViewModel()
                {
                    Title = "Kayıt Başarılı",
                    RedirectingUrl = "/Home/Login",
                };
                notifyObject.Items.Add("Lütfen e-posta adresinize gönderdiğimiz aktivasyon linkine tıklayarak hesabınızı aktive ediniz. Hesabınızı aktive etmeden yorum ve beğeni yapamazsınız.");
                return View("Ok", notifyObject); //hata yoksa buraya kayıt başarılı olarak gidecek
            }

            return View(model);
        }
        public ActionResult UserActivate(Guid id)
        {
            BusinessLayerResult<EvernoteUser> result = evernoteUserManager.ActivateUser(id);
            if (result.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObject = new ErrorViewModel()
                {
                    Title = "Geçersiz İşlem",
                    Items = result.Errors,
                };
                return View("Error", errorNotifyObject);
            }
            OkViewModel okNotifyObject = new OkViewModel()
            {
                Title = "Hesap Aktifleştirildi",
                RedirectingUrl = "/Home/Login",
            };
            okNotifyObject.Items.Add("Hesabınız aktifleştirildi. Artık yorum ve beğeni yapabilirsiniz.");
            return View("Ok", okNotifyObject);
        }

        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Index");
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult HasError()
        {
            return View();
        }

        public ActionResult Page(int? id)
        {
            Note note = noteManager.Find(i => i.Id == id);
            return View(note);
        }
    }
}
