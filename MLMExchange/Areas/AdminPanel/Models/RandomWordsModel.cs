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
  public class RandomWordsModel : AbstractDataModel<D_RandomWord, RandomWordsModel>
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
      base.Bind(@object);

      Author = @object.Author;
      Text = @object.Text;

      return this;
    }

    public override D_RandomWord UnBind(D_RandomWord @object = null)
    {
      var d_randomwords = base.UnBind(@object);

      if (Author == null)
        throw new ArgumentNullException("Author");

      d_randomwords.Author = Author.ToString();

      if (Text == null)
        throw new ArgumentNullException("Text");

      d_randomwords.Text = Text.ToString();

      return d_randomwords;
    }
  }
}