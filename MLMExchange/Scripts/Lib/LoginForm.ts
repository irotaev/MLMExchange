/// <reference path="../typings/jquery/jquery.d.ts" />

/// <amd-dependency path="jquery" />

$ = require("jquery");

export class LoginForm
{
  private _LoginWrapper: JQuery;
  private _ResetWrapper: JQuery;

  constructor($loginElement: JQuery, $resetElement: JQuery)
  {
    this._LoginWrapper = $loginElement;
    this._ResetWrapper = $resetElement;

    this.SwipeContent(this._LoginWrapper, this._ResetWrapper);
  }

  private SwipeContent($loginWrapper: JQuery, $resetWrapper: JQuery) : void
  {
    var $buttonLogin = $loginWrapper.find("span.b-ib__swapper__button");
    var $buttonReset = $resetWrapper.find("span.b-ib__swapper__button");

    var startCallback = function ()
    {
      $resetWrapper.fadeOut('slow', function () {
        $loginWrapper.fadeIn('slow');
        $buttonLogin.one("click", stopCallback);
      });

      var swipeToResetWrapperCallback = () =>
      {
        $buttonLogin.one("click", stopCallback);
      };
    };

    var stopCallback = function ()
    {
      $loginWrapper.fadeOut('slow', function ()
      {
        $resetWrapper.fadeIn('slow');
        $buttonReset.one("click", startCallback);
      });

      var swipeToLoginWrapperCallback = () =>
      {
        $buttonReset.one("click", startCallback);
      };
    };
    

    startCallback();
  }
}