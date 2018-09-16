using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using mvcCookieAuthSample.Services;
using mvcCookieAuthSample.ViewModels;

namespace mvcCookieAuthSample.Controllers
{
    public class ConsentController : Controller
    {

        private readonly ConstentService _service;
        public ConsentController(ConstentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var model = await _service.BuildConsentViewModel(returnUrl);

            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Index(InputConstentViewModel viewModel)
        {
            var result = await _service.ProcessConsent(viewModel);
            if (result.IsRedirect)
            {
                return Redirect(result.RedirectUrl);
            }
            else
            {
                if (!string.IsNullOrEmpty(result.ValidationError))
                {
                    ModelState.AddModelError("", result.ValidationError);
                }
                return View(result.ConsentViewModel);
            }


        }
    }
}