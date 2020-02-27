using System.Collections.Generic;
using MyEvernote.Entities.Messages;

namespace MyEvernote.BusinessLayer.Result
{
    public class BusinessLayerResult<T> where T : class
    {
        public List<ErrorMessageObject> Errors { get; set; }//hata mesajlarını saklayan list.keyvaluepair ile 2 tane liste alan değer haline getirdik.
        public T Result { get; set; }//bu classı oluştururken verdiğimiz tip yani result.işlem başarılıysa burada yer alacak.

        public BusinessLayerResult()
        {
            Errors = new List<ErrorMessageObject>();
        }

        public void AddError(ErrorMessageCode code, string message)
        {
            Errors.Add(new ErrorMessageObject()
            {
                Code = code,
                Message = message
            });
        }
    }
}
