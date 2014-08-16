 /// <reference path="../../typings/jquery/jquery.d.ts" />

/// <amd-dependency path="jquery" />

declare var Ext: any;
$ = require("jquery");

/**
 * Текстовый ресурс.
 * Отвечает за получение ресурсов с сервера и т.д.
 */
export class TextResource
{
  /**
   * Получить ресурсы с сервера.
   * @param request Запрос на сервер
   *    *projectNamespace Namespace проекта, где зранится ресурс
   *    *resourceNamespace Namespace класса ресурса (как правило совпадает с название класса)
   *    *resourceId Id ресурса (ключ ресурса, его имя)
   */
  public static GetResources(request: {ProjectNamespace: string; ResourceNamespace: string; ResourceId: string }[]): TextResourceModel[]
  {
    var textModels: TextResourceModel[] = [];

    $.ajax({
      url: '/Resources/GetTextResources',
      type: 'POST',
      data: JSON.stringify({ requests: request }),
      dataType: 'json',
      async: false,
      contentType: 'application/json',
    }).done((response: any) =>
    {
      $.each(response, function(index, el)
      {
        var model = new TextResourceModel();
        model.Id = el.ResourceId;
        model.Value = el.ResourceValue;

        textModels.push(model);
      });
    });

    return textModels;
  }
}

/**
 * Модель текстового ресурса
 */
export class TextResourceModel
{
  /**
   * Id ресурса
   */
  public Id: string;
  /**
   * Значение ресурса
   */
  public Value: string;
}