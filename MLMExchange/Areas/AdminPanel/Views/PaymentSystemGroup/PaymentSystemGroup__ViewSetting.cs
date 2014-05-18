using MLMExchange.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLMExchange.Areas.AdminPanel.Views.PaymentSystemGroup
{
  /// <summary>
  /// Настройка для вьюшек объекта PaymentSystemGroup
  /// </summary>
  public class PaymentSystemGroup_Browse__ViewSetting : AbstarctViewSetting
  {
    /// <summary>
    /// Является ли пользователь хозяеном данной группы
    /// </summary>
    public bool IsRootUser { get; set; }
    /// <summary>
    /// Разрешено ли добавление платежной системы
    /// </summary>
    public bool IsEnableAddPaymentSystem { get; set; }
    /// <summary>
    /// Показывать/непоказывать хедер
    /// </summary>
    public bool IsHeaderEnable { get; set; }
    /// <summary>
    /// Разрешены ли root операции для платежной системы
    /// </summary>
    public bool IsPaymentSystemOperationRootOperationEnable { get; set; }
    /// <summary>
    /// Позволить ли оплатить коммиссионный сбор для продавца
    /// </summary>
    public bool IsRequireSallerInterestRatePayment { get; set; }
    /// <summary>
    /// Позволить ли оплатить платеж доходности торговой сессии
    /// </summary>
    public bool IsRequiredYieldTradingSessionPayment { get; set; }
  }
}