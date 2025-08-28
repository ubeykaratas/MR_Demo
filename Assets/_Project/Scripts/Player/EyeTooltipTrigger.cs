using UnityEngine;

public class EyeTooltipTrigger : MonoBehaviour
{ 
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Marker")) return;
        DefectTooltipUI tooltip = other.gameObject.GetComponent<DefectTooltipUI>();
        if (!tooltip) Debug.LogError("Could not find DefectTooltipUI component");
        tooltip.ChangeVisibility(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Marker")) return;
        DefectTooltipUI tooltip = other.gameObject.GetComponent<DefectTooltipUI>();
        if (!tooltip) return;
        tooltip.ChangeVisibility(false);
    }
}
