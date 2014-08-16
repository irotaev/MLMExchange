/// <reference path="../typings/jquery/jquery.d.ts" />
define(["require", "exports", "Framework/Resource/TextResource", "jquery"], function(require, exports, TextResourceModule) {
    

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
                    callback: function () {
                    }
                },
                listeners: {
                    load: function () {
                        if (!_this._RoleAccessLevelBlock.rendered)
                            _this._RoleAccessLevelBlock.render(_this._BlockOuterWrapperId);

                        if (!_this._RoleAccessDataView.rendered)
                            _this._RoleAccessDataView.render(_this._BlockContentId);
                        else
                            _this._RoleAccessDataView.refresh();

                        if (_this._BlockMask)
                            _this._BlockMask.hide();
                    }
                }
            });

            //#endregion
            this._RoleAccessLevelBlock = new Ext.Component({
                html: '<section class="b-rta__section">' + '<header class="b-rta__header">' + $.grep(this._TextResources, function (e) {
                    return e.Id == "MLMExchange.PrivateResource.RolesAccessBlock__Header";
                })[0].Value + '</header>' + '<i class="b-rta__icon fa fa-question"></i>' + '<div class="b-rta__content" id="' + this._BlockContentId + '"></div>' + '</section>',
                afterRender: function () {
                    _this._BlockMask = new Ext.LoadMask($("#" + _this._BlockContentId)[0], { useMsg: true });

                    new Opentip($('.b-rta__icon'), $.grep(_this._TextResources, function (e) {
                        return e.Id == "MLMExchange.PrivateResource.RolesAccessBlock__Description";
                    })[0].Value, { style: 'BlackStyle' });
                }
            });

            this._RoleAccessDataView = Ext.create('RoleTradeAccessDataView', {
                store: this._Store,
                loadMask: false,
                onItemClick: function (dataView, item, index, e) {
                    var roleId = $(item).data("role-id");
                    var role = _this._Store.findRecord('Id', roleId);

                    role.set('IsTradeEnabled', !role.get('IsTradeEnabled'));
                    _this._BlockMask.show();
                    _this._Store.sync({
                        success: function () {
                            _this._Store.commitChanges();
                            _this._Store.load();
                        },
                        failure: function () {
                            _this._Store.rejectChanges();
                            _this._Store.load();
                        }
                    });
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
        extend: 'Ext.view.View',
        layout: 'fit',
        itemSelector: '.b-rta__role-block',
        itemTpl: new Ext.XTemplate('<div class="b-rta__role-block b-rta__role-block_role-name_{[values.RoleTypeName.toLowerCase()]} b-rta__role-block_state_{IsTradeEnabled}" data-role-id={Id}>', '<i class="b-rta__disabled-icon fa fa-ban"></i>', '<span class="b-rta__role-display-name">{RoleTypeDisplayName}</span>', '</div>')
    });
});
//# sourceMappingURL=AdminPanel_SystemSettings__Edit.js.map
