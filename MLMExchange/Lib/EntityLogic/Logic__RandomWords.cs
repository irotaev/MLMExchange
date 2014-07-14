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

    public static D_RandomWord GetRandomWord()
    {
      D_RandomWord randomwords;

      randomwords = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .QueryOver<D_RandomWord>().OrderByRandom().List().FirstOrDefault();

      if (randomwords == null)
      {
        randomwords = new D_RandomWord();
        randomwords.Author = Logic.Properties.GeneralResources.AuthorWords;
        randomwords.Text = Logic.Properties.GeneralResources.TextWords;
      }

      return randomwords;
    }
  }
}