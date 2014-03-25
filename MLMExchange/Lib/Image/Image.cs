using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLMExchange.Lib.Image
{
  public class Image
  {
    public Image(string imagePath)
    {
      _ImagePath = imagePath;
    }

    private readonly string _ImagePath;

    public string Crop(string width, string height)
    {
      return null;
    }

  }
}