using System;
using Moq;
using NUnit.Framework;
using PropHunt.Environment.Sound;
using PropHunt.Utils;
using UnityEngine;

namespace Tests.EditMode.Environment.Sound
{
    [TestFixture]
    public class SoundEffectOnHitTests : SoundEffectManagerTestBase
    {
        SoundEffectOnHit soundOnHit = new SoundEffectOnHit();

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            soundOnHit = new GameObject().AddComponent<SoundEffectOnHit>();
            soundOnHit.Awake();
        }

        public override void TearDown()
        {
            base.TearDown();
            GameObject.DestroyImmediate(soundOnHit.gameObject);
            // Cleanup any created audio sources
            foreach (AudioSource source in GameObject.FindObjectsOfType<AudioSource>())
            {
                GameObject.DestroyImmediate(source.gameObject);
            }
        }

        [Test]
        public void TestCreateSoundEvent()
        {
            Mock<ICollision> collisionMock = new Mock<ICollision>();
            Mock<IContactPoint> contactPoint = new Mock<IContactPoint>();
            contactPoint.Setup(e => e.point).Returns(Vector3.zero);
            collisionMock.Setup(e => e.GetContact(0)).Returns(contactPoint.Object);
            collisionMock.Setup(e => e.relativeVelocity).Returns(new Vector3(0, 10, 0));
            soundOnHit.CollisionEvent(collisionMock.Object);
        }

        [Test]
        public void TestDoNotCreateSoundEventSlow()
        {
            Mock<ICollision> collisionMock = new Mock<ICollision>();
            Mock<IContactPoint> contactPoint = new Mock<IContactPoint>();
            contactPoint.Setup(e => e.point).Returns(Vector3.zero);
            collisionMock.Setup(e => e.GetContact(0)).Returns(contactPoint.Object);
            collisionMock.Setup(e => e.relativeVelocity).Returns(Vector3.zero);
            soundOnHit.CollisionEvent(collisionMock.Object);
        }

        [Test]
        public void TestCreateSoundOnCollision()
        {
            // Test as not the server
            soundOnHit.OnCollisionEnter(new Collision());
            // Test as the server
            Mock<INetworkService> networkServiceMock = new Mock<INetworkService>();
            soundOnHit.networkService = networkServiceMock.Object;
            networkServiceMock.Setup(e => e.isServer).Returns(true);
            Assert.Throws<ArgumentOutOfRangeException>(() => soundOnHit.OnCollisionEnter(new Collision()));
        }

        [Test]
        public void TestSoundForElapsedTime()
        {
            Mock<ICollision> collisionMock = new Mock<ICollision>();
            Mock<IContactPoint> contactPoint = new Mock<IContactPoint>();
            contactPoint.Setup(e => e.point).Returns(Vector3.zero);
            collisionMock.Setup(e => e.GetContact(0)).Returns(contactPoint.Object);
            collisionMock.Setup(e => e.relativeVelocity).Returns(new Vector3(0, 10, 0));
            Mock<IUnityService> unityServiceMock = new Mock<IUnityService>();
            unityServiceMock.Setup(e => e.time).Returns(0);
            soundOnHit.unityService = unityServiceMock.Object;
            soundOnHit.minimumDelay = Mathf.Infinity;
            // Do two collision events to trigger the time interval problem
            soundOnHit.CollisionEvent(collisionMock.Object);
            soundOnHit.CollisionEvent(collisionMock.Object);
        }
    }
}