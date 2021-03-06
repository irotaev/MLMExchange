﻿requirejs.config((function () {
    var jqueryVersion = null;

    //#region Версия JQuery для разный браузеров
    if (navigator.userAgent.indexOf("MSIE 8") || navigator.userAgent.indexOf("MSIE 7") || navigator.userAgent.indexOf("MSIE 6")) {
        jqueryVersion = 'jquery-1.11.0.min';
    } else {
        jqueryVersion = 'jquery-2.0.3.min';
    }

    //#endregion
    //#region Настройка конфигурации
    var config = {
        baseUrl: "/Scripts",
        paths: {
            jquery: jqueryVersion
        }
    };

    //#endregion
    return config;
})());
//# sourceMappingURL=config.js.map
