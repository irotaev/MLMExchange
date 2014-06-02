/// <reference path="../../typings/jquery/jquery.d.ts" />

/// <amd-dependency path="jquery" /> 

$ = require("jquery");

import FormModule = require("../Form");

/**
 * Главное меню админ-панели
 */
export class AdminMainMenu
{
  constructor($menu: JQuery)
  {
    this._$Menu = $menu;
  }

  private _$Menu: JQuery;

  /**
   * Выделить пункт меню.
   * Выделяет пункт меню, в зависимости от текущего url
   */
  public SelectMenuItem(num: (num: number, name: string) => boolean): void
  {
    var url = window.location.href;

    this._$Menu.find("a").each(function ()
    {
      if (url == (this.href))
      {
        $(this).find("nav").addClass("pb-mm__menu-item-wrapper-active");
      }
    });
  }
}