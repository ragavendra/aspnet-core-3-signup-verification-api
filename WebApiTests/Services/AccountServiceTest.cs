using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
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
            var authReq = new AuthenticateRequest() { Email = "abc@email.com", Password = "string" };
            var authResp = new AuthenticateResponse() { Email = "abc@email.com", RefreshToken = "rfrshToken" };

            var mockRepo = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(c => c.Map<AuthenticateResponse>(It.IsAny<Account>())).Returns(authResp);

            var data = new List<Account>
            {
                new Account {
                    Email = "abc@email.com",
                    Verified = DateTime.Now,
                    PasswordHash = "$2a$11$j3Hhr8P5PEntOVGDTIUmQOCT7tp0lDBS40PvQb7PTiTsyx9ki1T6.",
                    RefreshTokens = new List<RefreshToken>()
                     },
                new Account { Email = "rgf@email.com", Verified = DateTime.Now, PasswordHash = "string" },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Account>>();
            mockSet.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Account>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockDtCtxt = new Mock<DataContext>(new Mock<IConfiguration>().Object);
            mockDtCtxt.Setup(c => c.Accounts).Returns(mockSet.Object);

            var mockAppSttgs = new Mock<IOptions<AppSettings>>();
            mockAppSttgs.Setup(c => c.Value).Returns(new AppSettings() { Secret = "ThisIsaS3cretsecrwt" });

            var mockEmlSrvc = new Mock<EmailService>(new Mock<IOptions<AppSettings>>().Object);

            var acctSrvc = new AccountService(mockDtCtxt.Object, mockMapper.Object, mockAppSttgs.Object, mockEmlSrvc.Object);

            // Act
            var res = acctSrvc.Authenticate(authReq, "");

            // Assert
            var resp = Assert.IsType<AuthenticateResponse>(res);

            Assert.Matches("abc@email.com", resp.Email);
            Assert.NotNull(resp.RefreshToken);
        }
    }
}
