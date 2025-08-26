using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets
{
    public class CurrentLevelPanelScript : MonoBehaviour
    {
        TextMeshPro currentLevel;

        public void SetCurrentLevel(Level level)
        {
            currentLevel.text = level.Id.ToString();
        }

        private void OnEnable()
        {
            var childComponents = GetComponentsInChildren<TextMeshPro>();

            currentLevel = childComponents[1];
        }
    }
}
