using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using mvcCookieAuthSample.ViewModels;

namespace mvcCookieAuthSample.Controllers
{
    public class ConsentController : Controller
    {

        private readonly IClientStore _icelientStore;
        private readonly IResourceStore _resourceStore;
        private readonly IIdentityServerInteractionService _identityServerInteractionService;
        public ConsentController(
            IClientStore icelientStore,
            IResourceStore resourceStore,
            IIdentityServerInteractionService identityServerInteractionService
            )
        {
            _icelientStore = icelientStore;

            _resourceStore = resourceStore;
            _identityServerInteractionService = identityServerInteractionService;
        }

        private async Task<ConsentViewModel> BuildConsentViewModel(string returnUrl)
        {
            var request = await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);
            if (request == null)
            {
                return null;
            }
            var client = await _icelientStore.FindEnabledClientByIdAsync(request.ClientId);
            var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);
            return CreateViewModel(request, client, resources);


        }

        private ConsentViewModel CreateViewModel(AuthorizationRequest request, Client client, Resources resource)
        {
            var vm = new ConsentViewModel();
            vm.CleintLogoUrl = client.LogoUri;
            vm.CleintName = client.ClientName;
            vm.ClientUrl = client.ClientUri;
            vm.AllowRemeberConsent = client.AllowRememberConsent;
            vm.IdentityScopes = resource.IdentityResources.Select(i => CreateScopeViewModel(i));
            vm.ResourceScopes = resource.ApiResources.SelectMany(r => r.Scopes).Select(x => CreateScopeViewModel(x));
            return vm;
        }

        private ScopeViewModel CreateScopeViewModel(IdentityResource identityResource)
        {
            return new ScopeViewModel
            {
                Name = identityResource.Name,
                DisplayName = identityResource.DisplayName,
                Description = identityResource.Description,
                Checked = identityResource.Required,
                Required = identityResource.Required,
            };
        }

        private ScopeViewModel CreateScopeViewModel(Scope scope )
        {
            return new ScopeViewModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                Checked = scope.Required,
                Required = scope.Required,
            };
        }
        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var model = await BuildConsentViewModel(returnUrl);

            return View(model);
        }
    }
}