using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using NHibernate.Linq;
using MLMExchange.Lib.Exception;
using NHibernate;

namespace MLMExchange.Lib.EntityLogic
{
  public class Logic__RandomWords  : AbstractEntityLogic
  {

    public static D_RandomWords GetRandomWord()
    {
      D_RandomWords randomwords;

      randomwords = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .QueryOver<D_RandomWords>().OrderByRandom().List().FirstOrDefault();

      return randomwords;
    }
  }
}