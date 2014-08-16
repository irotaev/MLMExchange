/// <reference path="../../typings/jquery/jquery.d.ts" />
define(["require", "exports", "jquery"], function(require, exports) {
    
    $ = require("jquery");

    /**
    * Текстовый ресурс.
    * Отвечает за получение ресурсов с сервера и т.д.
    */
    var TextResource = (function () {
        function TextResource() {
        }
        /**
        * Получить ресурсы с сервера.
        * @param request Запрос на сервер
        *    *projectNamespace Namespace проекта, где зранится ресурс
        *    *resourceNamespace Namespace класса ресурса (как правило совпадает с название класса)
        *    *resourceId Id ресурса (ключ ресурса, его имя)
        */
        TextResource.GetResources = function (request) {
            var textModels = [];

            $.ajax({
                url: '/Resources/GetTextResources',
                type: 'POST',
                data: JSON.stringify({ requests: request }),
                dataType: 'json',
                async: false,
                contentType: 'application/json'
            }).done(function (response) {
                $.each(response, function (index, el) {
                    var model = new TextResourceModel();
                    model.Id = el.ResourceId;
                    model.Value = el.ResourceValue;

                    textModels.push(model);
                });
            });

            return textModels;
        };
        return TextResource;
    })();
    exports.TextResource = TextResource;

    /**
    * Модель текстового ресурса
    */
    var TextResourceModel = (function () {
        function TextResourceModel() {
        }
        return TextResourceModel;
    })();
    exports.TextResourceModel = TextResourceModel;
});
//# sourceMappingURL=TextResource.js.map
