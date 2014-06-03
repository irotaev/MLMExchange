using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Lib
{
  /// <summary>
  /// Аттрибут уровня данных
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public class DataConfigAttribute : Attribute
  {
    /// <summary>
    /// Тип прокси-объекта логики
    /// </summary>
    public Type LogicProxyType { get; set; }
  }
}
