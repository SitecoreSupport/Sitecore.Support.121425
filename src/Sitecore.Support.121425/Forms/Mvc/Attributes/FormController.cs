using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Forms.Mvc.Controllers;
using Sitecore.Forms.Mvc.Data.Wrappers;
using Sitecore.Forms.Mvc.Models;
using Sitecore.Forms.Mvc.ViewModels;
using Sitecore.WFFM.Abstractions.Actions;
using System;
using System.Web.Mvc;

namespace Sitecore.Support.Forms.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class FormErrorHandlerAttribute : HandleErrorAttribute
    {
        private readonly IRenderingContext renderingContext;

        public FormErrorHandlerAttribute() : this((IRenderingContext)Factory.CreateObject(Sitecore.Forms.Mvc.Constants.FormRenderingContext, true))
        {
        }

        public FormErrorHandlerAttribute(IRenderingContext renderingContext)
        {
            Assert.ArgumentNotNull(renderingContext, "renderingContext");
            this.renderingContext = renderingContext;
        }

        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                Guid uniqueId = this.renderingContext.Rendering.UniqueId;
                string str = filterContext.RequestContext.HttpContext.Request.Form[FormViewModel.GetClientId(uniqueId) + ".Id"];
                if (!string.IsNullOrEmpty(str))
                {
                    Log.Error(filterContext.Exception.Message, filterContext.Exception, this);
                    FormController controller = filterContext.Controller as FormController;
                    FormErrorResult<FormModel, FormViewModel> result = null;
                    if (controller != null)
                    {
                        ExecuteResult.Failure failure = new ExecuteResult.Failure
                        {
                            ErrorMessage = filterContext.Exception.Message,
                            StackTrace = filterContext.Exception.StackTrace,
                            IsCustom = false
                        };
                        result = new FormErrorResult<FormModel, FormViewModel>(controller.FormRepository, controller.Mapper, controller.FormProcessor, failure)
                        {
                            ViewData = controller.ViewData,
                            TempData = controller.TempData,
                            ViewEngineCollection = controller.ViewEngineCollection
                        };
                    }
                    if (result != null)
                    {
                        filterContext.Result = result;
                    }
                    else
                    {
                        filterContext.Result = new EmptyResult();
                    }
                    filterContext.ExceptionHandled = true;
                }
            }

        }
    }
}
