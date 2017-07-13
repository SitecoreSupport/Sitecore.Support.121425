using Sitecore.Diagnostics;
using Sitecore.Forms.Mvc;
using Sitecore.Forms.Mvc.Controllers.ModelBinders;
using Sitecore.Forms.Mvc.Data.Wrappers;
using Sitecore.Forms.Mvc.Models;
using Sitecore.Forms.Mvc.ViewModels;
using System;
using System.Web.Mvc;
using System.Web.SessionState;

namespace Sitecore.Support.Forms.Mvc.Controllers.ModelBinders
{
    public class FormModelBinder : DefaultFormModelBinder
    {
        public FormModelBinder()
        {
        }

        public FormModelBinder(IRenderingContext renderingContext) : base(renderingContext)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Guid guid;
            Assert.ArgumentNotNull(controllerContext, "controllerContext");
            Assert.ArgumentNotNull(bindingContext, "bindingContext");
            if (!base.RenderingContext.Rendering.IsFormRendering)
            {
                return null;
            }
            Guid uniqueId = base.RenderingContext.Rendering.UniqueId;
            string prefix = this.GetPrefix(uniqueId);
            if (string.IsNullOrEmpty(prefix))
            {
                return null;
            }
            ValueProviderResult result = bindingContext.ValueProvider.GetValue(prefix + "." + Sitecore.Forms.Mvc.Constants.Id);
            if (result == null)
            {
                return null;
            }
            if (!Guid.TryParse(result.AttemptedValue, out guid) || ((uniqueId != Guid.Empty) && (guid != uniqueId)))
            {
                return null;
            }
            bindingContext.ModelName = prefix;
            return base.BindModel(controllerContext, bindingContext);
        }
        


        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            return this.GetFormViewModel(controllerContext) ?? base.CreateModel(controllerContext, bindingContext, modelType);
        }

        public override FormViewModel GetFormViewModel(ControllerContext controllerContext)
        {
            Assert.ArgumentNotNull(controllerContext, "controllerContext");
            if (base.RenderingContext?.Rendering == null)
            {
                return null;
            }
            Guid uniqueId = base.RenderingContext.Rendering.UniqueId;
            if ((controllerContext.HttpContext.Session != null) && (controllerContext.HttpContext.Session.Mode == SessionStateMode.InProc))
            {
                FormViewModel model3 = controllerContext.HttpContext.Session[uniqueId.ToString()] as FormViewModel;
                if (model3 != null)
                {
                    model3.SuccessSubmit = false;
                    return model3;
                }
            }
            FormController controller = controllerContext.Controller as FormController;
            FormModel model = controller?.FormRepository.GetModel(uniqueId);
            return ((model != null) ? controller.Mapper.GetView(model) : null);
        }

    }
}
