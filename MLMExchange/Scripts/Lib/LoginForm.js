/// <reference path="../typings/jquery/jquery.d.ts" />
define(["require", "exports", "jquery"], function(require, exports) {
    /// <amd-dependency path="jquery" />
    $ = require("jquery");

    var LoginForm = (function () {
        function LoginForm($loginElement, $resetElement) {
            this._LoginWrapper = $loginElement;
            this._ResetWrapper = $resetElement;

            this.SwipeContent(this._LoginWrapper, this._ResetWrapper);
        }
        LoginForm.prototype.SwipeContent = function ($loginWrapper, $resetWrapper) {
            var $buttonLogin = $loginWrapper.find("span.b-ib__swapper__button");
            var $buttonReset = $resetWrapper.find("span.b-ib__swapper__button");

            var startCallback = function () {
                $resetWrapper.fadeOut('slow', function () {
                    $loginWrapper.fadeIn('slow');
                    $buttonLogin.one("click", stopCallback);
                });

                var swipeToResetWrapperCallback = function () {
                    $buttonLogin.one("click", stopCallback);
                };
            };

            var stopCallback = function () {
                $loginWrapper.fadeOut('slow', function () {
                    $resetWrapper.fadeIn('slow');
                    $buttonReset.one("click", startCallback);
                });

                var swipeToLoginWrapperCallback = function () {
                    $buttonReset.one("click", startCallback);
                };
            };

            startCallback();
        };
        return LoginForm;
    })();
    exports.LoginForm = LoginForm;
});
//# sourceMappingURL=LoginForm.js.map
