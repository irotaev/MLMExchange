using Logic;
using MLMExchange.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLMExchange.Areas.AdminPanel.Models
{
  public class RoleTypeAccessLevelModel : AbstractDataModel<RoleTypeAccessLevel, D_RoleTypeAccessLevel, RoleTypeAccessLevelModel>
  {
    #region Role type
    public RoleType RoleType { get; set; }
    public string RoleTypeDisplayName
    {
      get
      {
        return _Object.LogicObject.RoleType.GetDisplayName();
      }
    }
    #endregion

    public bool? IsTradeEnabled { get; set; }

    public override RoleTypeAccessLevelModel Bind(RoleTypeAccessLevel @object)
    {
      base.Bind(@object);

      RoleType = @object.LogicObject.RoleType;
      IsTradeEnabled = @object.LogicObject.IsTradeEnable;

      return this;
    }

    public override RoleTypeAccessLevel UnBind(RoleTypeAccessLevel @object = null)
    {
      base.UnBind(@object);

      _Object.LogicObject.RoleType = @object.LogicObject.RoleType;
      _Object.LogicObject.IsTradeEnable = @object.LogicObject.IsTradeEnable;

      return _Object;
    }
  }
}