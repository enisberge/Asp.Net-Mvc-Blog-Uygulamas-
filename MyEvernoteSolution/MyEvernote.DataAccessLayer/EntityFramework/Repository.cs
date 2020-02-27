using MyEvernote.Common;
using MyEvernote.Core.DataAccess;
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    public class Repository<T> : RepositoryBase, IDataAccess<T> where T : class//t tipi class olmak zorundadır yani newlenen türde olmak zorundadır
    {

        private DbSet<T> _objectDbSet;

        public Repository()
        {

            _objectDbSet = _dbContext.Set<T>();
        }
        public List<T> List()
        {
            return _objectDbSet.ToList();//hangi tip nesne gelirse onu liste çevir döndür
        }
        public IQueryable<T> ListQueryable()
        {
            return _objectDbSet.AsQueryable();//Listeyi orderby şeklinde sql üzerinden sıralayarak çekmek istediğimizde bu methodu kullanabiliriz
        }

        public List<T> List(Expression<Func<T, bool>> where)//istediğimiz bir kritere göre liste döndüren method
        //ör: x=>x.id==3(lambda)
        {
            return _objectDbSet.Where(where).ToList();
        }

        //public IQueryable<T> List(Expression<Func<T, bool>> where)//orderby ile sıralama yapmak istersek bu methodu kullanabiliriz
        //{
        //    return _objectDbSet.Where(where);
        //}

        public int Insert(T obj)//bir nesne göndereceğiz onu insert edicek o da obje
        {
            _objectDbSet.Add(obj);//set burada tabloyu bulmamızı sağlıyor

            if (obj is MyEntityBase)//gelen entities myentitiybaseten türemişse yani createdon modifiedusername v.b. varsa
            {
                MyEntityBase obje = obj as MyEntityBase;//gelen objeyi cast et
                DateTime now = DateTime.Now;


                obje.CreatedOn = now;
                obje.ModifiedOn = now;
                obje.ModifiedUsername = App.Common.GetCurrentUsername();

            }

            return Save();
        }

        public int Update(T obj)
        {
            if (obj is MyEntityBase)//gelen entities myentitiybaseten türemişse yani createdon modifiedusername v.b. varsa
            {
                MyEntityBase obje = obj as MyEntityBase;//gelen objeyi cast et
                DateTime now = DateTime.Now;

                //createdon burada yok insertte oluyor sadece

                obje.ModifiedOn = now;
                obje.ModifiedUsername = App.Common.GetCurrentUsername();//TODO : işlem yapan kullanıcı adı yazılacak

            }

            return Save();
        }
        public int Delete(T obj)
        {
            //if (obj is MyEntityBase)
            //{
            //    MyEntityBase obje = obj as MyEntityBase;
            //    DateTime now = DateTime.Now;

            //    obje.ModifiedOn = now;
            //    obje.ModifiedUsername = App.Common.GetUsername();//TODO : işlem yapan kullanıcı adı yazılacak

            //}
            _objectDbSet.Remove(obj);
            return Save();
        }

        public int Save()//kaç kayıt etkilendiyse onun adedi döner o sebeple int
        {
            return _dbContext.SaveChanges();
        }

        public T Find(Expression<Func<T, bool>> where)//verilen koşula uygun tek bir tane obje döndürme
        {
            return _objectDbSet.FirstOrDefault(where);//firstordefault bulabilirse nesneyi döner bulamazsa null döner
        }
    }
}
