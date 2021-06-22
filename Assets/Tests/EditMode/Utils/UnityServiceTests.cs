using NUnit.Framework;
using PropHunt.Utils;

namespace Tests.EditMode.Utils
{
    [TestFixture]
    public class UnityServiceTests
    {
        [Test]
        public void VerifyUnityServiceInvokeWithoutErrors()
        {
            IUnityService unityService = new UnityService();
            Assert.IsTrue(unityService.deltaTime >= 0.0f);
            Assert.IsTrue(unityService.fixedDeltaTime >= 0.0f);
        }
    }
}