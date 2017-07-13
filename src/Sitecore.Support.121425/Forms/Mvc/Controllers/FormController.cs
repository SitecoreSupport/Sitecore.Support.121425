using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Forms.Mvc;
using Sitecore.Forms.Mvc.Attributes;
using Sitecore.Forms.Mvc.Controllers;
using Sitecore.Forms.Mvc.Controllers.Filters;
using Sitecore.Forms.Mvc.Controllers.ModelBinders;
using Sitecore.Forms.Mvc.Interfaces;
using Sitecore.Forms.Mvc.Models;
using Sitecore.Forms.Mvc.ViewModels;
using Sitecore.Mvc.Controllers;
using Sitecore.WFFM.Abstractions;
using System.IO;
using System.Web.Mvc;
namespace Sitecore.Support.Forms.Mvc.Controllers
{
    [ModelBinder(typeof(FormModelBinder))]
    public class FormController : SitecoreController
    {
        // Fields
        private readonly IFormProcessor<FormModel> formProcessor;
        private readonly IRepository<FormModel> formRepository;
        private readonly IAutoMapper<FormModel, FormViewModel> mapper;

        // Methods
        public FormController() : this((IRepository<FormModel>)Factory.CreateObject(Sitecore.Forms.Mvc.Constants.FormRepository, true), (IAutoMapper<FormModel, FormViewModel>)Factory.CreateObject(Sitecore.Forms.Mvc.Constants.FormAutoMapper, true), (IFormProcessor<FormModel>)Factory.CreateObject(Sitecore.Forms.Mvc.Constants.FormProcessor, true))
        {
        }

        public FormController(IRepository<FormModel> repository, IAutoMapper<FormModel, FormViewModel> mapper, IFormProcessor<FormModel> processor)
        {
            Assert.ArgumentNotNull(repository, "repository");
            Assert.ArgumentNotNull(mapper, "mapper");
            Assert.ArgumentNotNull(processor, "processor");
            this.formRepository = repository;
            this.mapper = mapper;
            this.formProcessor = processor;
        }

        public virtual FormResult<FormModel, FormViewModel> Form() =>
            new FormResult<FormModel, FormViewModel>(this.FormRepository, this.Mapper)
            {
                ViewData = base.ViewData,
                TempData = base.TempData,
                ViewEngineCollection = base.ViewEngineCollection
            };

        [FormErrorHandler, HttpGet]
        public override ActionResult Index() =>
            this.Form();

        [FormErrorHandler, SubmittedFormHandler, HttpPost]
        public virtual ActionResult Index([ModelBinder(typeof(FormModelBinder))] FormViewModel formViewModel) =>
            this.ProcessedForm(formViewModel, "");

        [FormErrorHandler, AllowCrossSiteJson]
        public virtual JsonResult Process([ModelBinder(typeof(FormModelBinder))] FormViewModel formViewModel)
        {
            string str;
            DependenciesManager.AnalyticsTracker.InitializeTracker();
            ProcessedFormResult<FormModel, FormViewModel> result = this.ProcessedForm(formViewModel, "~/Views/Form/Index.cshtml");
            result.ExecuteResult(base.ControllerContext);
            using (StringWriter writer = new StringWriter())
            {
                ViewContext viewContext = new ViewContext(base.ControllerContext, result.View, base.ViewData, base.TempData, writer);
                result.View.Render(viewContext, writer);
                str = writer.GetStringBuilder().ToString();
            }
            base.ControllerContext.HttpContext.Response.Clear();
            return new JsonResult { Data = str };
        }

        public virtual ProcessedFormResult<FormModel, FormViewModel> ProcessedForm(FormViewModel viewModel, string viewName = "")
        {
            ProcessedFormResult<FormModel, FormViewModel> result = new ProcessedFormResult<FormModel, FormViewModel>(this.FormRepository, this.Mapper, this.FormProcessor, viewModel)
            {
                ViewData = base.ViewData,
                TempData = base.TempData,
                ViewEngineCollection = base.ViewEngineCollection
            };
            if (!string.IsNullOrEmpty(viewName))
            {
                result.ViewName = viewName;
            }
            return result;
        }

        // Properties
        public IFormProcessor<FormModel> FormProcessor =>
            this.formProcessor;

        public IRepository<FormModel> FormRepository =>
            this.formRepository;

        public IAutoMapper<FormModel, FormViewModel> Mapper =>
            this.mapper;
    }
}