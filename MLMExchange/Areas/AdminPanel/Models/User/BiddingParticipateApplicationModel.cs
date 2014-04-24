using DataAnnotationsExtensions;
using Logic;
using MLMExchange.Lib.Exception;
using MLMExchange.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLMExchange.Areas.AdminPanel.Models.User
{
  /// <summary>
  /// Модель заявки на участие в торгах
  /// </summary>
  public class BiddingParticipateApplicationModel : AbstractDataModel, IDataBinding<BiddingParticipateApplication>
  {
    /// <summary>
    /// Количество my-crypt для подачи в заявку
    /// </summary>
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [Integer(ErrorMessageResourceName = "FieldFilledInvalid_IntegerOnly", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public int? MyCryptCount { get; set; }

    public void Bind(BiddingParticipateApplication @object)
    {
      base.Bind(@object);

      MyCryptCount = @object.MyCryptCount;
    }

    public BiddingParticipateApplication UnBind(BiddingParticipateApplication @object = null)
    {
      if (@object == null)
        @object = new BiddingParticipateApplication();

      base.UnBind(@object);

      if (MyCryptCount == null)
        throw new UserVisible__ArgumentNullException("MyCryptCount");

      @object.MyCryptCount = MyCryptCount.Value;
      @object.User = MLMExchange.Lib.CurrentSession.Default.CurrentUser;

      return @object;
    }
  }
}
