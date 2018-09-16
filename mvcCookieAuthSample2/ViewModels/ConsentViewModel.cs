using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcCookieAuthSample.ViewModels
{
    public class ConsentViewModel:InputConstentViewModel
    {
        public string CleintId { get; set; }
        public string ClientName { get; set; }

        public string ClientUrl { get; set; }
        public string ClientLogoUrl { get; set; }

        public bool RemeberConsent { get; set; }

        public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }

        public IEnumerable<ScopeViewModel> ResourceScopes { get; set; }

        public string ReturnUrl { get; set; }
    }
}
