/// <reference path="../typings/jquery/jquery.d.ts" />

/// <amd-dependency path="jquery" />

$ = require("jquery");
declare var Ext: any;

/**
 * Приложение.
 * Это базовый класс для всего приложения. 
 * Тут производятся инициализация и базовые настройки для всего приложения вцелом.
 */
export class App 
{
  /**
   * Применить настройки к сайту
   */
  public static ApplySiteSettings(): void
  {

  }

  /**
   * Инициализировать заново tooltip
   * @param $container Контейнер где искать tooltyip'ы. Если не задан, то будет взят body
   */
  public static ReinitializeTooltip($container: JQuery = null): void
  {
    var $ = (<any>window).$Sync;

    $container = $container || $("body");

    $.each($container.find("[data-ot]"), function (index, element)
    {
      var $element = $(element);

      $element.opentip($element.data("ot"), { style: $element.data("ot-style") });
    });
  }
}

/**
 * Зона admin панели
 */
export class AdmineArea
{
  public Initilize(): void
  {
    //#region НАстройка высоты главного контейнера admin панели
    Ext.onReady(function ()
    {
      var scrollHeight = $("#AdminPanel__CenterBlock-innerCt")[0].scrollHeight;
      Ext.getCmp('AdminPanel__MainContainer').setHeight(scrollHeight + 20);
      $("#AdminPanel__CenterBlock_Content").height(scrollHeight).css("overflow-y", "auto");
    });
    //#endregion
  }  
}