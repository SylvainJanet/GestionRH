﻿using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using MiseEnSituation.Services;
using MiseEnSituation.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MiseEnSituation.Filters
{
    public class JWTFilterAttribute : ActionFilterAttribute
    {
        private MyDbContext db = new MyDbContext();
        private UserService userService;

        public JWTFilterAttribute()
        {
            userService = new UserService(new UserRepository(db));
        }


        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var tokenWithBearer = actionContext.Request.Headers.Authorization;
            var token = tokenWithBearer.ToString().Substring(7);
            //Trace.WriteLine("TOKEN = " + token);
            string userToken = TokenManager.ValidateToken(token.ToString());

            var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Invalid Token"),
                ReasonPhrase = "Token Error"
            };
            //Verif du token
            if (userToken == null)
            {

                throw new HttpResponseException(resp);
            }
            else
            {
                User u = userService.FindByEmail(userToken);
                
                if (u == null || (u != null && !u.Email.Equals(userToken)))
                {
                    throw new HttpResponseException(resp);
                }
            }
        }
    }
}