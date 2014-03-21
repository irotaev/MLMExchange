/// <reference path="../typings/jquery/jquery.d.ts" />

/// <amd-dependency path="jquery" />

$ = require("jquery");

/**
 * Операции с формой
 */
export class Form
{
  private _$Form: JQuery;

  constructor($form: JQuery)
  {
    this._$Form = $form;
  }

  /**
   * Применить параметры по умолчанию
   */
  public ApplyDefaults(): void
  {
    //#region Поле загрузки фотографии
    var photoFields = this._$Form.find(".b-ib_content_photo");

    $.each(photoFields, (index, field) =>
    {
      $(field).find(".b-ib__file-preview .b-ib__image").attr("src", "/Content/images/Form/anonymous-user.png");

      $(field).find(".b-ib_file-chooser").bind("click", (event: Event) =>
      {
        $(field).find(".b-ib_choose-file").trigger("click");
      })

      $(field).find(".b-ib_choose-file").change((event: Event) =>
      {
        var inputElem: HTMLInputElement = <HTMLInputElement>event.currentTarget;

        if (inputElem.files && inputElem.files[0])
        {
          var reader = new FileReader();

          reader.onload = (e) =>
          {
            $(field).find(".b-ib__file-preview .b-ib__image").attr('src', e.target.result);
          };

          reader.readAsDataURL(inputElem.files[0]);
        }
      });
    });
    //#endregion

    //#region Замок для поля с паролем
    if (navigator.userAgent.indexOf("MSIE 9") < 0 && navigator.userAgent.indexOf("MSIE 8") < 0 && navigator.userAgent.indexOf("MSIE 7") < 0)
    {
      var passwordFields = this._$Form.find(".b-ib__input[type='password']");

      $.each(passwordFields, (index, field) =>
      {
        var replacedObject = $("<div class='b-ib__input-wrapper b-ib__input-wrapper_input-type_password'><i class='b-ib__icon'></div></div>").replaceAll($(field));

        replacedObject.append($(field));

        //$(field).remove();

        var $field = replacedObject.find(".b-ib__input");

        $field.focus((event: Event) =>
        {
          $field.closest(".b-ib__input-wrapper").find(".b-ib__icon").addClass("focused");
        })
          .blur((event: Event) =>
          {
            $field.closest(".b-ib__input-wrapper").find(".b-ib__icon").removeClass("focused");
          });
      });
    }
    //#endregion
  }
}