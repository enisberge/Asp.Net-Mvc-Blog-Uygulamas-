using System.Web;
using MyEvernote.Common;
using MyEvernote.Entities;
using MyEvernote.Web.Models;

namespace MyEvernote.Web.Initialize
{
    public class WebCommon : ICommon
    {
        public string GetCurrentUsername()
        {
            EvernoteUser user = CurrentSession.User;

            if (user != null)
            {
                return user.Username;
            }


            return "system";
        }
    }
}