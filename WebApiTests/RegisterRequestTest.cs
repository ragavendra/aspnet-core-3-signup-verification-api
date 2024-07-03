using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using WebApi.Models.Accounts;
using WebApi.Services;

namespace WebApiTests
{
    public class RegisterRequestTest
    {
        [Fact]
        public void CheckDataAttributes() {

            // Assert.Raises()
            var regReq = new RegisterRequest(){
                Password = ""
            };

            // Assert.True(1 == 3);

        }

        [Fact]
        public async Task AuthenticateRequest()
        {
            // Arrange
            // var authReq = new Mock<AuthenticateRequest>();
            var authReq = new AuthenticateRequest();
            var authResp = new AuthenticateResponse(){ Email = "abc@email.com", RefreshToken = "rfrshToken" };

            var mockRepo = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();

            var httpContext = new DefaultHttpContext(); // or mock a `HttpContext`
            httpContext.Request.Headers["token"] = "fake_token_here"; //Set header
                                                                      //Controller needs a controller context

            httpContext.Request.Headers["X-Forwarded-For"] = "";

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            mockRepo.Setup(repo => repo.Authenticate(It.IsAny<AuthenticateRequest>(), It.IsAny<string>()))
                .Returns(authResp);

            var controller = new AccountsController(mockRepo.Object, mockMapper.Object) {
                ControllerContext = controllerContext
             };

            // Act
            var res = controller.Authenticate(authReq);

            // Assert
            var resp = Assert.IsType<OkObjectResult>(res.Result);

            // var resp = Assert.IsType<ActionResult>(res);
            var authResp_ = Assert.IsAssignableFrom<AuthenticateResponse>(resp.Value);

            Assert.Matches("abc@email.com", authResp_.Email);
            Assert.Matches("rfrshToken", authResp_.RefreshToken);
        }

        [Fact]
        public async Task RefreshToken()
        {
            // Arrange
            // var authReq = new Mock<AuthenticateRequest>();
            var authReq = new AuthenticateRequest();
            var authResp = new AuthenticateResponse(){ Email = "abc@email.com", RefreshToken = "rfrshToken" };

            var mockRepo = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();

            var httpContext = new DefaultHttpContext(); // or mock a `HttpContext`
            httpContext.Request.Headers["token"] = "fake_token_here"; //Set header
                                                                      //Controller needs a controller context

            httpContext.Request.Headers["X-Forwarded-For"] = "";

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            mockRepo.Setup(repo => repo.RefreshToken(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(authResp);

            var controller = new AccountsController(mockRepo.Object, mockMapper.Object) {
                ControllerContext = controllerContext
             };

            // Act
            var res = controller.RefreshToken();
            // .Authenticate(authReq);

            // Assert
            var resp = Assert.IsType<OkObjectResult>(res.Result);

            // var resp = Assert.IsType<ActionResult>(res);
            var authResp_ = Assert.IsAssignableFrom<AuthenticateResponse>(resp.Value);

            Assert.Matches("abc@email.com", authResp_.Email);
            Assert.Matches("rfrshToken", authResp_.RefreshToken);
        }
    }
}
