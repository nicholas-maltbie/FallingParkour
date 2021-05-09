using System.Collections;
using NUnit.Framework;
using PropHunt.Environment.Sound;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Environment.Sound
{
    [TestFixture]
    public class ReturnAudioClipOnFinishTests : SoundEffectManagerTestBase
    {
        [Test]
        public void TestReturnAction()
        {
            // Ignroe warning of delete in edit mode
            LogAssert.ignoreFailingMessages = true;

            // Create and setup test
            GameObject go = new GameObject();
            var audioSource = go.AddComponent<AudioSource>();
            var deleteOnFinish = go.AddComponent<ReturnAudioSourceOnFinish>();

            deleteOnFinish.inUse = true;

            audioSource.Stop();

            deleteOnFinish.Start();
            deleteOnFinish.Update();

            Assert.IsFalse(deleteOnFinish.inUse);

            GameObject.DestroyImmediate(go);
        }
    }
}