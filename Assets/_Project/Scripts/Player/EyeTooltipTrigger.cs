using UnityEngine;

public class EyeTooltipTrigger : MonoBehaviour
{
    private bool _isPoking;
    
    private void Start()
    {
        GestureDetector.OnGestureChanged += IsPoking;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("Marker")) return;
        DefectTooltipUI tooltip = other.gameObject.GetComponent<DefectTooltipUI>();
        if (!tooltip) Debug.LogError("Could not find DefectTooltipUI component");
        tooltip.ChangeVisibility(_isPoking);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Marker")) return;
        DefectTooltipUI tooltip = other.gameObject.GetComponent<DefectTooltipUI>();
        if (!tooltip) Debug.LogError("Could not find DefectTooltipUI component");
        tooltip.ChangeVisibility(false);
    }

    private void IsPoking(GestureDetector.Gesture gesture)
    {
        _isPoking = gesture == GestureDetector.Gesture.Poke;
    }
}
