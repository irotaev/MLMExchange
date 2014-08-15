/// <reference path="../typings/jquery/jquery.d.ts" />

/// <amd-dependency path="jquery" />

declare var Ext: any;
$ = require("jquery");

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
          read: '/AdminPanel/BaseUserRole/GetAllRoleTypeAccessLevels'
        },
        callback: () =>
        {
        }
      },
      listeners: {
        load: () => 
        {
          var component = new Ext.Component({                                          
            html: 
              '<section class="b-rta__section">' +
              '<i class="b-rta__icon fa fa-question"></i>' +

              '<div class="b-rta__content" id="' + this._BlockContentId + '"></div>' +
              '</section>'
            ,
            listeners:
            {
              afterrender: () =>
              {
                this._RoleAccessDataView.render(this._BlockContentId);
              },
            }
          });   

          component.render(this._BlockOuterWrapperId);                 
        }
      }
    });
    //#endregion

    this._RoleAccessDataView = Ext.create('RoleTradeAccessDataView',
      {
        store: this._Store,
        listeners: {
          afterrender: (component) =>
          {
            $(component.el.dom).find(".b-rta__role-block").on("click", (event: Event) =>
            {
              console.log(event.currentTarget);
            });
          }
        }
      });
  }

  private _Store: any;
  private _BlockOuterWrapperId: string;
  private _RoleAccessDataView: any;
  private _BlockContentId;

  public Render(): void
  { 
    this._Store.load();  
  }
}

Ext.define('RoleTradeAccessDataView', {
  extend: 'Ext.DataView',
  layout: 'fit',  
  itemTpl: new Ext.XTemplate(
    '<div class="b-rta__role-block b-rta__role-block_role-name_{[values.RoleTypeName.toLowerCase()]}" data-role-id={Id}>',
    '<span class="b-rta__role-display-name">{RoleTypeDisplayName}</span>',
    '</div>'
    )
});
