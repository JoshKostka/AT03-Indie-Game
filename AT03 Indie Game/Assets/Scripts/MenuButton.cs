using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void MenuButtonAction();

    [Tooltip("The default colour of the button. ")]
    [SerializeField] private Color defaultColour;
    [Tooltip("The colour of the button when selected. ")]
    [SerializeField] private Color selectedColour;
    [Tooltip("The colour of the button when the mouse is over it")]
    [SerializeField] private Color highlightedColour;
    [SerializeField] private UnityEvent onActivate;

    private bool mouseOver = false;
    private Image image;
    private MenuNavigation instance;

    public event MenuButtonAction ActivateEvent = delegate { };
    public event MenuButtonAction SelectEvent = delegate { };

    private void Awake()
    {
        TryGetComponent(out image);
        transform.parent.TryGetComponent(out instance);
    }

    // Start is called before the first frame update
    void Start()
    {
        image.color = defaultColour;
    }

    // Update is called once per frame
    void Update()
    {
        if(mouseOver == true && Input.GetButtonDown("Fire1") == true)
        {
            //if he selected button for the menu is this button
            if (instance.selectedButton == this)
            {
                Activate();
            }
            else
            {
                Select();
            }
        }
    }

    /// <summary>
    /// Use this method to invoke the selection event for the button
    /// </summary>
    public void Select()
    {
        SelectEvent.Invoke();

    /// <summary>
    /// Use this method to invoke the activation event for the button
    /// </summary>
    }    public void Activate()
    {
        ActivateEvent.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
        if(instance.selectedButton != this)
        {
            image.color = highlightedColour;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver=false;
        if(image.color == highlightedColour && this != instance.selectedButton)
        {
            image.color = defaultColour;
        }
    }

    private void OnActivate()
    {
        onActivate.Invoke();
    }

    private void OnSelect()
    {
        if(instance.selectedButton != null)
        {
            instance.selectedButton.image.color = instance.selectedButton.defaultColour;
        }
        instance.selectedButton = this;
        image.color = selectedColour;
    }

    private void OnEnable()
    {
        ActivateEvent += OnActivate;
        SelectEvent += OnSelect;
    }

    private void OnDisable()
    {
        ActivateEvent -= OnActivate;
        SelectEvent -= OnSelect;
    }
}

