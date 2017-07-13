using Sitecore.Diagnostics;
using Sitecore.Forms.Mvc.Interfaces;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Sitecore.Support.Forms.Mvc.Controllers.Filters
{
    public class SubmittedFormHandler : ActionFilterAttribute
    {
      public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Assert.ArgumentNotNull(filterContext, "filterContext");
            FormController controller = filterContext.Controller as FormController;
            if ((controller != null) && !(filterContext.ActionParameters.Values.First<object>() is IViewModel))
            {
                filterContext.Result = controller.Form();
            }
        }



    }
}
