using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

public class SettingsScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var close = root.Q<VisualElement>("close");

        close.RegisterCallback<MouseDownEvent>(async (mouseDownEvent) => {

            var image = Resources.Load<Texture2D>("exitRoundPressed");
            close.style.backgroundImage = new StyleBackground(image);

            await AsyncTask.Await(100);

            image = Resources.Load<Texture2D>("exitRoundUnpressed");
            close.style.backgroundImage = new StyleBackground(image);

            this.gameObject.SetActive(false);


        });

        var soundSlider = root.Q<Slider>("soundSlider");
        var soundDragger = soundSlider.Q<VisualElement>("unity-dragger");
        soundDragger.style.backgroundColor = new StyleColor(new Color32(255,255,255,0));
        soundDragger.style.borderLeftWidth = 0;
        soundDragger.style.borderRightWidth = 0;
        soundDragger.style.borderTopWidth = 0;
        soundDragger.style.borderBottomWidth = 0;
        soundDragger.style.width = 15;
        soundDragger.style.maxWidth = 15;
        soundDragger.style.minWidth = 15;
        soundDragger.style.height = 15;
        soundDragger.style.maxHeight = 15;
        soundDragger.style.minHeight = 15;
        soundDragger.style.marginTop = -8;
        soundDragger.style.marginLeft = -2;
        soundDragger.style.marginRight = -2;

        var imageSlider = Resources.Load<Texture2D>("sliderBall");
        soundDragger.style.backgroundImage = new StyleBackground(imageSlider);



        var musicSlider = root.Q<Slider>("musicSlider");
        var musicDragger = musicSlider.Q<VisualElement>("unity-dragger");
        musicDragger.style.backgroundColor = new StyleColor(new Color32(255, 255, 255, 0));
        musicDragger.style.borderLeftWidth = 0;
        musicDragger.style.borderRightWidth = 0;
        musicDragger.style.borderTopWidth = 0;
        musicDragger.style.borderBottomWidth = 0;
        musicDragger.style.width = 15;
        musicDragger.style.maxWidth = 15;
        musicDragger.style.minWidth = 15;
        musicDragger.style.height = 15;
        musicDragger.style.maxHeight = 15;
        musicDragger.style.minHeight = 15;
        musicDragger.style.marginTop = -8;
        musicDragger.style.marginLeft = -2;
        musicDragger.style.marginRight = -2;

        musicDragger.style.backgroundImage = new StyleBackground(imageSlider);

    }
}
