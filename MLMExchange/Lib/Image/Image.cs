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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="imageRelativePath">Путь к картинке относительно корня сайта</param>
    /// <param name="imageName">Имя картинки</param>
    public Image(string imageRelativePath, string imageName)
    {
      if (String.IsNullOrEmpty(imageName))
        throw new ApplicationException("imageName cant be null");

      _CurrentImageInfo = new ImageInfo(imageRelativePath, imageName);
      _ImageUploadDir = imageRelativePath;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="imageFromUploadFullName">Имя картинки из директории Upload</param>
    public Image(string imageFromUploadFullName) : this("Uploads\\Images", imageFromUploadFullName) { }

    private readonly ImageInfo _CurrentImageInfo;

    private readonly string _ImageUploadDir;
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

      if (File.Exists(Path.Combine(BaseFile.ServerAbsolutePath, _ImageUploadDir, imageCropedName)))
        return new ImageInfo(_ImageUploadDir, imageCropedName);

      Rectangle rectangle = new Rectangle(0, 0, width, height);
      Bitmap imageBitmap = (Bitmap)_CurrentImageInfo.Image;
      Bitmap newImageBitmap = new Bitmap(rectangle.Width, rectangle.Height);
      newImageBitmap.SetResolution(imageBitmap.HorizontalResolution, imageBitmap.VerticalResolution);

      using (Graphics graphic = Graphics.FromImage(newImageBitmap))
      {
        graphic.SmoothingMode = SmoothingMode.AntiAlias;
        graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;

        graphic.DrawImage(imageBitmap, 0, 0, width, height);

        imageBitmap.Dispose();
      }

      newImageBitmap.Save(Path.Combine(BaseFile.ServerAbsolutePath, _ImageUploadDir, imageCropedName));
      newImageBitmap.Dispose();

      return new ImageInfo(_ImageUploadDir, imageCropedName);
    }

    /// <summary>
    /// Информация о фотографии
    /// </summary>
    public class ImageInfo
    {
      public ImageInfo(string imageRelativePath, string imageFullName)
      {
        if (String.IsNullOrEmpty(imageFullName))
          throw new ArgumentNullException("imageName");

        _ImageRelativePath = imageRelativePath;
        _ImageName = imageFullName;
      }

      private readonly string _ImageName;
      private readonly string _ImageRelativePath;

      /// <summary>
      /// Абсолютный путь от корня сайта - true, относительный - false
      /// </summary>
      public bool IsAbsolutePath { get; set; }

      public System.Drawing.Image Image
      {
        get
        {
          return System.Drawing.Image.FromFile(Path.Combine(BaseFile.ServerAbsolutePath, _ImageRelativePath, _ImageName));
        }
      }

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