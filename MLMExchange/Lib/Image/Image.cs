using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;
// TODO: Переделать
using Logic.Lib;

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
      {
        imageRelativePath = "Content\\images\\Form";
        imageName = "anonymous-user.png";
      }

      _CurrentImageInfo = new ImageInfo(imageRelativePath, imageName);
      _ImageUploadDir = imageRelativePath;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="imageFromUploadFullName">Имя картинки из директории Upload</param>
    public Image(string imageFromUploadFullName) : this(UploadRelativeDir, imageFromUploadFullName) { }

    private readonly ImageInfo _CurrentImageInfo;
    private readonly string _ImageUploadDir;

    /// <summary>
    /// Относительная директория картинок, помещающихся при загрузке
    /// </summary>
    public const string UploadRelativeDir = "Uploads\\Images";
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
    /// Сохранить картинку на сервер
    /// </summary>
    /// <param name="image">Картинка</param>
    /// <param name="server">Сервер. Необходимо для сохранения</param>
    /// <returns>Название картинки в относительном пути для сохранения</returns>
    public static string SaveImage(HttpPostedFileBase image, HttpServerUtilityBase server)
    {
      string fileName = String.Format("{0}_{1}", Guid.NewGuid().ToString(), System.IO.Path.GetExtension(image.FileName));
      string filePath = System.IO.Path.Combine(server.MapPath(String.Format("~/{0}", UploadRelativeDir)), fileName);
      image.SaveAs(filePath);

      return fileName;
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
      /// Полное имя и путь до изображения
      /// </summary>
      public string FullNameAndPath { get { return Path.Combine("\\", _ImageRelativePath, _ImageName); } }
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

    public class Validation
    { 
      private HttpPostedFileBase _Image { get; set; }
      /// <summary>
      /// Максимальная ширина
      /// </summary>
      private int _maxWidth { get; set; }
      /// <summary>
      /// Максимальная высота
      /// </summary>
      private int _maxHeight { get; set; }
      /// <summary>
      /// Доступные форматы для загрузки
      /// </summary>
      protected string[] valideFormats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };
      protected const int maxWidthConst = 1920;
      protected const int maxHeightConst = 1280;

      public Validation(HttpPostedFileBase Image)
      {
        _Image = Image;
        _maxWidth = maxWidthConst;
        _maxHeight = maxHeightConst;
      }

      public Validation(HttpPostedFileBase Image, int maxWidth, int maxHeight)
      {
        _Image = Image;
        _maxWidth = maxWidth;
        _maxHeight = maxHeight;
      }

      private bool isValideSizeof { get { return _Image.ContentLength >= 1 * _maxWidth * _maxHeight ? true : false; } }
      private bool isValieFormat { get { return valideFormats.Any(x => _Image.FileName.EndsWith(x, StringComparison.OrdinalIgnoreCase)); } }
      private bool isFileImageType { get { return _Image.ContentType.Contains("image"); } }

      public void ValidateImage()
      {
        if (!isFileImageType)
          throw new UserVisibleException("Файл не является картинкой");

        if (!isValieFormat)
          throw new UserVisibleException("Неправильный формат изображения");

        if (isValideSizeof)
          throw new UserVisibleException("Рамер фото больше допустимого");
      }
    }
  }
}