/// <reference path="../typings/jquery/jquery.d.ts" />
define(["require", "exports", "jquery"], function(require, exports) {
    /// <amd-dependency path="jquery" />
    $ = require("jquery");

    /**
    * Операции с формой
    */
    var Form = (function () {
        function Form($form) {
            this._$Form = $form;
        }
        /**
        * Применить параметры по умолчанию
        */
        Form.prototype.ApplyDefaults = function () {
            //#region Поле загрузки фотографии
            var photoFields = this._$Form.find(".b-ib_content_photo");

            $.each(photoFields, function (index, field) {
                var imageField = $(field).find(".b-ib__file-preview .b-ib__image");

                if (!imageField.attr("src"))
                    imageField.attr("src", "/Content/images/Form/anonymous-user.png");

                $(field).find(".b-ib_file-chooser").bind("click", function (event) {
                    $(field).find(".b-ib_choose-file").trigger("click");
                });

                $(field).find(".b-ib_choose-file").change(function (event) {
                    var inputElem = event.currentTarget;

                    if (inputElem.files && inputElem.files[0]) {
                        var reader = new FileReader();

                        reader.onload = function (e) {
                            $(field).find(".b-ib__file-preview .b-ib__image").attr('src', e.target.result);
                        };

                        reader.readAsDataURL(inputElem.files[0]);
                    }
                });
            });

            if (navigator.userAgent.indexOf("MSIE 9") < 0 && navigator.userAgent.indexOf("MSIE 8") < 0 && navigator.userAgent.indexOf("MSIE 7") < 0) {
                var passwordFields = this._$Form.find(".b-ib__input[type='password']");

                $.each(passwordFields, function (index, field) {
                    var replacedObject = $("<div class='b-ib__input-wrapper b-ib__input-wrapper_input-type_password'><i class='b-ib__icon'></div></div>").replaceAll($(field));

                    replacedObject.append($(field));

                    //$(field).remove();
                    var $field = replacedObject.find(".b-ib__input");

                    $field.focus(function (event) {
                        $field.closest(".b-ib__input-wrapper").find(".b-ib__icon").addClass("focused");
                    }).blur(function (event) {
                        $field.closest(".b-ib__input-wrapper").find(".b-ib__icon").removeClass("focused");
                    });
                });
            }

            //#endregion
            //#region инийиализирую кнопки отчиски поля
            $.each($(".b-ib .btn-clear"), function (index, elem) {
                $(elem).click(function (event) {
                    $(event.currentTarget).closest(".b-ib").find(".b-ib__input").val("");

                    event.preventDefault();

                    return false;
                });
            });

            //#endregion
            return this;
        };

        /**
        * Привязать валидацию
        */
        Form.prototype.BindValidation = function () {
            (window).$Sync.validator.unobtrusive.parse(this._$Form);

            return this;
        };

        /**
        * Привязать плагин iCheck к форме
        */
        Form.prototype.BindICheck = function () {
            var $ = (window).$Sync;

            $(this._$Form).find("input[type='checkbox']").not("[data-icheck='true']").css({ "position": "absolute", "top": "0", "left": "0", "visibility": "hidden" });

            $(this._$Form).find('input[data-icheck="true"]').iCheck({
                labelHover: false,
                cursor: true,
                checkboxClass: 'icheckbox_square-green',
                radioClass: 'icheckbox_square-green',
                increaseArea: '20%'
            }).on("ifChanged", function (event) {
                $(event.currentTarget).closest(".b-ib").find(".b-ib_checkbox").click();
            });

            return this;
        };

        /**
        * Привязать Tooltip (OpenTip) к форме
        */
        Form.prototype.BindTooltip = function () {
            var $ = (window).$Sync;

            $.each(this._$Form.find("[data-ot]"), function (index, element) {
                var $element = $(element);

                $element.opentip($element.data("ot"), { style: $element.data("ot-style") });
            });

            return this;
        };

        /**
        * Обновит все скрипты, относящиеся к данной форме
        */
        Form.prototype.ReBindAll = function () {
            this.ApplyDefaults();
            this.BindValidation();
            this.BindICheck();
            this.BindTooltip();

            return this;
        };
        return Form;
    })();
    exports.Form = Form;
});
//# sourceMappingURL=Form.js.map
