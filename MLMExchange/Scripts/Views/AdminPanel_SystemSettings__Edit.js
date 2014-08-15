/// <reference path="../typings/jquery/jquery.d.ts" />
define(["require", "exports", "jquery"], function(require, exports) {
    
    $ = require("jquery");

    /**
    * Блок регулирования доступа к торговле в зависимости от роли пользователя
    */
    var UserRoleTradeAccessBlock = (function () {
        /**
        * @param outerWrapperId Id внешнего блока, куда вставить данный блок
        */
        function UserRoleTradeAccessBlock(outerWrapperId) {
            var _this = this;
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
                    callback: function () {
                    }
                },
                listeners: {
                    load: function () {
                        var component = new Ext.Component({
                            html: '<section class="b-rta__section">' + '<i class="b-rta__icon fa fa-question"></i>' + '<div class="b-rta__content" id="' + _this._BlockContentId + '"></div>' + '</section>',
                            listeners: {
                                afterrender: function () {
                                    _this._RoleAccessDataView.render(_this._BlockContentId);
                                }
                            }
                        });

                        component.render(_this._BlockOuterWrapperId);
                    }
                }
            });

            //#endregion
            this._RoleAccessDataView = Ext.create('RoleTradeAccessDataView', {
                store: this._Store,
                listeners: {
                    afterrender: function (component) {
                        $(component.el.dom).find(".b-rta__role-block").on("click", function (event) {
                            console.log(event.currentTarget);
                        });
                    }
                }
            });
        }
        UserRoleTradeAccessBlock.prototype.Render = function () {
            this._Store.load();
        };
        return UserRoleTradeAccessBlock;
    })();
    exports.UserRoleTradeAccessBlock = UserRoleTradeAccessBlock;

    Ext.define('RoleTradeAccessDataView', {
        extend: 'Ext.DataView',
        layout: 'fit',
        itemTpl: new Ext.XTemplate('<div class="b-rta__role-block b-rta__role-block_role-name_{[values.RoleTypeName.toLowerCase()]}" data-role-id={Id}>', '<span class="b-rta__role-display-name">{RoleTypeDisplayName}</span>', '</div>')
    });
});
//# sourceMappingURL=AdminPanel_SystemSettings__Edit.js.map
