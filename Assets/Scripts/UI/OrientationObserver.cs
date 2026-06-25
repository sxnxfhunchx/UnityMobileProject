using UnityEngine;
using UnityEngine.EventSystems;


[ExecuteAlways]
public class OrientationObserver : UIBehaviour
{
    [SerializeField] GameObject horizintalLayout;
    [SerializeField] GameObject verticalLayout;
    
    private ScreenOrientation lastOrientation;
    private bool isSwitching = false;

    private void Start()
    {
        lastOrientation = GetCurrentOrientation();
        SwitchLayout();
    }
    
    private void OnRectTransformDimensionsChange()
    {
        Debug.Log("OnRectTransformDimensionsChange");
        base.OnRectTransformDimensionsChange();

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

        horizintalLayout.SetActive(isLandscape);
        verticalLayout.SetActive(!isLandscape);

        isSwitching = false;
    }
    
    private ScreenOrientation GetCurrentOrientation()
    {
#if UNITY_EDITOR
        return (Screen.width > Screen.height) ? ScreenOrientation.LandscapeLeft : ScreenOrientation.Portrait;
#else
        if (Screen.orientation == ScreenOrientation.Unknown)
        {
            return (Screen.width > Screen.height) ? ScreenOrientation.LandscapeLeft : ScreenOrientation.Portrait;
        }
        return Screen.orientation;
#endif
    }
}
