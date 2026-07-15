using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.UI
{
    public class CustomSlider : Slider
    {
        VisualElement progressFiller;

        public CustomSlider()
        {
            // Add custom class for easy manipulation later
            AddToClassList("custom-slider");

            // Get the tracker
            VisualElement tracker = this.Q<VisualElement>("unity-tracker");
            tracker.style.backgroundColor = new StyleColor(new Color32(255, 255, 255, 0));
            tracker.style.borderBottomLeftRadius = 3;
            tracker.style.borderBottomRightRadius = 3;
            tracker.style.borderTopLeftRadius = 3;
            tracker.style.borderTopRightRadius = 3;
            tracker.style.borderBottomColor = new StyleColor(new Color32(117,93,73,255));

            var dragger = this.Q<VisualElement>("unity-dragger");
            dragger.style.backgroundColor = new StyleColor(new Color32(255, 255, 255, 0));
            dragger.style.borderLeftWidth = 0;
            dragger.style.borderRightWidth = 0;
            dragger.style.borderTopWidth = 0;
            dragger.style.borderBottomWidth = 0;
            dragger.style.width = 15;
            dragger.style.maxWidth = 15;
            dragger.style.minWidth = 15;
            dragger.style.height = 15;
            dragger.style.maxHeight = 15;
            dragger.style.minHeight = 15;
            dragger.style.marginTop = -8;
            dragger.style.marginLeft = -2;
            dragger.style.marginRight = -2;

            var imageSlider = Resources.Load<Texture2D>("sliderBall");
            dragger.style.backgroundImage = new StyleBackground(imageSlider);

            // Create progress filler inside the tracker
            progressFiller = new VisualElement();
            progressFiller.name = "unity-filler";
            progressFiller.AddToClassList("horizontal-unity-filler");
            progressFiller.style.position = new StyleEnum<Position>(Position.Absolute);
            progressFiller.style.backgroundColor = new StyleColor(new Color32(108, 86, 67, 255));
            progressFiller.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            tracker.Add(progressFiller);

            // Prevent filler to overflow tracker (if it has rounded borders)
            tracker.style.overflow = new StyleEnum<Overflow>(Overflow.Hidden);

            // Make sure the filler will fill-in the good portion of length when changing slider position
            this.RegisterValueChangedCallback((ChangeEvent<float> ev) => { UpdateProgressFillerPosition(ev.newValue); });

            // Initialize the filler correctly once the slider is drawn
            RegisterCallback((GeometryChangedEvent ev) => { UpdateProgressFillerPosition(value); });
        }

        /// <summary>
        /// Method to position the filler at the right spot
        /// </summary>
        /// <param name="value"></param>
        private void UpdateProgressFillerPosition(float value)
        {
            float percent = (value / (highValue - lowValue)) * 100;
            progressFiller.style.width = new StyleLength(new Length(percent, LengthUnit.Percent));
        }


        /// <summary>
        /// Factory for easy use inside the UI builder
        /// </summary>
        public new class UxmlFactory : UxmlFactory<CustomSlider, CustomSliderTraits>
        {
        }

        /// <summary>
        /// Inherit default traits
        /// </summary>
        public class CustomSliderTraits : UxmlTraits
        {
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                CustomSlider slider = (CustomSlider)ve;
                slider.UpdateProgressFillerPosition(slider.value);
            }
        }

        
    }
}
