/// <reference path="../typings/jquery/jquery.d.ts" />

/// <amd-dependency path="jquery" /> 

/**
 * Расширение интерфейса JQuery.
 * Добавление новых методов
 */
interface JQuery {
  scrollTop(): JQuery;
  scrollTop(settings: Object): JQuery;
}

(function ($) {

  $.fn.scrollTopNow = function (settings) {

    var config = {
      speed : 1000
    };

    if (settings) {
      $.extend(config, settings);
    }

    return this.each(function () {

      var $this = $(this);

      $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
          $this.fadeIn();
        } else {
          $this.fadeOut();
        }
      });

      $this.click(function (e) {
        console.log("Hello");
        e.preventDefault();
        $("body, html").animate({
          scrollTop: 0
        }, config.speed);
      });

    });

  };

})(jQuery);