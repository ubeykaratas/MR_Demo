using System;
using TMPro;
using UnityEngine;

public class HandleTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _tooltipText;
    [SerializeField] private Transform _eyeObject;

    private void Update()
    {
        if (!_eyeObject) return;
        Vector3 direction = _eyeObject.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180f, 0);
    }

    public void SetText(Vector3 pos)
    {
        if(!_tooltipText) Debug.LogError("Could not find TooltipText");
        _tooltipText.text = $"X: {pos.x:F1}, Y: {pos.y:F1}, Z: {pos.z:F1}";
    }
}
