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
            unityService.GetAxis("Horizontal");
            unityService.GetButton("Jump");
            unityService.GetButtonDown("Jump");
            Assert.IsTrue(unityService.deltaTime >= 0.0f);
            Assert.IsTrue(unityService.fixedDeltaTime >= 0.0f);
        }
    }
}