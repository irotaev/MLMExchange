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

        /**
        * Инициализировать заново tooltip
        * @param $container Контейнер где искать tooltyip'ы. Если не задан, то будет взят body
        */
        App.ReinitializeTooltip = function ($container) {
            if (typeof $container === "undefined") { $container = null; }
            var $ = window.$Sync;

            $container = $container || $("body");

            $.each($container.find("[data-ot]"), function (index, element) {
                var $element = $(element);

                $element.opentip($element.data("ot"), { style: $element.data("ot-style") });
            });
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
                var scrollHeight = $("#AdminPanel__CenterBlock-innerCt")[0].scrollHeight;
                Ext.getCmp('AdminPanel__MainContainer').setHeight(scrollHeight + 20);
                $("#AdminPanel__CenterBlock_Content").height(scrollHeight).css("overflow-y", "auto");
            });
            //#endregion
        };
        return AdmineArea;
    })();
    exports.AdmineArea = AdmineArea;
});
//# sourceMappingURL=Site.js.map
