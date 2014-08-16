/// <reference path="../typings/jquery/jquery.d.ts" />

/// <amd-dependency path="jquery" />

declare var Ext: any;
declare var Opentip: any;
$ = require("jquery");
import TextResourceModule = require("Framework/Resource/TextResource");

/**
 * Блок регулирования доступа к торговле в зависимости от роли пользователя
 */
export class UserRoleTradeAccessBlock
{
  /**
   * @param outerWrapperId Id внешнего блока, куда вставить данный блок
   */
  constructor(outerWrapperId: string)
  {    
    this._TextResources = TextResourceModule.TextResource.GetResources([
      { ProjectNamespace: "MLMExchange", ResourceNamespace: "PrivateResource", ResourceId: "RolesAccessBlock__Header" },
      { ProjectNamespace: "MLMExchange", ResourceNamespace: "PrivateResource", ResourceId: "RolesAccessBlock__Description" }
    ]);

    this._BlockOuterWrapperId = outerWrapperId;
    this._BlockContentId = Ext.id();

    //#region Set store
    this._Store = new Ext.data.JsonStore({
      storeId: 'Roles.TradeAccessBlock.UserRoles',      

      fields: ['Id', 'RoleTypeName', 'RoleTypeDisplayName', 'IsTradeEnabled'],
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
          read: '/AdminPanel/BaseUserRole/GetAllRoleTypeAccessLevels',
          update: '/AdminPanel/BaseUserRole/UpdateAllRoleTypeAccessLevels'
        },
        callback: () =>
        {
        }
      },
      listeners: {
        load: () => 
        {             
          if (!this._RoleAccessLevelBlock.rendered)
            this._RoleAccessLevelBlock.render(this._BlockOuterWrapperId);

          if (!this._RoleAccessDataView.rendered)
            this._RoleAccessDataView.render(this._BlockContentId);
          else 
            this._RoleAccessDataView.refresh();

          if (this._BlockMask)
            this._BlockMask.hide();
        }
      }
    });
    //#endregion

    this._RoleAccessLevelBlock = new Ext.Component({                                          
      html: 
        '<section class="b-rta__section">' +
        '<header class="b-rta__header">' + $.grep(this._TextResources, function(e){ return e.Id == "MLMExchange.PrivateResource.RolesAccessBlock__Header"; })[0].Value + '</header>' +
        '<i class="b-rta__icon fa fa-question"></i>' +

        '<div class="b-rta__content" id="' + this._BlockContentId + '"></div>' +
        '</section>',
      afterRender: () =>
      {
        this._BlockMask = new Ext.LoadMask($("#" + this._BlockContentId)[0], {useMsg: true});

        new Opentip($('.b-rta__icon'), $.grep(this._TextResources, function(e){ return e.Id == "MLMExchange.PrivateResource.RolesAccessBlock__Description"; })[0].Value, { style: 'BlackStyle' });
      }
    });

    this._RoleAccessDataView = Ext.create('RoleTradeAccessDataView',
      {
        store: this._Store,
        loadMask: false,
        onItemClick: (dataView, item, index, e) =>
        {
          var roleId: number = $(item).data("role-id");
          var role = this._Store.findRecord('Id', roleId);

          role.set('IsTradeEnabled', !role.get('IsTradeEnabled'));
          this._BlockMask.show();
          this._Store.sync({
               success: () => {
                 this._Store.commitChanges();
                 this._Store.load();
               },
               failure: () => {
                 this._Store.rejectChanges();
                 this._Store.load();                 
               }
             });
        }
      });
  }

  private _Store: any;
  private _BlockOuterWrapperId: string;
  private _RoleAccessDataView: any;
  private _BlockContentId;
  private _RoleAccessLevelBlock: any;
  private _BlockMask;
  private _TextResources: TextResourceModule.TextResourceModel[];

  public Render(): void
  { 
    this._Store.load();
  }
}

Ext.define('RoleTradeAccessDataView', {
  extend: 'Ext.view.View',
  layout: 'fit',
  itemSelector: '.b-rta__role-block',
  itemTpl: new Ext.XTemplate(
    '<div class="b-rta__role-block b-rta__role-block_role-name_{[values.RoleTypeName.toLowerCase()]} b-rta__role-block_state_{IsTradeEnabled}" data-role-id={Id}>',
      '<i class="b-rta__disabled-icon fa fa-ban"></i>',
      '<span class="b-rta__role-display-name">{RoleTypeDisplayName}</span>',
    '</div>')
});
