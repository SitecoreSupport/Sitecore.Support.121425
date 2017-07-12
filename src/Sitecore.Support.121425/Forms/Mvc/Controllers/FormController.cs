using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Forms.Mvc.Attributes;
using Sitecore.Forms.Mvc.Controllers.Filters;
using Sitecore.Forms.Mvc.Controllers.ModelBinders;
using Sitecore.Forms.Mvc.Interfaces;
using Sitecore.Forms.Mvc.Models;
using Sitecore.Forms.Mvc.ViewModels;
using Sitecore.Mvc.Controllers;
using Sitecore.WFFM.Abstractions.Dependencies;
using Sitecore.WFFM.Abstractions.Shared;
using System;
using System.IO;
using System.Web.Mvc;


namespace Sitecore.Support.Forms.Mvc.Controllers
{
  [ModelBinder(typeof(FormModelBinder))]
  public class FormController : Sitecore.Forms.Mvc.Controllers.FormController
  {

    private readonly IAnalyticsTracker analyticsTracker;
    public new IRepository<FormModel> FormRepository
    {
      get;
      private set;
    }

    public new IAutoMapper<FormModel, FormViewModel> Mapper
    {
      get;
      private set;
    }

    public new IFormProcessor<FormModel> FormProcessor
    {
      get;
      private set;
    }

    public FormController() : this((IRepository<FormModel>)Factory.CreateObject(Sitecore.Forms.Mvc.Constants.FormRepository, true), (IAutoMapper<FormModel, FormViewModel>)Factory.CreateObject(Sitecore.Forms.Mvc.Constants.FormAutoMapper, true), (IFormProcessor<FormModel>)Factory.CreateObject(Sitecore.Forms.Mvc.Constants.FormProcessor, true), DependenciesManager.AnalyticsTracker)
    {
    }

    [Obsolete]
    public FormController(IRepository<FormModel> repository, IAutoMapper<FormModel, FormViewModel> mapper, IFormProcessor<FormModel> processor) : this(repository, mapper, processor, DependenciesManager.AnalyticsTracker)
    {
    }

    public FormController(IRepository<FormModel> repository, IAutoMapper<FormModel, FormViewModel> mapper, IFormProcessor<FormModel> processor, IAnalyticsTracker analyticsTracker)
    {
      Assert.ArgumentNotNull(repository, "repository");
      Assert.ArgumentNotNull(mapper, "mapper");
      Assert.ArgumentNotNull(processor, "processor");
      Assert.ArgumentNotNull(analyticsTracker, "analyticsTracker");
      this.FormRepository = repository;
      this.Mapper = mapper;
      this.FormProcessor = processor;
      this.analyticsTracker = analyticsTracker;
    }
    [FormErrorHandler, SubmittedFormHandler, Sitecore.Support.Forms.Mvc.Controllers.Filters.WffmValidateAntiForgeryToken, HttpPost]
    public override ActionResult Index([ModelBinder(typeof(FormModelBinder))] FormViewModel formViewModel)
    {
      this.analyticsTracker.InitializeTracker();
      return this.ProcessedForm(formViewModel, "");
    }

  }
}