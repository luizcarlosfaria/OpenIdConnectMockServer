// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;

namespace IdentityServerHost.Quickstart.UI
{
    public class ProcessConsentResult
    {
        public bool IsRedirect => this.RedirectUri != null;
        public string RedirectUri { get; set; }
        public Client Client { get; set; }

        public bool ShowView => this.ViewModel != null;
        public ConsentViewModel ViewModel { get; set; }

        public bool HasValidationError => this.ValidationError != null;
        public string ValidationError { get; set; }
    }
}
