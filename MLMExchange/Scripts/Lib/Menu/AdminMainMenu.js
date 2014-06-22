/// <reference path="../../typings/jquery/jquery.d.ts" />
define(["require", "exports", "jquery"], function(require, exports) {
    /// <amd-dependency path="jquery" />
    $ = require("jquery");

    /**
    * Главное меню админ-панели
    */
    var AdminMainMenu = (function () {
        function AdminMainMenu($menu) {
            this._$Menu = $menu;
        }
        /**
        * Выделить пункт меню.
        * Выделяет пункт меню, в зависимости от текущего url
        */
        AdminMainMenu.prototype.SelectMenuItem = function (num) {
            this._$Menu.find("a").each(function () {
                var originalUrl = location.protocol + '//' + location.host + location.pathname;
                if (originalUrl == this.href) {
                    $(this).find("nav").addClass("pb-mm__menu-item-wrapper-active");
                }
            });
        };
        return AdminMainMenu;
    })();
    exports.AdminMainMenu = AdminMainMenu;
});
//# sourceMappingURL=AdminMainMenu.js.map
