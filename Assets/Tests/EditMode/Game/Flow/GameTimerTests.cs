using System.Collections;
using Mirror;
using Moq;
using NUnit.Framework;
using PropHunt.Game.Flow;
using PropHunt.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Game.Flow
{
    [TestFixture]
    public class GameTimerTests : CustomNetworkManagerTestBase
    {
        GameObject go;
        GameTimer timer;
        Mock<IUnityService> unityServiceMock;

        int finishCount;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            go = new GameObject();
            timer = go.AddComponent<GameTimer>();
            unityServiceMock = new Mock<IUnityService>();
            timer.unityService = unityServiceMock.Object;
            finishCount = 0;
            // Have a second pass each update
            unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
            // Wait for timer to finish
            timer.OnFinish += (source, eventArgs) => finishCount++;
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            // Cleanup created objects
            GameObject.DestroyImmediate(go);
        }

        [Test]
        public void TestTimerPause()
        {
            // Start a timer that is 10 seconds long
            timer.StartTimer(10.0f);
            Assert.IsTrue(timer.Running);
            Assert.IsFalse(timer.Finished);
            // Wait until timer finishes
            for (int i = 0; i < 3; i++)
            {
                timer.Update();
                Assert.IsTrue(timer.Running);
                Assert.IsFalse(timer.Finished);
            }

            // Pause timer and ensure it does not finish
            timer.PauseTimer();
            Assert.IsFalse(timer.Running);
            Assert.IsFalse(timer.Finished);
            for (int i = 0; i < 20; i++)
            {
                timer.Update();
                Assert.IsFalse(timer.Running);
                Assert.IsFalse(timer.Finished);
            }

            // Resume timer and wait for completion
            timer.ResumeTimer();
            Assert.IsTrue(timer.Running);
            Assert.IsFalse(timer.Finished);
            // Wait until timer finishes
            while (!timer.Finished) timer.Update();
            // Assert completion and activation of event
            Assert.IsTrue(timer.Running);
            Assert.IsTrue(timer.Finished);
            Assert.That(finishCount, Is.EqualTo(1));
        }

        [Test]
        public void TestTimerComplete()
        {
            // Start a timer that is 10 seconds long
            timer.StartTimer(10.0f);
            Assert.IsTrue(timer.Running);
            Assert.IsFalse(timer.Finished);
            // Wait until timer finishes (10 updates/seconds)
            for (int i = 0; i < 9; i++)
            {
                timer.Update();
                Assert.IsTrue(timer.Running);
                Assert.IsFalse(timer.Finished);
            }
            // Final update
            timer.Update();
            // Assert completion and activation of event
            Assert.IsTrue(timer.Running);
            Assert.IsTrue(timer.Finished);
            Assert.That(finishCount, Is.EqualTo(1));
        }
    }
}