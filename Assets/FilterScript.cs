using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

public class FilterScript : MonoBehaviour
{
    VisualElement root;
    List<VisualElement> filters = new List<VisualElement>();
    public SortType currentSortType;
    public SortType temporarySortType;
    VisualElement arrow;
    public bool isDescending = false;
    bool temporaryIsDescending = false;
    VisualElement applyFilters;
    public event EventHandler<FilterEvent> FilterApplied;
    VisualElement resetFilters;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    private async void FilterScript_onClick()
    {
        root.Q<Button>("close").clicked -= FilterScript_onClick;

        var close = root.Q<Button>("close");

        var image = Resources.Load<Texture2D>("exitRoundPressed");
        close.style.backgroundImage = new StyleBackground(image);

        await AsyncTask.Await(100);

        image = Resources.Load<Texture2D>("exitRoundUnpressed");
        close.style.backgroundImage = new StyleBackground(image);

        await AsyncTask.Await(100);

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
                selectedOption.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("filterButtonBackgroundSelected"));
                selectedOption.Q<Label>().style.color = new StyleColor(new Color32(254,244,229,255));

                var others = filters.Where(x => x.name != selectedOption.name);
                foreach (var other in others)
                {
                    other.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("filterButtonBackground"));
                    other.Q<Label>().style.color = new StyleColor(new Color32(117, 93, 73, 255));
                }

                temporarySortType = (SortType)Enum.Parse(typeof(SortType), selectedOption.name, true);
                //currentSortType = (SortType)Enum.Parse(typeof(SortType), selectedOption.name, true);

            });
        }

        var currentFilter = filters.First(x => x.name == currentSortType.ToString().ToLower());
        currentFilter.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("filterButtonBackgroundSelected"));
        currentFilter.Q<Label>().style.color = new StyleColor(new Color32(254, 244, 229, 255));

        arrow = root.Q<VisualElement>("arrow");

        if (isDescending)
        {
            arrow.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("sortDownUnpressed"));
        }
        else
        {
            arrow.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("sortUpUnpressed"));
        }

        arrow.RegisterCallback<PointerDownEvent>(async (pointerDown) =>
        {
            if (temporaryIsDescending)
            {
                var image = Resources.Load<Texture2D>("sortDownPressed");
                arrow.style.backgroundImage = new StyleBackground(image);

                await AsyncTask.Await(100);

                arrow.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("sortUpUnpressed"));
                temporaryIsDescending = false;
            }
            else
            {
                var image = Resources.Load<Texture2D>("sortUpPressed");
                arrow.style.backgroundImage = new StyleBackground(image);

                await AsyncTask.Await(100);

                arrow.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("sortDownUnpressed"));
                temporaryIsDescending = true;
            }
        });

        applyFilters = root.Q<VisualElement>("applyFilters");
        applyFilters.RegisterCallback<PointerDownEvent>(async (pointerDownEvent) =>
        {
            var image = Resources.Load<Texture2D>("applyFiltersPressed");
            applyFilters.style.backgroundImage = new StyleBackground(image);

            await AsyncTask.Await(100);

            image = Resources.Load<Texture2D>("applyFiltersUnpressed");
            applyFilters.style.backgroundImage = new StyleBackground(image);

            currentSortType = temporarySortType;
            isDescending = temporaryIsDescending;

            var filterEvent = new FilterEvent(isDescending, currentSortType);
            FilterApplied.Invoke(this, filterEvent);

            root.Q<Button>("close").clicked -= FilterScript_onClick;
            gameObject.SetActive(false);
            filters.Clear();
        });

        resetFilters = root.Q<VisualElement>("resetFilters");
        resetFilters.RegisterCallback<PointerDownEvent>(async (pointerDownEvent) =>
        {
            var image = Resources.Load<Texture2D>("resetFiltersPressed");
            resetFilters.style.backgroundImage = new StyleBackground(image);

            await AsyncTask.Await(100);

            image = Resources.Load<Texture2D>("resetFiltersUnpressed");
            resetFilters.style.backgroundImage = new StyleBackground(image);

            temporarySortType = SortType.Calories;
            temporaryIsDescending = false;

            arrow.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("sortUpUnpressed"));
            var selectedOption = filters.First(x => x.name == SortType.Calories.ToString().ToLower());
            selectedOption.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("filterButtonBackgroundSelected"));
            selectedOption.Q<Label>().style.color = new StyleColor(new Color32(254, 244, 229, 255));

            var others = filters.Where(x => x.name != selectedOption.name);
            foreach (var other in others)
            {
                other.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("filterButtonBackground"));
                other.Q<Label>().style.color = new StyleColor(new Color32(117, 93, 73, 255));
            }

        });
     }

    // Update is called once per frame
    void Update()
    {
        
    }
}
