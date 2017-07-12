using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Forms.Mvc.Data.Wrappers;
using Sitecore.Forms.Mvc.ViewModels;
using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Sitecore.Support.Forms.Mvc.Controllers.Filters
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  public sealed class WffmValidateAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
  {
    public IRenderingContext RenderingContext
    {
      get;
      set;
    }

    public WffmValidateAntiForgeryTokenAttribute() : this((IRenderingContext)Factory.CreateObject(Sitecore.Forms.Mvc.Constants.FormRenderingContext, true))
    {
    }

    public WffmValidateAntiForgeryTokenAttribute(IRenderingContext renderingContext)
    {
      Assert.ArgumentNotNull(renderingContext, "renderingContext");
      this.RenderingContext = renderingContext;
    }

    public void OnAuthorization(AuthorizationContext filterContext)
    {
      if(Settings.GetBoolSetting("WFM.EnableAntiCsrf", true))
      {
        Guid uniqueId = this.RenderingContext.Rendering.UniqueId;
        string value = filterContext.RequestContext.HttpContext.Request.Form[FormViewModel.GetClientId(uniqueId) + ".Id"];
        if (string.IsNullOrEmpty(value))
        {
          return;
        }
        if (AjaxRequestExtensions.IsAjaxRequest(filterContext.HttpContext.Request))
        {
          this.ValidateRequestHeader(filterContext.HttpContext.Request);
          return;
        }
        AntiForgery.Validate();
      }
    }

    private void ValidateRequestHeader(HttpRequestBase request)
    {
      if (Settings.GetBoolSetting("WFM.EnableAntiCsrf", true))
      {
        NameValueCollection headers = request.Headers;
        HttpCookie httpCookie = request.Cookies[AntiForgeryConfig.CookieName];
        AntiForgery.Validate((httpCookie != null) ? httpCookie.Value : null, headers["X-RequestVerificationToken"]);
      }
    }
  }
}
