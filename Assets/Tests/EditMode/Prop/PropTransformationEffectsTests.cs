using System.Linq;
using Mirror;
using Moq;
using NUnit.Framework;
using PropHunt.Environment.Sound;
using PropHunt.Prop;
using PropHunt.Utils;
using Tests.EditMode.Environment.Sound;
using UnityEditor;
using UnityEngine;

namespace Tests.EditMode.Prop
{
    [TestFixture]
    public class PropTransformationEffectsTests : SoundEffectManagerTestBase
    {
        PropTransformationEffects propTransformation;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            library.sounds = library.sounds.Concat(new LabeledSFX[]{
                new LabeledSFX
                {
                    soundMaterial = SoundMaterial.Misc,
                    soundType = SoundType.PropTransformation,
                    audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/SFX/Misc/SFX_Poof.wav"),
                    soundId = "test-sound"
                }
            }).ToArray();
            library.ClearLookups();
            library.SetupLookups();
            // Create a game object and setup camera follow component
            GameObject go = new GameObject();
            this.propTransformation = go.AddComponent<PropTransformationEffects>();
            this.propTransformation.Start();
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            this.propTransformation.OnDestroy();
            GameObject.DestroyImmediate(this.propTransformation.gameObject);
        }

        [Test]
        public void TestHandleTransformation()
        {
            propTransformation.HandlePropDisguiseChange(this, new ChangeDisguiseEvent
            {
                player = this.propTransformation.gameObject
            });
            NetworkServer.Update();
            NetworkClient.Update();
            Assert.IsTrue(SoundEffectManager.Instance.UsedSources == 1);
        }
    }
}