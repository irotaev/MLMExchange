using DataAnnotationsExtensions;
using Logic;
using MLMExchange.Lib.DataValidation;
using MLMExchange.Lib.Exception;
using MLMExchange.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MLMExchange.Areas.AdminPanel.Models
{
  public class RandomWordsModel : AbstractDataModel<D_RandomWord, RandomWordsModel>, IDataBinding<D_RandomWord, RandomWordsModel>
  {
    /// <summary>
    /// Автор высказывания
    /// </summary>
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string Author { get; set; }

    /// <summary>
    /// Текст высказывания
    /// </summary>
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string Text { get; set; }

    public override RandomWordsModel Bind(D_RandomWord @object)
    {
      if (@object == null)
        throw new ArgumentNullException("object");

      base.Bind(@object);

      Author = @object.Author;
      Text = @object.Text;

      return this;
    }

    public override D_RandomWord UnBind(D_RandomWord @object = null)
    {
      if (@object == null)
        @object = new D_RandomWord();

      base.UnBind(@object);

      if (Author == null)
        throw new Logic.Lib.UserVisible__ArgumentNullException("Author");
      @object.Author = Author;

      if (Text == null)
        throw new Logic.Lib.UserVisible__ArgumentNullException("Text");
      @object.Text = Text;

      return @object;
    }
  }
}