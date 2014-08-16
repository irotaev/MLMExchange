using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MLMExchange.Lib.Exception;

namespace MLMExchange.Controllers
{
  public class ResourcesController : BaseController
  {
    #region Текстовые ресурсы
    /// <summary>
    /// Получить текстовые ресурсы
    /// </summary>
    /// <param name="requests">Запрос на ресурс</param>
    /// <returns>Значение запрашиваемых ресурсов</returns>
    [HttpPost] // Используется POST, т.к. запрос из вложенных объектов
    public JsonResult GetTextResources(List<_TextResourceRequest> requests)
    {
      if (!Request.IsAjaxRequest())
        throw new UserVisible__ActionAjaxOnlyException();

      if (requests == null || requests.Count() == 0)
        throw new UserVisible__WrongParametrException("requests");

      List<_TextResourceResponse> response = new List<_TextResourceResponse>();

      foreach (var request in requests)
        response.Add(GetResource(request));

      return Json(response, JsonRequestBehavior.AllowGet);
    }

    private _TextResourceResponse GetResource(_TextResourceRequest request)
    {
      _TextResourceResponse response = new _TextResourceResponse 
      { 
        ResourceId = String.Format("{0}.{1}.{2}", request.ProjectNamespace, request.ResourceNamespace, request.ResourceId), 
        ResourceValue = null 
      };

      switch (request.ProjectNamespace)
      {
        case "MLMExchange":
          switch (request.ResourceNamespace)
          {
            case "PrivateResource":
              try
              {
                var resource = new MLMExchange.Properties.PrivateResource();
                response.ResourceValue = (string)resource.GetType().GetProperty(request.ResourceId).GetValue(resource);
              }
              catch { }
              break;
            case "ResourcesA":
              try
              {
                var resource = new MLMExchange.Properties.ResourcesA();
                response.ResourceValue = (string)resource.GetType().GetProperty(request.ResourceId).GetValue(resource);
              }
              catch { }
              break;
          }
          break;
          
        case "Logic":
          switch (request.ResourceNamespace)
          {
            case "GeneralResources":
              try
              {
                response.ResourceValue = (string)typeof(Logic.Properties.GeneralResources).GetProperty(request.ResourceId).GetValue(null);
              }
              catch { }
              break;
            case "PrivateResources":
              try
              {
                response.ResourceValue = (string)typeof(Logic.Properties.PrivateResources).GetType().GetProperty(request.ResourceId).GetValue(null);
              }
              catch { }
              break;
          }
          break;
      }

      return response;
    }

    /// <summary>
    /// Формат запроса текстового ресурса
    /// </summary>
    public class _TextResourceRequest
    {
      public string ProjectNamespace { get; set; }
      public string ResourceNamespace { get; set; }
      public string ResourceId { get; set; }
    }

    /// <summary>
    /// Ответ на запрос о ресурсе
    /// </summary>
    [Serializable]
    public class _TextResourceResponse
    {
      public string ResourceId { get; set; }
      public string ResourceValue { get; set; }
    }
    #endregion
  }
}