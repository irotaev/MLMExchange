/// <reference path="../typings/jquery/jquery.d.ts" />
define(["require", "exports", "jquery"], function(require, exports) {
    /// <amd-dependency path="jquery" />
    $ = require("jquery");

    /**
    * Приложение.
    * Это базовый класс для всего приложения.
    * Тут производятся инициализация и базовые настройки для всего приложения вцелом.
    */
    var App = (function () {
        function App() {
        }
        /**
        * Применить настройки к сайту
        */
        App.ApplySiteSettings = function () {
        };
        return App;
    })();
    exports.App = App;

    /**
    * Зона admin панели
    */
    var AdmineArea = (function () {
        function AdmineArea() {
        }
        AdmineArea.prototype.Initilize = function () {
            //#region НАстройка высоты главного контейнера admin панели
            Ext.onReady(function () {
                Ext.getCmp('AdminPanel__MainContainer').setHeight($("#AdminPanel__CenterBlock-innerCt")[0].scrollHeight + 20);
            });
            //#endregion
        };
        return AdmineArea;
    })();
    exports.AdmineArea = AdmineArea;
});
//# sourceMappingURL=Site.js.map
