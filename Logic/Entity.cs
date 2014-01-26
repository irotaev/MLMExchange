using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace Logic
{
  #region Entity
  public class User
  {
    public virtual long Id { get; set; }
    public virtual string Login { get; set; }
    public virtual string PasswordHash { get; set; }
  }
  #endregion

  #region Map Entity
  class User_Map : ClassMap<User>
  {
    public User_Map()
    {
      Id(x => x.Id).GeneratedBy.Increment();

      Map(x => x.Login).Not.Nullable().Length(100);

      Map(x => x.PasswordHash).Not.Nullable();
    }
  }
  #endregion
}
