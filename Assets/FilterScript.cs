using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;

public class FilterScript : MonoBehaviour
{
    VisualElement root;
    List<VisualElement> filters = new List<VisualElement>();
    public SortType currentSortType;
    VisualElement arrow;
    public bool isDescending = false;
    VisualElement applyFilters;
    public event EventHandler<FilterEvent> FilterApplied;
    VisualElement resetFilters;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    private void FilterScript_onClick()
    {
        root.Q<Button>("close").clicked -= FilterScript_onClick;
        gameObject.SetActive(false);
        filters.Clear();
    }

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<Button>("close").clicked += FilterScript_onClick;

        filters.Add(root.Q<VisualElement>("calories"));
        filters.Add(root.Q<VisualElement>("amount"));
        filters.Add(root.Q<VisualElement>("fat"));
        filters.Add(root.Q<VisualElement>("saturates"));
        filters.Add(root.Q<VisualElement>("salt"));
        filters.Add(root.Q<VisualElement>("sugar"));

        foreach (var filter in filters)
        {
            filter.RegisterCallback<PointerDownEvent>((pointerDown) =>
            {
                var selectedOption = (VisualElement)pointerDown.currentTarget;
                selectedOption.style.backgroundColor = Color.black;
                selectedOption.Q<Label>().style.color = Color.white;

                var others = filters.Where(x => x.name != selectedOption.name);
                foreach (var other in others)
                {
                    other.style.backgroundColor = Color.white;
                    other.Q<Label>().style.color = Color.black;
                }

                currentSortType = (SortType)Enum.Parse(typeof(SortType), selectedOption.name, true);

            });
        }

        var currentFilter = filters.First(x => x.name == currentSortType.ToString().ToLower());
        currentFilter.style.backgroundColor = Color.black;
        currentFilter.Q<Label>().style.color = Color.white;

        arrow = root.Q<VisualElement>("arrow");

        if (isDescending)
        {
            arrow.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("arrow_down"));
        }
        else
        {
            arrow.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("arrow_up"));
        }

        arrow.RegisterCallback<PointerDownEvent>((pointerDown) =>
        {
            if (isDescending)
            {
                arrow.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("arrow_up"));
                isDescending = false;
            }
            else
            {
                arrow.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("arrow_down"));
                isDescending = true;
            }
        });

        applyFilters = root.Q<VisualElement>("applyFilters");
        applyFilters.RegisterCallback<PointerDownEvent>((pointerDownEvent) =>
        {
            var filterEvent = new FilterEvent(isDescending, currentSortType);
            FilterApplied.Invoke(this, filterEvent);

            root.Q<Button>("close").clicked -= FilterScript_onClick;
            gameObject.SetActive(false);
            filters.Clear();
        });

        resetFilters = root.Q<VisualElement>("resetFilters");
        resetFilters.RegisterCallback<PointerDownEvent>((pointerDownEvent) =>
        {
            currentSortType = SortType.Calories;
            isDescending = false;

            arrow.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("arrow_up"));
            var selectedOption = filters.First(x => x.name == SortType.Calories.ToString().ToLower());
            selectedOption.style.backgroundColor = Color.black;
            selectedOption.Q<Label>().style.color = Color.white;

            var others = filters.Where(x => x.name != selectedOption.name);
            foreach (var other in others)
            {
                other.style.backgroundColor = Color.white;
                other.Q<Label>().style.color = Color.black;
            }

        });
     }

    // Update is called once per frame
    void Update()
    {
        
    }
}
