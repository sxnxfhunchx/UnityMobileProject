using System;
using UnityEngine;
using UnityEngine.EventSystems;


public class OrientationObserver : MonoBehaviour
{
    [SerializeField] GameObject horizontalLayout;
    [SerializeField] GameObject verticalLayout;
    
    public GameObject ActiveLayout => IsLandscape() ? horizontalLayout : verticalLayout;
    public MenuController ActiveMenu => ActiveLayout.GetComponent<MenuController>();
    
    private ScreenOrientation lastOrientation;
    private bool isSwitching = false;

    private void Start()
    {
        lastOrientation = GetCurrentOrientation();
        SwitchLayout();
    }
    
    private void OnRectTransformDimensionsChange()
    {
        if (isSwitching) return;

        ScreenOrientation currentOrientation = GetCurrentOrientation();
        if (currentOrientation == lastOrientation) return;

        lastOrientation = currentOrientation;
        SwitchLayout();
    }

    private void SwitchLayout()
    {
        isSwitching = true;

        bool isLandscape = (lastOrientation == ScreenOrientation.LandscapeLeft || 
                            lastOrientation == ScreenOrientation.LandscapeRight);
        
        if (horizontalLayout.activeSelf != isLandscape)
            horizontalLayout.SetActive(isLandscape);

        if (verticalLayout.activeSelf == isLandscape)
            verticalLayout.SetActive(!isLandscape);

        isSwitching = false;
    }
    
    private ScreenOrientation GetCurrentOrientation()
    {
        if (Screen.orientation == ScreenOrientation.Unknown)
        {
            return (Screen.width > Screen.height) ? ScreenOrientation.LandscapeLeft : ScreenOrientation.Portrait;
        }
        return Screen.orientation;
    }

    private bool IsLandscape()
    {
        return lastOrientation == ScreenOrientation.LandscapeLeft || lastOrientation == ScreenOrientation.LandscapeRight;
    }
}
