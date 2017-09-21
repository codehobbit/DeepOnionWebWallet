using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace WWWOnionWallet.Authentication
{
    public class AppUser : ClaimsPrincipal
    {
        public AppUser(ClaimsPrincipal principal)
            : base(principal)
        {
        }

        public string AccountName
        {
            get
            {
                return this.FindFirst("AccountName").Value;
            }
        }

        public string OnionAddress
        {
            get
            {
                return this.FindFirst("OnionAddress").Value;
            }
        }

        public string Email
        {
            get
            {
                return this.FindFirst("Email").Value;
            }
        }
    }
}