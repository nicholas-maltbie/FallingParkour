using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PropHunt.UI
{
    [RequireComponent(typeof(Text))]
    public class PopulateVersion : MonoBehaviour
    {
        public void Awake()
        {
            GetComponent<Text>().text = $"Version - v{Application.version}";
        }
    }
}