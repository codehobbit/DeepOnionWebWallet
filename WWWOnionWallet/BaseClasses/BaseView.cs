using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using WWWOnionWallet.Authentication;

namespace WWWOnionWallet.BaseClasses
{
    public abstract class BaseView<TModel> : WebViewPage<TModel>
    {
        protected AppUser CurrentUser
        {
            get
            {
                return new AppUser(this.User as ClaimsPrincipal);
            }
        }
    }

    public abstract class AppViewPage : BaseView<dynamic>
    {
    }
}