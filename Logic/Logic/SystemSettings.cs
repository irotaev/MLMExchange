using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NHibernate.Linq;

namespace Logic
{
  /// <summary>
  /// Proxy-объект логики системных настроик
  /// </summary>
  public class SystemSettings : AbstractLogicObject<D_SystemSettings>
  {
    public SystemSettings(D_SystemSettings d_systemSettings) : base(d_systemSettings) { }

    public static explicit operator SystemSettings(D_SystemSettings dataSystemSettings)
    {
      return new SystemSettings(dataSystemSettings);
    }

    /// <summary>
    /// Получить текущие настроки системы. 
    /// На данный момент
    /// </summary>
    /// <returns>Текущие настройки системы</returns>
    public static SystemSettings GetCurrentSestemSettings()
    {
      D_SystemSettings d_lastSystemSettings = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .QueryOver<D_SystemSettings>().OrderBy(x => x.CreationDateTime).Desc.List().FirstOrDefault();

      if (d_lastSystemSettings != null)
        return (SystemSettings)d_lastSystemSettings;
      else
        return null;
    }
  }
}
