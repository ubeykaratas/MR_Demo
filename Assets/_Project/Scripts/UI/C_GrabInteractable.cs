using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class C_GrabInteractable : XRGrabInteractable
{
    [SerializeField] private GestureDetector.Gesture _allowedGesture;
    [SerializeField] private GestureDetector _detector;

    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        GestureDetector.Gesture currentGesture = _detector.GetCurrentGesture();
        if (currentGesture != _allowedGesture) return false;
        return base.IsSelectableBy(interactor);
    }
}
