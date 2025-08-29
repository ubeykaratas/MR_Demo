using System;
using TMPro;
using UnityEngine;

public class HandleTooltip : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _tooltipText;
    [SerializeField] private Transform _eyeObject;
    
    [Header("Animation")]
    [SerializeField] private float _swayAmount;
    [SerializeField] private float _frequency;
    
    private Vector3 _startPosition;
    
    private void Start()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        if (!_eyeObject) return;
        Vector3 direction = _eyeObject.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180f, 0);
        
        float newY = _startPosition.y + MathF.Sin(Time.time * _frequency) * _swayAmount;
        transform.position = new Vector3(_startPosition.x, newY, _startPosition.z);
        
    }

    public void SetText(Vector3 pos)
    {
        if(!_tooltipText) Debug.LogError("Could not find TooltipText");
        _tooltipText.text = $"X: {pos.x:F1}, Y: {pos.y:F1}, Z: {pos.z:F1}";
    }
}
