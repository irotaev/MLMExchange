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
            var url = window.location.protocol + "//" + window.location.hostname + window.location.pathname;

            this._$Menu.find("a").each(function () {
                if (url == (this.href)) {
                    $(this).find("nav").addClass("pb-mm__menu-item-wrapper-active");
                }
            });
        };
        return AdminMainMenu;
    })();
    exports.AdminMainMenu = AdminMainMenu;
});
//# sourceMappingURL=AdminMainMenu.js.map
