using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using NHibernate.Event;

namespace Logic
{
  #region Entity
  public abstract class BaseObject
  {
    public virtual long Id { get; set; }
    public virtual DateTime CreationDateTime { get; set; }
  }

  public class User : BaseObject
  {
    public virtual string Login { get; set; }
    public virtual string PasswordHash { get; set; }
    public virtual string Name { get; set; }
    public virtual string Surname { get; set; }
    public virtual string Patronymic { get; set; }
    public virtual string Email { get; set; }
    public virtual string PhotoRelativePath { get; set; }
  }

  public class AddMyCryptTransaction : BaseObject
  {
    /// <summary>
    /// Пополняемое количество MyCrypt
    /// </summary>
    public virtual int MyCryptCount { get; set; }
    public virtual string Comment { get; set; }
    public virtual User User { get; set; }
    /// <summary>
    /// Относительный путь к подтверждающей картинке
    /// </summary>
    public virtual string ImageRelativePath { get; set; }
  }
  #endregion

  #region Map Entity
  abstract public class BaseObject_Map<TObject> : ClassMap<TObject> where TObject : BaseObject
  {
    public BaseObject_Map()
    {
      Id(x => x.Id).GeneratedBy.Increment();

      Map(x => x.CreationDateTime).Not.Nullable();
    }
  }

  public class User_Map : BaseObject_Map<User>
  {
    public User_Map()
    {      
      Map(x => x.Login).Not.Nullable().Length(100);
      Map(x => x.PasswordHash).Not.Nullable().Length(200);
      Map(x => x.Name).Nullable().Length(100);
      Map(x => x.Surname).Nullable().Length(100);
      Map(x => x.Patronymic).Nullable().Length(100);
      Map(x => x.Email).Nullable().Length(100);
      Map(x => x.PhotoRelativePath).Nullable().Length(200);
    }
  }

  public class AddMyCryptTransaction_Map : BaseObject_Map<AddMyCryptTransaction>
  {
    public AddMyCryptTransaction_Map()
    {
      Map(x => x.MyCryptCount).Not.Nullable();
      Map(x => x.Comment).Nullable().Length(5000);
      References(x => x.User).Column("UserId");
      Map(x => x.ImageRelativePath).Nullable().Length(1000);
    }
  }
  #endregion

  #region Event listeners
  public class PreInsertEvent : IPreInsertEventListener
  {
    public bool OnPreInsert(NHibernate.Event.PreInsertEvent @event)
    {
      BaseObject baseObject = (@event.Entity as BaseObject);

      if (baseObject == null)
        return false;

      #region Задаю время создания
      int createdDateTimeIndex = Array.IndexOf(@event.Persister.PropertyNames, "CreationDateTime");

      @event.State[createdDateTimeIndex] = DateTime.Now.ToUniversalTime();      
      #endregion

      return false;
    }
  }
  #endregion
}
