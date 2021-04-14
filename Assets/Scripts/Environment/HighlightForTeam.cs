using PropHunt.Character;
using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Highlight an given object if a player looks at it for a given team
    /// </summary>
    public class HighlightForTeam : HighlightOnFocus
    {
        /// <summary>
        /// Team of players for which this object will highlight when looked at
        /// </summary>
        public Team highlightTeam;

        public override void Focus(GameObject sender)
        {
            PlayerTeam team = sender.GetComponent<PlayerTeam>();
            // Set focused to true for this frame if the player is from the given team
            if (team != null && team.playerTeam == highlightTeam)
            {
                this.Focused = true;
            }
        }
    }
}