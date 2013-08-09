﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.SelfHost;
using NUnit.Framework;
using Rhino.Mocks;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Services;
using Umbraco.Tests.TestHelpers;
using Umbraco.Web;
using Umbraco.Web.WebApi;
using Umbraco.Web.WebApi.Filters;
using umbraco.presentation.channels.businesslogic;

namespace Umbraco.Tests.Controllers.WebApiEditors
{
    [TestFixture]
    public class EnsureUserPermissionForContentAttributeTests
    {
        [Test]
        public void Does_Not_Throw_Exception_When_Access_Allowed_By_Path()
        {
            //arrange
            var user = MockRepository.GenerateStub<IUser>();
            user.Id = 9;
            user.StartContentId = -1;
            var content = MockRepository.GenerateStub<IContent>();
            content.Path = "-1,1234,5678";
            var contentService = MockRepository.GenerateStub<IContentService>();
            contentService.Stub(x => x.GetById(1234)).Return(content);
            var userService = MockRepository.GenerateStub<IUserService>();
            var permissions = new List<EntityPermission>();
            userService.Stub(x => x.GetPermissions(user, 1234)).Return(permissions);
            var ctx = new HttpActionContext();
            ctx.ActionArguments.Add("id", 1234);
            var attribute = new EnsureUserPermissionForContentAttribute(user, userService, contentService, 1234, 'F');

            //act
            attribute.OnActionExecuting(ctx);

            //assert
            Assert.Pass();
        }

        [Test]
        public void Throws_Exception_When_No_Content_Found()
        {
            //arrange
            var user = MockRepository.GenerateStub<IUser>();
            user.Id = 9;
            user.StartContentId = -1;
            var content = MockRepository.GenerateStub<IContent>();
            content.Path = "-1,1234,5678";
            var contentService = MockRepository.GenerateStub<IContentService>();
            contentService.Stub(x => x.GetById(0)).Return(content);
            var userService = MockRepository.GenerateStub<IUserService>();
            var permissions = new List<EntityPermission>();
            userService.Stub(x => x.GetPermissions(user, 1234)).Return(permissions);
            var ctx = new HttpActionContext();
            var attribute = new EnsureUserPermissionForContentAttribute(user, userService, contentService, 1234, 'F');

            //act/assert
            Assert.Throws<HttpResponseException>(() => attribute.OnActionExecuting(ctx));
        }

        [Test]
        public void Throws_Exception_When_No_Access_By_Path()
        {
            //arrange
            var user = MockRepository.GenerateStub<IUser>();
            user.Id = 9;
            user.StartContentId = 9876;
            var content = MockRepository.GenerateStub<IContent>();
            content.Path = "-1,1234,5678";
            var contentService = MockRepository.GenerateStub<IContentService>();
            contentService.Stub(x => x.GetById(1234)).Return(content);
            var userService = MockRepository.GenerateStub<IUserService>();
            var permissions = new List<EntityPermission>();
            userService.Stub(x => x.GetPermissions(user, 1234)).Return(permissions);
            var ctx = new HttpActionContext();
            var attribute = new EnsureUserPermissionForContentAttribute(user, userService, contentService, 1234, 'F');

            //act/assert
            Assert.Throws<HttpResponseException>(() => attribute.OnActionExecuting(ctx));
        }

        [Test]
        public void Throws_Exception_When_No_Access_By_Permission()
        {
            //arrange
            var user = MockRepository.GenerateStub<IUser>();
            user.Id = 9;
            user.StartContentId = -1;
            var content = MockRepository.GenerateStub<IContent>();
            content.Path = "-1,1234,5678";
            var contentService = MockRepository.GenerateStub<IContentService>();
            contentService.Stub(x => x.GetById(1234)).Return(content);
            var userService = MockRepository.GenerateStub<IUserService>();
            var permissions = new List<EntityPermission>
                {
                    new EntityPermission(9, 1234, new string[]{ "A", "B", "C" })
                };
            userService.Stub(x => x.GetPermissions(user, 1234)).Return(permissions);
            var ctx = new HttpActionContext();
            var attribute = new EnsureUserPermissionForContentAttribute(user, userService, contentService, 1234, 'F');

            //act/assert
            Assert.Throws<HttpResponseException>(() => attribute.OnActionExecuting(ctx));
        }

        [Test]
        public void Does_Not_Throw_Exception_When_Access_Allowed_By_Permission()
        {
            //arrange
            var user = MockRepository.GenerateStub<IUser>();
            user.Id = 9;
            user.StartContentId = -1;
            var content = MockRepository.GenerateStub<IContent>();
            content.Path = "-1,1234,5678";
            var contentService = MockRepository.GenerateStub<IContentService>();
            contentService.Stub(x => x.GetById(1234)).Return(content);
            var userService = MockRepository.GenerateStub<IUserService>();
            var permissions = new List<EntityPermission>
                {
                    new EntityPermission(9, 1234, new string[]{ "A", "F", "C" })
                };
            userService.Stub(x => x.GetPermissions(user, 1234)).Return(permissions);
            var ctx = new HttpActionContext();
            ctx.ActionArguments.Add("id", 1234);
            var attribute = new EnsureUserPermissionForContentAttribute(user, userService, contentService, 1234, 'F');

            //act
            attribute.OnActionExecuting(ctx);

            //assert
            Assert.Pass();
        }

        [Test]
        public void Does_Not_Throw_Exception_When_No_Permissions_Assigned()
        {
            //arrange
            var user = MockRepository.GenerateStub<IUser>();
            user.Id = 9;
            user.StartContentId = -1;
            var content = MockRepository.GenerateStub<IContent>();
            content.Path = "-1,1234,5678";
            var contentService = MockRepository.GenerateStub<IContentService>();
            contentService.Stub(x => x.GetById(1234)).Return(content);
            var userService = MockRepository.GenerateStub<IUserService>();
            var permissions = new List<EntityPermission>();
            userService.Stub(x => x.GetPermissions(user, 1234)).Return(permissions);
            var ctx = new HttpActionContext();
            ctx.ActionArguments.Add("id", 1234);
            var attribute = new EnsureUserPermissionForContentAttribute(user, userService, contentService, 1234, 'F');

            //act
            attribute.OnActionExecuting(ctx);

            //assert
            Assert.Pass();
        }

    }

    //we REALLY need a way to nicely mock the service context, etc... so we don't have to do integration tests... coming soon.

    //NOTE: The below self hosted stuff does work so need to get some tests written. Some are not possible atm because
    // of the legacy SQL calls like checking permissions.

    //[TestFixture]
    //public class ContentControllerHostedTests : BaseRoutingTest
    //{

    //    protected override DatabaseBehavior DatabaseTestBehavior
    //    {
    //        get { return DatabaseBehavior.NoDatabasePerFixture; }
    //    }

    //    public override void TearDown()
    //    {
    //        base.TearDown();
    //        UmbracoAuthorizeAttribute.Enable = true;
    //        UmbracoApplicationAuthorizeAttribute.Enable = true;
    //    }

    //    /// <summary>
    //    /// Tests to ensure that the response filter works so that any items the user
    //    /// doesn't have access to are removed
    //    /// </summary>
    //    [Test]
    //    public async void Get_By_Ids_Response_Filtered()
    //    {
    //        UmbracoAuthorizeAttribute.Enable = false;
    //        UmbracoApplicationAuthorizeAttribute.Enable = false;

    //        var baseUrl = string.Format("http://{0}:9876", Environment.MachineName);
    //        var url = baseUrl + "/api/Content/GetByIds?ids=1&ids=2";

    //        var routingCtx = GetRoutingContext(url, 1234, null, true);

    //        var config = new HttpSelfHostConfiguration(baseUrl);
    //        using (var server = new HttpSelfHostServer(config))
    //        {
    //            var route = config.Routes.MapHttpRoute("test", "api/Content/GetByIds",
    //            new
    //            {
    //                controller = "Content",
    //                action = "GetByIds",
    //                id = RouteParameter.Optional
    //            });
    //            route.DataTokens["Namespaces"] = new string[] { "Umbraco.Web.Editors" };

    //            var client = new HttpClient(server);

    //            var request = new HttpRequestMessage
    //            {
    //                RequestUri = new Uri(url),
    //                Method = HttpMethod.Get
    //            };

    //            var result = await client.SendAsync(request);
    //        }
             
    //    }

    //}
}
