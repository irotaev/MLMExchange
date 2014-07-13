using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;

///<summary>
/// Расширение пространства имен NHibernate
///</summary>
namespace NHibernate
{
  public class RandomOrder : Order
  {
    public RandomOrder()
      : base(String.Empty, true)
    { }

    public override SqlCommand.SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
    {
      return new SqlCommand.SqlString("NEWID()");
    }
  }

  #region Extension mehod
  /// <summary>
  /// Расширения для библиотеки NHibernate
  /// </summary>
  public static class NhibernateExtansion
  {
    public static IQueryOver<TRoot, TSubType> OrderByRandom<TRoot, TSubType>(this IQueryOver<TRoot, TSubType> query)
    {
      query.UnderlyingCriteria.AddOrder(new RandomOrder());

      return query;
    }
  }
  #endregion
}
