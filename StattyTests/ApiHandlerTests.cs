using System.Threading.Tasks;
using NUnit.Framework;
using StattyBot.structs;
using StattyBot.util;
namespace StattyTests {
    [TestFixture]
    public class ApiHandlerTests {
        private ApiHandler apiHandler = new ApiHandler();
        
        [Test]
        [Timeout(5000)]
        public void CanGetUserFromName() {
            Task<User> task = apiHandler.UserProfile("Statty");
            task.ContinueWith(task1 => {
                User user = task1.Result;
                
                Assert.NotNull(user);
                Assert.True(user.UserId == 1695);
                Assert.True(user.Username == "Statty");
                
                Assert.Pass();
            });
        }
        
        [Test]
        [Timeout(5000)]
        public void CanGetUserFromID() {
            Task<User> task = apiHandler.UserProfile(1695);
            task.ContinueWith(task1 => {
                User user = task1.Result;
                
                Assert.NotNull(user);
                Assert.True(user.UserId == 1695);
                Assert.True(user.Username == "Statty");
                
                Assert.Pass();
            });
        }
    }
}