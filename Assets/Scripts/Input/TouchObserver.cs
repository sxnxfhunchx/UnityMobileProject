using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch; 

public class TouchObserver : MonoBehaviour
{
    enum GestureType
    {
        None,
        Swipe, 
        Pinch
    }
    
    [SerializeField] private RectTransform portraitPreviewArea;
    [SerializeField] private RectTransform landscapePreviewArea;
    
    [Header("Rotation Settings")]
    [SerializeField] private Transform yawPivot;
    [SerializeField] private Transform pitchPivot;
    [SerializeField] private float rotationSensitivity = 180f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 30f;

    [Header("Zoom Settings")]
    [SerializeField] private Transform previewCamera;
    [SerializeField] private float zoomSensitivity = 0.005f;
    [SerializeField] private float minDistance = 1.5f;
    [SerializeField] private float maxDistance = 5f;
    
    private float yaw;
    private float pitch;
    private float currentCameraDistance = 3f;
    
    private GestureType currentGesture = GestureType.None;
    
    private Finger firstFinger;
    private Finger secondFinger;
    private Vector2 lastPosition;
    private float lastPinchDistance;
    private Vector2 delta;
    
    private RectTransform CurrentPreviewArea =>
        portraitPreviewArea.gameObject.activeInHierarchy
            ? portraitPreviewArea
            : landscapePreviewArea;
    
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }
    
    void Update()
    {
        int fingersCount = Touch.activeFingers.Count;

        if (fingersCount == 0)
        {
            ResetGesture();
            return;
        }

        if (currentGesture == GestureType.Pinch)
            UpdatePinch();
        
        if (currentGesture == GestureType.Swipe)
            UpdateSwipe();
        
        if (fingersCount == 2)
            TryStartPinch();
        if (fingersCount == 1)
            TryStartSwipe();
    }

    private void TryStartPinch()
    {
        Finger firstFingerCandidate = Touch.activeFingers[0];
        Finger secondFingerCandidate = Touch.activeFingers[1];
        
        Touch firstTouch = firstFingerCandidate.currentTouch;
        Touch secondTouch = secondFingerCandidate.currentTouch;
        
        if (!IsInsidePreviewArea(firstTouch.screenPosition))
            return;
        
        if (!IsInsidePreviewArea(secondTouch.screenPosition))
            return;
        
        firstFinger = firstFingerCandidate;
        secondFinger = secondFingerCandidate;
        
        lastPinchDistance = Vector2.Distance(firstTouch.screenPosition, secondTouch.screenPosition);
        
        currentGesture = GestureType.Pinch;
    }
    
    private void UpdatePinch()
    {
        if (firstFinger == null || secondFinger == null)
        {
            ResetGesture();
            return;
        }
            
        if (!Touch.activeFingers.Contains(firstFinger) ||
            !Touch.activeFingers.Contains(secondFinger))
        {
            ResetGesture();
            return;
        }
        
        Touch touch1 = firstFinger.currentTouch;
        Touch touch2 = secondFinger.currentTouch;

        if (touch1.ended || touch2.ended)
        {
            ResetGesture();
            return;
        }
        
        float distance = Vector2.Distance(touch1.screenPosition, touch2.screenPosition);
        float pinchDelta = distance - lastPinchDistance;
        lastPinchDistance = distance;
        
        if (pinchDelta != 0)
        {
            ZoomModel(pinchDelta);
        }
    }

    private void TryStartSwipe()
    { 
        Finger firstFingerCandidate = Touch.activeFingers[0];
        
        Touch firstTouch = firstFingerCandidate.currentTouch;
        
        if (!IsInsidePreviewArea(firstTouch.screenPosition))
            return;
        
        firstFinger = firstFingerCandidate;
        lastPosition = firstTouch.screenPosition;
        
        currentGesture = GestureType.Swipe;
    }

    private void UpdateSwipe()
    {
        if (firstFinger == null)
        {
            ResetGesture();
            return;
        }
            
        if (!Touch.activeFingers.Contains(firstFinger))
        {
            ResetGesture();
            return;
        }
        
        Touch touch1 = firstFinger.currentTouch;

        if (touch1.ended)
        {
            ResetGesture();
            return;
        }
        
        delta = touch1.screenPosition - lastPosition;
        lastPosition = touch1.screenPosition;

        if (delta != Vector2.zero)
        {
            RotateModel(delta);
        }
    }

    private void ResetGesture()
    {
        currentGesture = GestureType.None;
        
        firstFinger = null;
        secondFinger = null;
        
        lastPinchDistance = 0f;
        lastPosition = Vector2.zero;
        delta = Vector2.zero;
    }
    
    private bool IsInsidePreviewArea(Vector2 position)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(CurrentPreviewArea, position, null
        );
    }
    
    private void RotateModel(Vector2 screenDelta)
    {
        float normalizedX = screenDelta.x / Screen.width;
        float normalizedY = screenDelta.y / Screen.height;

        yaw += normalizedX * rotationSensitivity;
        pitch += normalizedY * rotationSensitivity;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        yawPivot.localRotation = Quaternion.Euler(0f, yaw, 0f);
        pitchPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
    
    private void ZoomModel(float pinchDelta)
    {
        currentCameraDistance -= pinchDelta * zoomSensitivity;
        currentCameraDistance = Mathf.Clamp(currentCameraDistance, minDistance, maxDistance);

        Vector3 pos = previewCamera.localPosition;
        pos.z = currentCameraDistance;
        previewCamera.localPosition = pos;
    }
}
