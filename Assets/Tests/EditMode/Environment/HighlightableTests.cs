using System.Collections;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Environment;
using QuickOutline;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Environment
{
    /// <summary>
    /// File to test behaviour of highlightable objects
    /// </summary>
    [TestFixture]
    public class HighlightableTests
    {
        /// <summary>
        /// Our highlightable object
        /// </summary>
        HighlightOnFocus highlightable;

        /// <summary>
        /// Outline of our object
        /// </summary>
        Outline objectOutline;

        [UnitySetUp]
        public IEnumerator UnitySetup()
        {
            GameObject go = new GameObject();
            this.objectOutline = go.AddComponent<Outline>();
            this.highlightable = go.AddComponent<HighlightOnFocus>();
            // Setup highlightable
            highlightable.Start();
            // Wait 10 frames for setup to finish
            for (int i = 0; i < 10; i++)
            {
                yield return null;
            }

            // Assert that the start co-routine can finish
            IEnumerator iterable = highlightable.SetupOutline();
            while (iterable.MoveNext()) ;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(highlightable.gameObject);
        }

        [Test]
        public void TestHighlightable()
        {
            // Example flow of highlightable
            // Update without being looked at
            this.highlightable.Update();
            // Assert that the outline is disabled
            Assert.IsFalse(highlightable.Focused);
            // Update after looking at object
            this.highlightable.Focus(highlightable.gameObject);
            // Assert that the outline is enabled
            Assert.IsTrue(highlightable.Focused);
            // Assert that object's highlight goes away after a frame
            this.highlightable.Update();
            Assert.IsFalse(highlightable.Focused);
        }

        [Test]
        public void TestHighlightForTeam()
        {
            GameObject teamPlayer = new GameObject();
            teamPlayer.AddComponent<PlayerTeam>().playerTeam = Team.Prop;

            HighlightForTeam teamHighlight = highlightable.gameObject.AddComponent<HighlightForTeam>();
            teamHighlight.highlightTeam = Team.Prop;

            teamHighlight.Focus(teamPlayer);
            Assert.IsTrue(teamHighlight.Focused);
            // Assert that highlight goes away after a frame
            teamHighlight.Update();
            Assert.IsFalse(teamHighlight.Focused);
            // Assert that highlight is not enabled when looked at by wrong team
            teamHighlight.highlightTeam = Team.Hunter;
            // Assert highlight state
            teamHighlight.Focus(teamPlayer);
            Assert.IsFalse(teamHighlight.Focused);

            GameObject.DestroyImmediate(teamPlayer);
        }
    }
}