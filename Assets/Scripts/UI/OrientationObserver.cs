using System;
using UnityEngine;
using UnityEngine.EventSystems;


public class OrientationObserver : UIBehaviour
{
    [SerializeField] GameObject horizontalLayout;
    [SerializeField] GameObject verticalLayout;
    
    public GameObject ActiveLayout => lastIsLandscape == true ? horizontalLayout : verticalLayout;
    public MenuController ActiveMenu => ActiveLayout.GetComponent<MenuController>();
    
    
    private bool? lastIsLandscape;

    protected override void Start()
    {
        base.Start();
        ApplyCurrentLayout();
    }

    private void Update()
    {
        ApplyCurrentLayout();
    }

    private void ApplyCurrentLayout()
    {
        bool isLandscape = Screen.width > Screen.height;

        if (lastIsLandscape.HasValue && lastIsLandscape.Value == isLandscape)
            return;

        lastIsLandscape = isLandscape;

        horizontalLayout.SetActive(isLandscape);
        verticalLayout.SetActive(!isLandscape);
    }
}
