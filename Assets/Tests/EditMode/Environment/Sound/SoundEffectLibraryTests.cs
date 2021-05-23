using NUnit.Framework;
using PropHunt.Environment.Sound;
using UnityEditor;
using UnityEngine;

namespace Tests.EditMode.Environment.Sound
{
    [TestFixture]
    public class SoundEffectLibraryTests
    {
        [Test]
        public void TestLookupTables()
        {
            SoundEffectLibrary library = new SoundEffectLibrary();

            LabeledSFX glassHit = new LabeledSFX
            {
                soundMaterial = SoundMaterial.Glass,
                soundType = SoundType.Hit,
                audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/SFX/Hits/glass-hit-1.wav"),
                soundId = "testSound1",
            };
            LabeledSFX concreteHit = new LabeledSFX
            {
                soundMaterial = SoundMaterial.Concrete,
                soundType = SoundType.Hit,
                audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/SFX/Hits/glass-hit-2.wav"),
                soundId = "testSound2",
            };
            LabeledSFX[] sounds = new LabeledSFX[]{
                glassHit, concreteHit
            };

            library.sounds = sounds;

            // Setup sound library
            library.VerifyLookups();

            // Verify getting sounds works as expected
            Assert.IsTrue(library.GetSFXClipById("testSound1") == glassHit);
            Assert.IsTrue(library.GetSFXClipBySoundMaterial(SoundMaterial.Glass) == glassHit);
            LabeledSFX byType = library.GetSFXClipBySoundType(SoundType.Hit);
            Assert.IsTrue(byType == glassHit || byType == concreteHit);
            Assert.IsTrue(library.GetSFXClipBySoundMaterialAndType(SoundMaterial.Concrete, SoundType.Hit) == concreteHit);

            // Test the various lookup functions
            Assert.IsTrue(library.HasSoundEffect("testSound1"));
            Assert.IsFalse(library.HasSoundEffect("sound not in the library"));

            Assert.IsTrue(library.HasSoundEffect(SoundMaterial.Glass));
            Assert.IsFalse(library.HasSoundEffect(SoundMaterial.Misc));

            Assert.IsTrue(library.HasSoundEffect(SoundType.Hit));
            Assert.IsFalse(library.HasSoundEffect(SoundType.Misc));

            Assert.IsTrue(library.HasSoundEffect(SoundMaterial.Glass, SoundType.Hit));
            Assert.IsFalse(library.HasSoundEffect(SoundMaterial.Glass, SoundType.Misc));
        }
    }
}