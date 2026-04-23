using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;


namespace Assets.UI
{
    public class BarFill : VisualElement
    {
        public BarFill() 
        { 
            
        }

        public void RenderRect()
        {
            var rect = new VisualElement();


            // Styling via code
            rect.style.width = parent.localBound.width;
            rect.style.height = parent.localBound.height;
            rect.style.backgroundColor = Color.blue;

            // Set rounded corners
            rect.style.borderTopLeftRadius = 20;
            rect.style.borderTopRightRadius = 20;
            rect.style.borderBottomLeftRadius = 20;
            rect.style.borderBottomRightRadius = 20;

            parent.Add(rect);
        }

        
    }
}
