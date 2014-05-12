/// <reference path="../typings/jquery/jquery.d.ts" />

/// <amd-dependency path="jquery" />

$ = require("jquery");
declare var Opentip: any;

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
  public ApplyDefaults(): Form
  {
    //#region Поле загрузки фотографии
    var photoFields = this._$Form.find(".b-ib_content_photo");

    $.each(photoFields, (index, field) =>
    {
      var imageField = $(field).find(".b-ib__file-preview .b-ib__image");
      
      if (!imageField.attr("src"))
       imageField.attr("src", "/Content/images/Form/anonymous-user.png");

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

    //#region инийиализирую кнопки отчиски поля
    $.each($(".b-ib .btn-clear"), function (index, elem)
    {
      $(elem).click(function (event: Event)
      {
        $(event.currentTarget).closest(".b-ib").find(".b-ib__input").val("");

        event.preventDefault();

        return false;
      });
    });
    //#endregion

    return this;
  }

  /**
  * Привязать валидацию
  */
  public BindValidation(): Form
  {
    (<any>window).$Sync.validator.unobtrusive.parse(this._$Form);

    return this;
  }

  /**
  * Привязать плагин iCheck к форме
  */
  public BindICheck(): Form
  {
    var $ = (<any>window).$Sync;

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
  }

  /**
  * Привязать Tooltip (OpenTip) к форме
  */
  public BindTooltip(): Form
  {
    var $ = (<any>window).$Sync;

    $.each(this._$Form.find("[data-ot]"), function (index, element)
    {
      var $element = $(element);

      $element.opentip($element.data("ot"), { style: $element.data("ot-style") });
    });

    return this;
  }

  /**
  * Обновит все скрипты, относящиеся к данной форме
  */
  public ReBindAll(): Form
  {
    this.ApplyDefaults();
    this.BindValidation();
    this.BindICheck();
    this.BindTooltip();

    return this;
  }
}