using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using WebApi.Controllers;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Accounts;
using WebApi.Services;

namespace WebApiTests
{
    public class AccountServiceTest
    {
        [Fact]
        public async Task AuthenticateRequest()
        {
            // Arrange
            // var authReq = new Mock<AuthenticateRequest>();
            var authReq = new AuthenticateRequest();
            var authResp = new AuthenticateResponse(){ Email = "abc@email.com", RefreshToken = "rfrshToken" };

            var mockRepo = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();
            var dtCtxt = new Mock<DataContext>();

            // var appSttgs = new Mock<AppSettings>();
            var appSttgs = new Mock<IOptions<AppSettings>>();
            var emlSrvc = new Mock<EmailService>();

            var acctSrvc = new AccountService(dtCtxt.Object, mockMapper.Object, appSttgs.Object, emlSrvc.Object);
            dtCtxt.Setup(ctx => ctx.Accounts.SingleOrDefault()).Returns(new Account(){ Verified = default });

            // Act
            var res = acctSrvc.Authenticate(authReq, "");

/*
            // Assert
            var resp = Assert.IsType<OkObjectResult>(res.Result);

            // var resp = Assert.IsType<ActionResult>(res);
            var authResp_ = Assert.IsAssignableFrom<AuthenticateResponse>(resp.Value);

            Assert.Matches("abc@email.com", authResp_.Email);
            Assert.Matches("rfrshToken", authResp_.RefreshToken);*/
        }

    }
}
