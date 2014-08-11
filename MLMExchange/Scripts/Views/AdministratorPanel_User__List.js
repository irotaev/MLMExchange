/// <reference path="../typings/jquery/jquery.d.ts" />
define(["require", "exports", "jquery"], function(require, exports) {
    
    $ = require("jquery");

    var RoleListTemplate = (function () {
        function RoleListTemplate(userId) {
            var _this = this;
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
                    callback: function () {
                        if (_this._ListWindow)
                            _this._ListWindow.doLayout();
                    }
                },
                listeners: {
                    load: function () {
                        _this.ShowRolesList__Window();
                        _this._ListWindow.toBack();

                        if (_this._LoadingMask)
                            _this._LoadingMask.hide();
                    }
                }
            });

            this._RolesStore.load();
        }
        RoleListTemplate.prototype.ShowRolesList__Window = function () {
            var _this = this;
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
                listeners: {
                    afterrender: function () {
                        $(_this._ListWindow.el.dom).find(".b-rl").find(".b-rl__action_function_delete").on("click", function (el) {
                            var id = $(el.currentTarget).closest(".b-rl__role").data("id");

                            var role = _this._RolesStore.findRecord('Id', id);

                            _this._RolesStore.remove([role]);

                            _this._LoadingMask.show();
                            _this._RolesStore.sync({
                                success: function () {
                                    _this._RolesStore.commitChanges();
                                    _this._RolesStore.load();
                                },
                                failure: function () {
                                    _this._RolesStore.rejectChanges();
                                    _this._RolesStore.load();
                                }
                            });
                        });

                        _this._LoadingMask = new Ext.LoadMask(Ext.get('b-rl_ID'), { msg: _this._RolesStore.data.items[0].data.TextResources.LoadingMessage });
                    }
                }
            });

            var roleList = Ext.create('CustomRoleListContainer');
            roleList.Roles = this._RolesStore.data.items.map(function (el) {
                return el.data;
            });
            roleList.Button = this.CreateAddNewRoleButton();
            this._ListWindow.items.add(roleList);

            //this._ListWindow.items.add();
            this._ListWindow.show();
        };

        RoleListTemplate.prototype.CreateAddNewRoleButton = function () {
            var _this = this;
            var buttonMenu = new Ext.menu.Menu({ listeners: {
                    click: function (j, button) {
                        _this._ListWindow.clearContent();

                        _this._RolesStore.add({ RoleType: button._key });

                        _this._LoadingMask.show();
                        _this._RolesStore.sync({
                            success: function () {
                                _this._RolesStore.commitChanges();
                                _this._RolesStore.load();
                            },
                            failure: function () {
                                _this._RolesStore.rejectChanges();
                                _this._RolesStore.load();
                            }
                        });
                    }
                } });

            $.each(this._RolesStore.data.items[0].data.TextResources.AllRoleTypePairs, function (index, el) {
                buttonMenu.add({
                    text: el.Value,
                    _key: el.Key
                });
            });

            var btn = new Ext.Button({
                text: this._RolesStore.data.items[0].data.TextResources.AddRole,
                menu: buttonMenu,
                height: 30,
                width: 570
            });

            return btn;
        };
        return RoleListTemplate;
    })();
    exports.RoleListTemplate = RoleListTemplate;

    Ext.define('CustomRoleListContainer', {
        extend: 'Ext.container.Container',
        layout: 'fit',
        Roles: undefined,
        Button: undefined,
        listeners: {
            beforerender: function () {
                var tpl = new Ext.XTemplate('<div class="b-rl" id="b-rl_ID">', '<div id="b-rl__button-place"></div>', '<tpl for=".">', '<div class="b-rl__role b-rl__role_type_{[values.RoleTypeAsString.toLowerCase()]}" data-id="{Id}">', '<div class="b-rl__role-display-name"> {RoleTypeDisplayName} </div>', '<div class="b-rl__actions">', '<span class="b-rl__action b-rl__action_function_delete"><i class="b-rl__icon fa fa-trash-o"></i>{TextResources.DeleteRole}</span>', '</div>', ' </div>', '</tpl>', '</div>');

                this.html = tpl.apply(this.Roles);
            },
            afterrender: function () {
                if (this.Button)
                    this.Button.render('b-rl__button-place');
            }
        }
    });
});
//# sourceMappingURL=AdministratorPanel_User__List.js.map
