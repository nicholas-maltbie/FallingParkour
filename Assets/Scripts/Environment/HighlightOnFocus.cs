using System.Collections;
using PropHunt.Utils;
using QuickOutline;
using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Highlight an object when a player looks at it
    /// </summary>
    public class HighlightOnFocus : Focusable
    {
        /// <summary>
        /// Current focused state of the object
        /// </summary>
        public bool Focused { get; protected set; }

        /// <summary>
        /// Outline object
        /// </summary>
        protected Outline outline;

        public void Start()
        {
            this.outline = GetComponent<Outline>();
            MaterialUtils.RecursiveSetFloatProperty(gameObject, "_EmissionIsActive", 0);
            StartCoroutine(SetupOutline());
        }

        public IEnumerator SetupOutline()
        {
            if (this.outline != null)
            {
                this.outline.enabled = true;
                yield return null;
                yield return null;
                this.outline.enabled = false;
                yield return null;
                yield return null;
            }
        }

        public override void Focus(GameObject sender)
        {
            // Set focused to true for this frame
            Focused = true;
        }

        public void Update()
        {
            // Set the current focused state
            if (outline != null)
            {
                outline.enabled = Focused;
            }
            MaterialUtils.RecursiveSetFloatProperty(gameObject, "_EmissionIsActive", Focused ? 1 : 0);

            // Assume the player looks away unless told otherwise
            Focused = false;
        }
    }
}