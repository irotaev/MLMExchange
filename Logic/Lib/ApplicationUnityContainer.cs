using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Logic.Lib
{
  /// <summary>
  /// Оболочка над UnityContainer
  /// </summary>
  public class ApplicationUnityContainer
  {
    static ApplicationUnityContainer()
    {
      UnityContainer = new UnityContainer();
    }

    public static UnityContainer UnityContainer { get; private set; }
  }
}
