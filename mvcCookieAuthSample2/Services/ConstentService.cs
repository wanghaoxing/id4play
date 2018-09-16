using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using mvcCookieAuthSample.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcCookieAuthSample.Services
{
    public class ConstentService
    {
        private readonly IClientStore _icelientStore;
        private readonly IResourceStore _resourceStore;
        private readonly IIdentityServerInteractionService _identityServerInteractionService;

        #region  私有

        private ConsentViewModel CreateViewModel(AuthorizationRequest request, Client client, Resources resource,InputConstentViewModel model)
        {
            var selectedScopes = model?.ScopesConsented ?? Enumerable.Empty<string>();
            var vm = new ConsentViewModel();
            vm.ClientLogoUrl = client.LogoUri;
            vm.ClientName = client.ClientName;
            vm.ClientUrl = client.ClientUri;
            vm.RemeberConsent = model?.RememberConsent??true;


            vm.IdentityScopes = resource.IdentityResources.Select(i => CreateScopeViewModel(i, selectedScopes.Contains(i.Name)||model==null));
            vm.ResourceScopes = resource.ApiResources.SelectMany(r => r.Scopes).Select(x => CreateScopeViewModel(x, selectedScopes.Contains(x.Name)||model==null));
            return vm;
        }

        private ScopeViewModel CreateScopeViewModel(IdentityResource identityResource,bool check)
        {
            return new ScopeViewModel
            {
                Name = identityResource.Name,
                DisplayName = identityResource.DisplayName,
                Description = identityResource.Description,
                Checked = identityResource.Required||check,
                Required = identityResource.Required,
            };
        }

        private ScopeViewModel CreateScopeViewModel(Scope scope,bool check)
        {
            return new ScopeViewModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                Checked = scope.Required||check,
                Required = scope.Required,
            };
        }
        #endregion

        public async Task<ConsentViewModel> BuildConsentViewModel(string returnUrl,InputConstentViewModel model=null)
        {
            var request = await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);
            if (request == null)
            {
                return null;
            }
            var client = await _icelientStore.FindEnabledClientByIdAsync(request.ClientId);
            var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);
            var vm = CreateViewModel(request, client, resources,model);
            vm.ReturnUrl = returnUrl;
            return vm;


        }


        public async Task<ProcessConsentResult> ProcessConsent(InputConstentViewModel viewModel)
        {
            ConsentResponse consentResponse = null;

            var result = new ProcessConsentResult();
            if (viewModel.Button == "no")
            {
                consentResponse = ConsentResponse.Denied;
            }
            else if (viewModel.Button == "yes")
            {
                if (viewModel.ScopesConsented != null && viewModel.ScopesConsented.Any())
                {
                    consentResponse = new ConsentResponse
                    {
                        ScopesConsented = viewModel.ScopesConsented,
                        RememberConsent = viewModel.RememberConsent
                    };
                }
                result.ValidationError = "请至少选择一个权限";
            }

            if (consentResponse != null)
            {
                var request = await _identityServerInteractionService.GetAuthorizationContextAsync(viewModel.ReturnUrl);
                await _identityServerInteractionService.GrantConsentAsync(request, consentResponse);

                result.RedirectUrl = viewModel.ReturnUrl;
            }
            else
            {
                var consentModel =await  BuildConsentViewModel(viewModel.ReturnUrl);
                result.ConsentViewModel = consentModel;
            }
            return result;
        }
        public ConstentService(
            IClientStore icelientStore,
            IResourceStore resourceStore,
            IIdentityServerInteractionService identityServerInteractionService
            )
        {
            _icelientStore = icelientStore;

            _resourceStore = resourceStore;
            _identityServerInteractionService = identityServerInteractionService;
        }
    }
}
