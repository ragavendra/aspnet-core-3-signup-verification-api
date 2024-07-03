using WebApi.Models.Accounts;

namespace WebApiTests
{
    public class RegisterRequestTest
    {
        [Fact]
        public void CheckDataAttributes() {

            // Assert.Raises()

            var regReq = new RegisterRequest(){

            };

            Assert.True(1 == 3);


        }
   }
}
