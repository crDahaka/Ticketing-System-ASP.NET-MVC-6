﻿namespace TicketingSystem.Web.Controllers
{
    using Data;
    using Models;
    using System.Web.Mvc;
    using System;
    using System.Linq;
    using System.Web.Routing;
    using Microsoft.AspNet.Identity;

    [HandleError]
    public class BaseController : Controller
    {
        protected ITicketSystemData Data { get; private set; }

        protected User UserProfile { get; private set; }

        public BaseController(ITicketSystemData data)
        {
            this.Data = data;
        }

        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {

            this.UserProfile = this.Data.Users.All().Where(u => u.UserName == requestContext.HttpContext.User.Identity.Name).FirstOrDefault();
            return base.BeginExecute(requestContext, callback, state);
        }
    }
}