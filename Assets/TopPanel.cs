using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TopPanel : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

 

    public void SetSafeAreaHeight(float height)
    {
        var uiDocument = GetComponent<UIDocument>();
        VisualElement topBar = uiDocument.rootVisualElement.Q<VisualElement>("TopBar");
        topBar.style.maxHeight = new StyleLength(new Length(height, LengthUnit.Pixel));
        topBar.style.minHeight = new StyleLength(new Length(height, LengthUnit.Pixel));
        topBar.style.height = new StyleLength(new Length(height, LengthUnit.Pixel));

    }
}
