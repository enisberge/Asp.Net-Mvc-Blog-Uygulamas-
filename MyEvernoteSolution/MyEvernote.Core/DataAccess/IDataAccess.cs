using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MyEvernote.Core.DataAccess
{
    public interface IDataAccess<T>//eski adı IRepository
    {
        List<T> List();

        IQueryable<T> ListQueryable();

        List<T> List(Expression<Func<T, bool>> where);//istediğimiz bir kritere göre liste döndüren method


        int Insert(T obj);//bir nesne göndereceğiz onu insert edicek o da obje

        int Update(T obj);
        int Delete(T obj);

        int Save();//kaç kayıt etkilendiyse onun adedi döner o sebeple int

        T Find(Expression<Func<T, bool>> where);//verilen koşula uygun tek bir tane obje döndürme
    }
}
