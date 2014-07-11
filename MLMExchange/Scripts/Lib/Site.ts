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
      Ext.getCmp('AdminPanel__MainContainer').setHeight($("#AdminPanel__CenterBlock-innerCt")[0].scrollHeight + 20);
    });
    //#endregion
  }
}