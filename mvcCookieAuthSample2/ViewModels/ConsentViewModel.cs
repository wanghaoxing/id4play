using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcCookieAuthSample.ViewModels
{
    public class ConsentViewModel
    {
        public string CleintId { get; set; }
        public string CleintName { get; set; }

        public string ClientUrl { get; set; }
        public string CleintLogoUrl { get; set; }

        public bool AllowRemeberConsent { get; set; }

        public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }

        public IEnumerable<ScopeViewModel> ResourceScopes { get; set; }
    }
}
