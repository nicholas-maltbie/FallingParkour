using NUnit.Framework;
using PropHunt.Environment.Sound;

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
                audioClip = null,
                soundId = "testSound1",
            };
            LabeledSFX concreteHit = new LabeledSFX
            {
                soundMaterial = SoundMaterial.Concrete,
                soundType = SoundType.Hit,
                audioClip = null,
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
        }
    }
}