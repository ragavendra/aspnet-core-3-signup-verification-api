using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using WebApi.Entities;
using WebApi.Models.Accounts;
using WebApi.Services;

namespace WebApiTests
{
    public class RegisterRequestTest
    {
        // Model validation happens in middleware? hence no validation occurs here
        [Obsolete]
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

        // admin can revoke any token
        [Fact]
        public async Task RevokeTokenAdminByDefault()
        {
            // Arrange
            // var authReq = new Mock<AuthenticateRequest>();
            var revTknReq = new RevokeTokenRequest() { Token = "some" };

            var mockRepo = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();

            var httpContext = new DefaultHttpContext(); // or mock a `HttpContext`
            httpContext.Request.Headers["token"] = "fake_token_here"; //Set header
                                                                      //Controller needs a controller context

            httpContext.Request.Headers["X-Forwarded-For"] = "";
            httpContext.Items["Account"] = new Account(){ RefreshTokens = new List<RefreshToken>() { new WebApi.Entities.RefreshToken() { Token = "token" }}};

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            // mockRepo.Setup(repo => repo.RevokeToken(It.IsAny<string>(), It.IsAny<string>()));
                //.Returns(authResp);

            var controller = new AccountsController(mockRepo.Object, mockMapper.Object) {
                ControllerContext = controllerContext
             };

            // Act
            var res = controller.RevokeToken(revTknReq);
            // .Authenticate(authReq);

            // Assert
            var resp = Assert.IsType<OkObjectResult>(res);

            // var resp = Assert.IsType<ActionResult>(res);
            // Assert.Matches("abc@email.com", resp.Value.message);
            var authResp_ = Assert.IsAssignableFrom<object>(resp.Value);

            // var someObj = new { key = "" };
            Assert.Matches("{ message = Token revoked }", authResp_.ToString());
            // Assert.Matches("rfrshToken", authResp_.RefreshToken);
        }

        // Need to mock Account prop as well. Not sure if it is worth?
        [Obsolete]
        [Fact]
        public async Task RevokeValidTokenCtrlrCtxtMock()
        {
            // Arrange
            // var authReq = new Mock<AuthenticateRequest>();
            var revTknReq = new RevokeTokenRequest() { Token = "token" };

            var mockRepo = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();

            var httpContext = new DefaultHttpContext(); // or mock a `HttpContext`
            httpContext.Request.Headers["token"] = "fake_token_here"; //Set header
                                                                      //Controller needs a controller context

            httpContext.Request.Headers["X-Forwarded-For"] = "";
            httpContext.Items["Account"] = new Account()
            {
                RefreshTokens = new List<RefreshToken>() { new WebApi.Entities.RefreshToken() { Token = "token" } },
                Role = Role.User
            };

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var cntrlrMock = new Mock<ControllerContext>();

            // mockRepo.Setup(repo => repo.RevokeToken(It.IsAny<string>(), It.IsAny<string>()));
                //.Returns(authResp);

            var controller = new AccountsController(mockRepo.Object, mockMapper.Object) {
                // ControllerContext = controllerContext
                ControllerContext = cntrlrMock.Object
             };

            // Act
            var res = controller.RevokeToken(revTknReq);
            // .Authenticate(authReq);

            // Assert
            var resp = Assert.IsType<OkObjectResult>(res);

            // var resp = Assert.IsType<ActionResult>(res);
            // Assert.Matches("abc@email.com", resp.Value.message);
            var authResp_ = Assert.IsAssignableFrom<object>(resp.Value);

            // var someObj = new { key = "" };
            Assert.Matches("{ message = Token revoked }", authResp_.ToString());
            // Assert.Matches("rfrshToken", authResp_.RefreshToken);
        }

        [Fact]
        public async Task RevokeValidToken()
        {
            // Arrange
            // var authReq = new Mock<AuthenticateRequest>();
            var revTknReq = new RevokeTokenRequest() { Token = "token" };

            var mockRepo = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();

            var httpContext = new DefaultHttpContext(); // or mock a `HttpContext`
            httpContext.Request.Headers["token"] = "fake_token_here"; //Set header
                                                                      //Controller needs a controller context

            httpContext.Request.Headers["X-Forwarded-For"] = "";
            httpContext.Items["Account"] = new Account()
            {
                RefreshTokens = new List<RefreshToken>() { new WebApi.Entities.RefreshToken() { Token = "token" } },
                Role = Role.User
            };

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            // mockRepo.Setup(repo => repo.RevokeToken(It.IsAny<string>(), It.IsAny<string>()));
                //.Returns(authResp);

            var controller = new AccountsController(mockRepo.Object, mockMapper.Object) {
                ControllerContext = controllerContext
             };

            // Act
            var res = controller.RevokeToken(revTknReq);
            // .Authenticate(authReq);

            // Assert
            var resp = Assert.IsType<OkObjectResult>(res);

            // var resp = Assert.IsType<ActionResult>(res);
            // Assert.Matches("abc@email.com", resp.Value.message);
            var authResp_ = Assert.IsAssignableFrom<object>(resp.Value);

            // var someObj = new { key = "" };
            Assert.Matches("{ message = Token revoked }", authResp_.ToString());
            // Assert.Matches("rfrshToken", authResp_.RefreshToken);
        }

        [Fact]
        public async Task RevokeInvalidToken()
        {
            // Arrange
            // var authReq = new Mock<AuthenticateRequest>();
            var revTknReq = new RevokeTokenRequest() { Token = "some" };

            var mockRepo = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();

            var httpContext = new DefaultHttpContext(); // or mock a `HttpContext`
            httpContext.Request.Headers["token"] = "fake_token_here"; //Set header
                                                                      //Controller needs a controller context

            httpContext.Request.Headers["X-Forwarded-For"] = "";
            httpContext.Items["Account"] = new Account()
            {
                RefreshTokens = new List<RefreshToken>() { new RefreshToken() { Token = "token" } },
                Role = Role.User
            };

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            // mockRepo.Setup(repo => repo.RevokeToken(It.IsAny<string>(), It.IsAny<string>()));
                //.Returns(authResp);

            var controller = new AccountsController(mockRepo.Object, mockMapper.Object) {
                ControllerContext = controllerContext
             };

            // Act
            var res = controller.RevokeToken(revTknReq);
            // .Authenticate(authReq);

            // Assert
            var resp = Assert.IsType<UnauthorizedObjectResult>(res);

            // var resp = Assert.IsType<ActionResult>(res);
            // Assert.Matches("abc@email.com", resp.Value.message);
            var authResp_ = Assert.IsAssignableFrom<object>(resp.Value);

            // var someObj = new { key = "" };
            Assert.Matches("{ message = Unauthorized }", authResp_.ToString());
            // Assert.Matches("rfrshToken", authResp_.RefreshToken);
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task RevokeTokenNullEmpty(string token)
        {
            // Arrange
            // var authReq = new Mock<AuthenticateRequest>();
            var revTknReq = new RevokeTokenRequest() { Token = token };
            var authResp = new AuthenticateResponse(){ Email = "abc@email.com", RefreshToken = "rfrshToken" };

            var mockRepo = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();

            var httpContext = new DefaultHttpContext(); // or mock a `HttpContext`
            httpContext.Request.Headers["token"] = "fake_token_here"; //Set header
                                                                      //Controller needs a controller context

            httpContext.Request.Headers["X-Forwarded-For"] = "";
            httpContext.Items["Account"] = new Account(){ RefreshTokens = new List<RefreshToken>() { new WebApi.Entities.RefreshToken() { Token = "token" }}};

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var controller = new AccountsController(mockRepo.Object, mockMapper.Object) {
                ControllerContext = controllerContext
             };

            // Act
            var res = controller.RevokeToken(revTknReq);
            // .Authenticate(authReq);

            // Assert
            var resp = Assert.IsType<BadRequestObjectResult>(res);

            // var resp = Assert.IsType<ActionResult>(res);
            // Assert.Matches("abc@email.com", resp.Value.message);
            var authResp_ = Assert.IsAssignableFrom<object>(resp.Value);

            // var someObj = new { key = "" };
            Assert.Matches("{ message = Token is required }", authResp_.ToString());
            // Assert.Matches("rfrshToken", authResp_.RefreshToken);
        }

        [Fact]
        public async Task Register()
        {
            // Arrange
            // var authReq = new Mock<AuthenticateRequest>();
            var model = new RegisterRequest() { Title = "Title" };

            var mockRepo = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();

            var httpContext = new DefaultHttpContext(); // or mock a `HttpContext`
            // httpContext.Request.Headers["origin"] = "fake_token_here"; //Set header
                                                                      //Controller needs a controller context

/*
            httpContext.Items["Account"] = new Account()
            {
                RefreshTokens = new List<RefreshToken>() { new WebApi.Entities.RefreshToken() { Token = "token" } },
                Role = Role.User
            };*/

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            // mockRepo.Setup(repo => repo.Register(It.IsAny<RegisterRequest>(), It.IsAny<string>()));
                //.Returns(authResp);

            var controller = new AccountsController(mockRepo.Object, mockMapper.Object) {
                ControllerContext = controllerContext
             };

            // Act
            var res = controller.Register(model);

            // Assert
            var resp = Assert.IsType<OkObjectResult>(res);
            var authResp_ = Assert.IsAssignableFrom<object>(resp.Value);

            // var someObj = new { key = "" };
            Assert.Matches("{ message = Registration successful, please check your email for verification instructions }", authResp_.ToString());
        }

        [Fact]
        public async Task VerifyEmail()
        {
            // Arrange
            // var authReq = new Mock<AuthenticateRequest>();
            var model = new VerifyEmailRequest() { };

            var mockRepo = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();
            var controller = new AccountsController(mockRepo.Object, mockMapper.Object) {
                // ControllerContext = controllerContext
             };

            // Act
            var res = controller.VerifyEmail(model);

            // Assert
            var resp = Assert.IsType<OkObjectResult>(res);
            var authResp_ = Assert.IsAssignableFrom<object>(resp.Value);

            // var someObj = new { key = "" };
            Assert.Matches("{ message = Verification successful, you can now login }", authResp_.ToString());
        }

        [Fact]
        public async Task ForgotPassword()
        {
            // Arrange
            // var authReq = new Mock<AuthenticateRequest>();
            var model = new ForgotPasswordRequest { };

            var mockRepo = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();

            var httpContext = new DefaultHttpContext(); // or mock a `HttpContext`
            // httpContext.Request.Headers["origin"] = "fake_token_here"; //Set header
                                                                      //Controller needs a controller context
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var controller = new AccountsController(mockRepo.Object, mockMapper.Object) {
                ControllerContext = controllerContext
             };

            // Act
            var res = controller.ForgotPassword(model);

            // Assert
            var resp = Assert.IsType<OkObjectResult>(res);
            var authResp_ = Assert.IsAssignableFrom<object>(resp.Value);

            // var someObj = new { key = "" };
            Assert.Matches("{ message = Please check your email for password reset instructions }", authResp_.ToString());
        }


    }
}
