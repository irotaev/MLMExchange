/// <reference path="../typings/jquery/jquery.d.ts" />

/// <amd-dependency path="jquery" />

declare var Ext: any;
$ = require("jquery");

export class RoleListTemplate
{
  private _UserId: number;
  private _RolesStore: any;
  private _ListWindow: any;

  public constructor(userId: number)
  {    
    this._UserId = userId;

    this._RolesStore = new Ext.data.JsonStore({
      storeId: 'myStore',

      fields: ['Id', 'User', 'RoleType', 'RoleTypeAsString', 'RoleTypeDisplayName', 'TextResources'],
      proxy: {
        type: 'ajax',
        url: '/AdminPanel/BaseUserRole/GetAllUserRoles?userId=' + userId,
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
          update: '/AdminPanel/BaseUserRole/UpdateAllUserRoles',
          read: '/AdminPanel/BaseUserRole/GetAllUserRoles?userId=' + userId,
          create: '/AdminPanel/BaseUserRole/AddUserRoles?userId=' + userId,
          destroy: '/AdminPanel/BaseUserRole/RemoveUserRoles?userId=' + userId
        },
        callback: () => {
          if (this._ListWindow)
            this._ListWindow.doLayout();
        }
      },
      listeners: {
        load: () => 
        {
          this.ShowRolesList__Window();
          this._ListWindow.toBack();
        }
      }
    });

    this._RolesStore.load();
  }

  private ShowRolesList__Window(): void
  {
    this._ListWindow = new Ext.Window({
      id: "AdministratorUserList_Grid__ShowRoles_Window",
      width: 600,
      height: 500,
      modal: true,
      resizable: false,
      title: this._RolesStore.data.items[0].data.TextResources.Roles,
      autoScroll: true,
      //html: html,
      closeAction: 'hide',
      listeners : {
         afterrender: () => {
           $(this._ListWindow.el.dom).find(".b-rl").find(".b-rl__action_function_delete").on("click", (el) => {
             var id: number = $(el.currentTarget).closest(".b-rl__role").data("id");

             var role = this._RolesStore.findRecord('Id', id);

             this._RolesStore.remove([role]);
             this._RolesStore.sync({
               success: () => {
                 this._RolesStore.commitChanges();
                 this._RolesStore.load();
               },
               failure: () => {
                 this._RolesStore.rejectChanges();
                 this._RolesStore.load();
               }
             });
           });
         }
      }
    });

    var roleList = Ext.create('CustomRoleListContainer')
    roleList.Roles = this._RolesStore.data.items.map(function (el) { return el.data; });
    roleList.Button = this.CreateAddNewRoleButton();
    this._ListWindow.items.add(roleList);
    //this._ListWindow.items.add();
    this._ListWindow.show();
  }

  private CreateAddNewRoleButton(): any
  {
    var buttonMenu = new Ext.menu.Menu({
      listeners: {
        click: (j, button) => {
            this._ListWindow.clearContent();
            
            this._RolesStore.add({RoleType: button._key});
            this._RolesStore.sync({
              success: () => {
                this._RolesStore.commitChanges();
                this._RolesStore.load();
              },
              failure: () => {
                this._RolesStore.rejectChanges();
                this._RolesStore.load();
              }  
            });
        }
      }});

    $.each(this._RolesStore.data.items[0].data.TextResources.AllRoleTypePairs, function(index, el)
    {
      buttonMenu.add({
          text: el.Value,
          _key: el.Key
        });  
    });

    var btn = new Ext.Button({
        text: this._RolesStore.data.items[0].data.TextResources.AddRole,
        menu: buttonMenu
    });    

    return btn;
  }
}

Ext.define('CustomRoleListContainer',
  {
    extend: 'Ext.container.Container',
    layout: 'fit',
    Roles: undefined,
    Button: undefined,
    listeners: {
      beforerender: function () {
        var tpl = new Ext.XTemplate(
          '<div class="b-rl">',

          '<div id="b-rl__button-place"></div>',

          '<tpl for=".">',
          '<div class="b-rl__role b-rl__role_type_{[values.RoleTypeAsString.toLowerCase()]}" data-id="{Id}">',
          '<div class="b-rl__role-display-name"> {RoleTypeDisplayName} </div>',

          '<div class="b-rl__actions">',
          '<span class="b-rl__action b-rl__action_function_delete">{TextResources.DeleteRole}</span>',
          '</div>',
          ' </div>',
          '</tpl>',

          '</div>'
          );

        this.html = tpl.apply(this.Roles);
      },
      afterrender: function () {
        if (this.Button)
          this.Button.render('b-rl__button-place');
      }
    }
  });