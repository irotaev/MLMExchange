using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace Logic
{
  #region Entity
  public abstract class BaseObject
  {
  }

  public class User : BaseObject
  {
    public virtual long Id { get; set; }
    public virtual string Login { get; set; }
    public virtual string PasswordHash { get; set; }
    public virtual string Name { get; set; }
    public virtual string Surname { get; set; }
    public virtual string Patronymic { get; set; }
    public virtual string Email { get; set; }
    public virtual string PhotoRelativePath { get; set; }
  }
  #endregion
  
  #region Map Entity
  public class User_Map : ClassMap<User>
  {
    public User_Map()
    { 
      Id(x => x.Id).GeneratedBy.Increment();

      Map(x => x.Login).Not.Nullable().Length(100);
      Map(x => x.PasswordHash).Not.Nullable().Length(200);
      Map(x => x.Name).Nullable().Length(100);
      Map(x => x.Surname).Nullable().Length(100);
      Map(x => x.Patronymic).Nullable().Length(100);
      Map(x => x.Email).Nullable().Length(100);
      Map(x => x.PhotoRelativePath).Nullable().Length(200);
    }
  }
  #endregion
}
