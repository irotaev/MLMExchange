/// <reference path="../../../typings/jquery/jquery.d.ts" />

/// <amd-dependency path="jquery" /> 

var $ = require("jquery");

/**
 * Блок админ-панели
 */
export class AdminBlock
{
  private _Wrapper: JQuery;

  constructor($element: JQuery)
  {
    this._Wrapper = $element;

    var $additionalInfoWrapper = this._Wrapper.find(".b-ab__additional-info-wrapper");

    if ($additionalInfoWrapper.length > 0)
    {
      this.InitAdditionalInfo($additionalInfoWrapper);
    }
  }

  private InitAdditionalInfo($wrapper: JQuery): void
  {
    var $arrow = $wrapper.find(".b-ab__open-icon");
    var $additionalInfoContent = $wrapper.find(".b-ab__ai-content");
    var $content = this._Wrapper.find(".b-ab__content");
    var contentOuterStartHeight = $content.outerHeight();

    var additionalInfoFullHeight = $additionalInfoContent[0].scrollHeight + $arrow.outerHeight() + 100;

    var openCallback = function () 
    {
      var openAdditionalInfoWrapperCallback = function ()
      {
        $additionalInfoContent.animate({ height: $additionalInfoContent[0].scrollHeight }, "slow", function ()
        {          
          $arrow.one("click", closeCallback);
        });
      };

      $arrow.removeClass("b-ab__open-icon_rotate_0").addClass("b-ab__open-icon_rotate_180");

      if (contentOuterStartHeight < additionalInfoFullHeight)
      {
        $content.animate({ height: additionalInfoFullHeight }, "slow", openAdditionalInfoWrapperCallback);
      }
      else 
      {
        openAdditionalInfoWrapperCallback();
      }
    };

    var closeCallback = function ()
    {
      var closeAdditionalInfoWrapperCallback = function () 
      { 
        $arrow.removeClass("b-ab__open-icon_rotate_180").addClass("b-ab__open-icon_rotate_0");       
        $arrow.one("click", openCallback);
      };

      $additionalInfoContent.animate({ height: 0 }, "slow", function ()
      {
        if (contentOuterStartHeight != $content.outerHeight())
        {
          $content.animate({ height: contentOuterStartHeight }, "slow", closeAdditionalInfoWrapperCallback);
        }
        else 
        {
          closeAdditionalInfoWrapperCallback();
        }
      });
    };

    $arrow.one("click", closeCallback);    

    openCallback();
  }
} 