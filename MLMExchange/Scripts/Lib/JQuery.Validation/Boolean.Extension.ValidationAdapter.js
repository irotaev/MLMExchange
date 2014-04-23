jQuery.validator.addMethod("boolean", function (value, element) {
  return element.checked;
});
jQuery.validator.unobtrusive.adapters.addBool("boolean");