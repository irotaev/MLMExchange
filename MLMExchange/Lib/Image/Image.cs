using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;

namespace MLMExchange.Lib.Image
{
  /// <summary>
  /// Фотография
  /// </summary>
  public class Image
  {
    public Image(string imageName)
    {
      if (String.IsNullOrEmpty(imageName))
        throw new ApplicationException("imageName cant be null");

      _CurrentImageInfo = new ImageInfo(imageName);
    }

    private readonly ImageInfo _CurrentImageInfo;

    public const string _ImageDir = "Uploads";
    public ImageInfo CurrentImage { get { return _CurrentImageInfo; } }

    /// <summary>
    /// Деформирует размеры изображдения
    /// </summary>
    /// <param name="width">Новая ширина</param>
    /// <param name="height">Новая высота</param>
    /// <returns>Новое изображдение</returns>
    public ImageInfo Crop(int width, int height)
    {
      string imageCropedName = _CurrentImageInfo.Name + String.Format("_{0}X{1}", width, height) + _CurrentImageInfo.Extension;

      if (File.Exists(Path.Combine(BaseFile.ServerAbsolutePath, _ImageDir, imageCropedName)))
        return new ImageInfo(imageCropedName);

      Rectangle rectangle = new Rectangle(0, 0, width, height);
      Bitmap imageBitmap = (Bitmap)_CurrentImageInfo.Image;
      Bitmap newImageBitmap = new Bitmap(rectangle.Width, rectangle.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      newImageBitmap.SetResolution(imageBitmap.HorizontalResolution, imageBitmap.VerticalResolution);

      using (Graphics graphic = Graphics.FromImage(newImageBitmap))
      {
        graphic.SmoothingMode = SmoothingMode.AntiAlias;
        graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;

        graphic.DrawImage(imageBitmap, 0, 0, width, height);

        imageBitmap.Dispose();
      }

      newImageBitmap.Save(Path.Combine(BaseFile.ServerAbsolutePath, _ImageDir, imageCropedName));
      newImageBitmap.Dispose();

      return new ImageInfo(imageCropedName);
    }

    /// <summary>
    /// Информация о фотографии
    /// </summary>
    public class ImageInfo
    {
      public ImageInfo(string imageFullName)
      {
        if (String.IsNullOrEmpty(imageFullName))
          throw new ArgumentNullException("imageName");

        _ImageName = imageFullName;
      }

      private readonly string _ImageName;

      public System.Drawing.Image Image { get { return System.Drawing.Image.FromFile(Path.Combine(BaseFile.ServerAbsolutePath, _ImageDir, _ImageName)); } }
      /// <summary>
      /// Полное имя изображения (+ расширение)
      /// </summary>
      public string FullName { get { return _ImageName; } }
      /// <summary>
      /// Имя изображения
      /// </summary>
      public string Name { get { return !String.IsNullOrEmpty(FullName) ? Path.GetFileNameWithoutExtension(FullName) : null; } }
      /// <summary>
      /// Разрешение изображения
      /// </summary>
      public string Extension { get { return !String.IsNullOrEmpty(FullName) ? Path.GetExtension(FullName) : null; } }
    }
  }
}