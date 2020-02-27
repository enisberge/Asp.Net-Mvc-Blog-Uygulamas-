using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyEvernote.BusinessLayer.Abstract;
using MyEvernote.BusinessLayer.Result;
using MyEvernote.Common.Helpers;
using MyEvernote.DataAccessLayer.EntityFramework;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ValueObjects;

namespace MyEvernote.BusinessLayer
{
    public class EvernoteUserManager : ManagerBase<EvernoteUser>
    {

        public BusinessLayerResult<EvernoteUser> RegisterUser(RegisterViewModel data)
        {
            EvernoteUser user = Find(i => i.Username == data.Username || i.Email == data.Email);
            BusinessLayerResult<EvernoteUser> layerResult = new BusinessLayerResult<EvernoteUser>();

            if (user != null)
            {
                if (user.Username == data.Username)
                {
                    layerResult.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı Adı zaten kayıtlı !");
                }

                if (user.Email == data.Email)
                {
                    layerResult.AddError(ErrorMessageCode.EmailAlreadyExist, "E-posta hesabı zaten kayıtlı !");
                }
            }
            else
            {
                int dbResult = base.Insert(new EvernoteUser()
                {
                    Username = data.Username,
                    Email = data.Email,
                    ProfileImageFileName = "user.png",
                    Password = data.Password,
                    ActivateGuid = Guid.NewGuid(),
                    IsActive = false,
                    IsAdmin = false,
                });
                if (dbResult > 0)
                {
                    layerResult.Result = Find(i => i.Email == data.Email && i.Username == data.Username);

                    string siteUri = ConfigHelper.Get<string>("SiteRootUri");
                    string activateUri = $"{siteUri}/Home/UserActivate/{layerResult.Result.ActivateGuid}";
                    string body =
                        $"Merhaba {layerResult.Result.Username},<br>Hesabınızı aktifleştirmek için <a href='{activateUri}' target='_blank'>tıklayınız</a>.";
                    MailHelper.SendMail(body, layerResult.Result.Email, "MyEvernote Hesap Aktifleştirme");
                }
            }

            return layerResult;
        }

        public BusinessLayerResult<EvernoteUser> LoginUser(LoginViewModel data)
        {
            //giriş kontrolü
            //hesap aktive edilmiş mi?

            BusinessLayerResult<EvernoteUser> layerResult = new BusinessLayerResult<EvernoteUser>();
            layerResult.Result = Find(i => i.Username == data.Username && i.Password == data.Password);


            if (layerResult.Result != null)
            {
                if (!layerResult.Result.IsActive)
                {
                    layerResult.AddError(ErrorMessageCode.UserIsNotActive, "Kullanıcı Hesabı henüz aktif edilmemiş !");
                    layerResult.AddError(ErrorMessageCode.CheckYourEmail, "Lütfen E-posta adresinizi kontrol ediniz !");
                }

            }
            else
            {
                layerResult.AddError(ErrorMessageCode.UsernameOrPassWrong, "Kullanıcı Adı ya da Şifre uyuşmuyor !");
            }

            return layerResult;

        }

        public BusinessLayerResult<EvernoteUser> ActivateUser(Guid activateId)
        {
            BusinessLayerResult<EvernoteUser> layerResult = new BusinessLayerResult<EvernoteUser>();
            layerResult.Result = Find(i => i.ActivateGuid == activateId);
            if (layerResult.Result != null)
            {
                if (layerResult.Result.IsActive)
                {
                    layerResult.AddError(ErrorMessageCode.UserAlreadyActive, "Kullanıcı hesabı zaten aktif edilmiştir !");
                    return layerResult;
                }

                layerResult.Result.IsActive = true;
                Update(layerResult.Result);
            }
            else
            {
                layerResult.AddError(ErrorMessageCode.ActivateIdDoesNotExist, "Aktifleştirilecek kullanıcı bulunamadı !");
            }

            return layerResult;
        }


        public BusinessLayerResult<EvernoteUser> GetUserById(int currentUserId)
        {
            BusinessLayerResult<EvernoteUser> result = new BusinessLayerResult<EvernoteUser>();

            result.Result = Find(x => x.Id == currentUserId);

            if (result.Result == null)
            {
                result.AddError(ErrorMessageCode.UserNotFound, "Kullanıcı Bulunamadı !");
            }

            return result;
        }

        public BusinessLayerResult<EvernoteUser> UpdateProfile(EvernoteUser model)
        {
            EvernoteUser evernoteUser =
                Find(i => i.Username == model.Username || i.Email == model.Email);
            BusinessLayerResult<EvernoteUser> result = new BusinessLayerResult<EvernoteUser>();

            if (evernoteUser != null && evernoteUser.Id != model.Id)
            {
                if (evernoteUser.Username == model.Username)
                {
                    result.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı Adı kayıtlı !");
                }
                if (evernoteUser.Email == model.Email)
                {
                    result.AddError(ErrorMessageCode.EmailAlreadyExist, "E-posta adresi kayıtlı !");
                }

                return result;
            }

            result.Result = Find(i => i.Id == model.Id);
            result.Result.Email = model.Email;
            result.Result.Name = model.Name;
            result.Result.Surname = model.Surname;
            result.Result.Password = model.Password;
            result.Result.Username = model.Username;
            if (string.IsNullOrEmpty(model.ProfileImageFileName) == false)
            {
                result.Result.ProfileImageFileName = model.ProfileImageFileName;
            }

            if (base.Update(result.Result) == 0)
            {
                result.AddError(ErrorMessageCode.ProfileCouldNotUpdate, "Profil Güncellenemedi !");
            }

            return result;
        }

        public BusinessLayerResult<EvernoteUser> RemoveUserById(int Id)
        {
            NoteManager noteManager = new NoteManager();
            CommentManager commentManager = new CommentManager();
            LikedManager likedManager = new LikedManager();

            BusinessLayerResult<EvernoteUser> result = new BusinessLayerResult<EvernoteUser>();

            List<Note> note = noteManager.List(i => i.Owner.Id == Id);
            List<Comment> comment = commentManager.List(i => i.Owner.Id == Id);
            List<Liked> liked = likedManager.List(i => i.LikedUser.Id == Id);

           
            foreach (Comment itemComment in comment.ToList())
            {
                commentManager.Delete(itemComment);
            }

            foreach (Liked itemLiked in liked.ToList())
            {
                likedManager.Delete(itemLiked);
            }

            foreach (Note itemNote in note.ToList())
            {


                foreach (Liked noteLiked in itemNote.Likes.ToList())
                {
                    likedManager.Delete(noteLiked);
                }

                foreach (Comment noteComment in itemNote.Comments.ToList())
                {
                    commentManager.Delete(noteComment);
                }

                noteManager.Delete(itemNote);
            }

            EvernoteUser user = Find(i => i.Id == Id);
            if (user != null)
            {
                if (Delete(user) == 0)
                {
                    result.AddError(ErrorMessageCode.UserCouldNotRemove, "Kullanıcı Silinemedi !");
                    return result;
                }
            }
            else
            {
                result.AddError(ErrorMessageCode.UserCouldNotFind, "Kullanıcı Bulunamadı !");
            }

            return result;
        }

        public new BusinessLayerResult<EvernoteUser> Insert(EvernoteUser data)//newleyerek managerbaseten gelen metodu ezdik.method hiding deniyor buna
        {
            EvernoteUser user = Find(i => i.Username == data.Username || i.Email == data.Email);
            BusinessLayerResult<EvernoteUser> result = new BusinessLayerResult<EvernoteUser>();
            result.Result = data;

            if (user != null)
            {
                if (user.Username == data.Username)
                {
                    result.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı Adı zaten kayıtlı !");
                }

                if (user.Email == data.Email)
                {
                    result.AddError(ErrorMessageCode.EmailAlreadyExist, "E-posta hesabı zaten kayıtlı !");
                }
            }
            else
            {
                result.Result.ProfileImageFileName = "user.png";
                result.Result.ActivateGuid = Guid.NewGuid();

                if (base.Insert(result.Result) == 0)
                {
                    result.AddError(ErrorMessageCode.UserCouldNotInserted, "Kullanıcı Eklenemedi !");
                }

            }

            return result;

        }

        public new BusinessLayerResult<EvernoteUser> Update(EvernoteUser data)
        {
            EvernoteUser evernoteUser =
                Find(i => i.Username == data.Username || i.Email == data.Email);
            BusinessLayerResult<EvernoteUser> result = new BusinessLayerResult<EvernoteUser>();
            result.Result = data;
            if (evernoteUser != null && evernoteUser.Id != data.Id)
            {
                if (evernoteUser.Username == data.Username)
                {
                    result.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı Adı kayıtlı !");
                }
                if (evernoteUser.Email == data.Email)
                {
                    result.AddError(ErrorMessageCode.EmailAlreadyExist, "E-posta adresi kayıtlı !");
                }

                return result;
            }

            result.Result = Find(i => i.Id == data.Id);
            result.Result.Email = data.Email;
            result.Result.Name = data.Name;
            result.Result.Surname = data.Surname;
            result.Result.Password = data.Password;
            result.Result.Username = data.Username;
            result.Result.IsActive = data.IsActive;
            result.Result.IsAdmin = data.IsAdmin;

            if (base.Update(result.Result) == 0)
            {
                result.AddError(ErrorMessageCode.UserCouldNotUpdated, "Kullanıcı Güncellenemedi !");
            }

            return result;
        }
    }
}
