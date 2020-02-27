namespace MyEvernote.DataAccessLayer.EntityFramework
{
    //SİNGLETON PATTERN
    public class RepositoryBase
    {
        protected static DatabaseContext _dbContext;//burada newlemiyoruz
        private static object _lockSync = new object();

        protected RepositoryBase()//classın newlenmemesi için protected yapıyoruz
        {
            CreateContext();
        }

        private static void CreateContext()
        {
            if (_dbContext == null)//dbmiz nullsa
            {
                //multithread uygulamalarda aynı anda iki parçacık girip newleme yapabiliyor. yapmasın diye lock blogunu kullanabiliriz
                lock (_lockSync)
                {
                    if (_dbContext == null)
                    {
                        _dbContext = new DatabaseContext();
                    }

                }


            }
        }
    }
}
