using LogicTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLMExchange.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MLMExchangeTest.Controllers
{
  [TestClass]
  public class ResourcesControllerTest : AbstractTest
  {
    [TestMethod]
    public void GetTextResources()
    {
      var request = new Mock<HttpRequestBase>();

      request.SetupGet(x => x.Headers).Returns(
        new System.Net.WebHeaderCollection 
        {
            {"X-Requested-With", "XMLHttpRequest"}
        }
      );

      var context = new Mock<HttpContextBase>();
      context.SetupGet(x => x.Request).Returns(request.Object);

      ResourcesController controller = new ResourcesController();
      controller.ControllerContext = new ControllerContext(context.Object, new System.Web.Routing.RouteData(), controller);

      var response = controller.GetTextResources(
        new ResourcesController._TextResourceRequest { ProjectNamespace = "MLMExchange", ResourceNamespace = "PrivateResource", ResourceId = "PayYieldTradingSession" },
        new ResourcesController._TextResourceRequest { ProjectNamespace = "MLMExchange", ResourceNamespace = "ResourcesA", ResourceId = "AddMyCrypt" },
        new ResourcesController._TextResourceRequest { ProjectNamespace = "Logic", ResourceNamespace = "GeneralResources", ResourceId = "ElectronicName" },
        new ResourcesController._TextResourceRequest { ProjectNamespace = "Logic", ResourceNamespace = "PrivateResources", ResourceId = "Ssms__Exception_ServerResponseParse" },
        new ResourcesController._TextResourceRequest { ProjectNamespace = "Logic_fake", ResourceNamespace = "PrivateResources", ResourceId = "Ssms__Exception_ServerResponseParse" },
        new ResourcesController._TextResourceRequest { ProjectNamespace = "Logic", ResourceNamespace = "PrivateResources_fake", ResourceId = "Ssms__Exception_ServerResponseParse" },
        new ResourcesController._TextResourceRequest { ProjectNamespace = "Logic", ResourceNamespace = "PrivateResources", ResourceId = "Fake_0" }
        );

      Assert.IsTrue(response != null && response.Data != null, "Выдан пустой ответ");
      Assert.IsTrue(((List<ResourcesController._TextResourceResponse>)response.Data).ElementAt(0).ResourceValue != null, "Не получено зеначение по задонному Id ресурса");
      Assert.IsTrue(((List<ResourcesController._TextResourceResponse>)response.Data).ElementAt(1).ResourceValue != null, "Не получено зеначение по задонному Id ресурса");
      Assert.IsTrue(((List<ResourcesController._TextResourceResponse>)response.Data).ElementAt(2).ResourceValue != null, "Не получено зеначение по задонному Id ресурса");
      Assert.IsTrue(((List<ResourcesController._TextResourceResponse>)response.Data).ElementAt(3).ResourceValue == null, "Получено ненулевое зеначение по задонному Id ресурса");
      Assert.IsTrue(((List<ResourcesController._TextResourceResponse>)response.Data).ElementAt(4).ResourceValue == null, "Получено ненулевое зеначение по задонному Id ресурса");
      Assert.IsTrue(((List<ResourcesController._TextResourceResponse>)response.Data).ElementAt(5).ResourceValue == null, "Получено ненулевое зеначение по задонному Id ресурса");
      Assert.IsTrue(((List<ResourcesController._TextResourceResponse>)response.Data).ElementAt(6).ResourceValue == null, "Получено ненулевое зеначение по задонному Id ресурса");
    }
  }
}
