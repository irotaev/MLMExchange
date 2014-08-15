/// <reference path="../typings/jquery/jquery.d.ts" />

/// <amd-dependency path="jquery" />

declare var Ext: any;
$ = require("jquery");

/**
 * Блок регулирования доступа к торговле в зависимости от роли пользователя
 */
export class UserRoleTradeAccessBlock
{
  constructor()
  {
    //#region Set store
    this._Store = new Ext.data.JsonStore({
      storeId: 'Roles.TradeAccessBlock.UserRoles',

      fields: ['Id', 'User', 'RoleType', 'RoleTypeAsString', 'RoleTypeDisplayName', 'TextResources'],
      proxy: {
        type: 'ajax',
        actionMethods: {
          create: 'POST',
          read: 'GET',
          update: 'PUT',
          destroy: 'DELETE'
        },
        writer: {
          type: 'json',
          allowSingle: false
        },
        api: {
          read: '/AdminPanel/BaseUserRole/GetAllRoleTypeAccessLevels'
        },
        callback: () =>
        {
        }
      },
      listeners: {
        load: () => 
        {
        }
      }
    });
    //#endregion
  }

  private _Store: any;
}

Ext.define('Roles.TradeAccessBlock', {
  extend: 'Ext.dataview.DataView',

});

Ext.define('CustomRoleListContainer',
  {
    extend: 'Ext.container.Container',
    layout: 'fit',
    Roles: undefined,
    Button: undefined,
    listeners: {
      beforerender: function ()
      {
        var tpl = new Ext.XTemplate(
          '<div class="b-rl" id="b-rl_ID">',

          '<div id="b-rl__button-place"></div>',

          '<tpl for=".">',
          '<div class="b-rl__role b-rl__role_type_{[values.RoleTypeAsString.toLowerCase()]}" data-id="{Id}">',
          '<div class="b-rl__role-display-name"> {RoleTypeDisplayName} </div>',

          '<div class="b-rl__actions">',
          '<span class="b-rl__action b-rl__action_function_delete"><i class="b-rl__icon fa fa-trash-o"></i>{TextResources.DeleteRole}</span>',
          '</div>',
          ' </div>',
          '</tpl>',

          '</div>'
          );

        this.html = tpl.apply(this.Roles);
      },
      afterrender: function ()
      {
        if (this.Button)
          this.Button.render('b-rl__button-place');
      }
    }
  });