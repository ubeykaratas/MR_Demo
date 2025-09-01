using System;
using UnityEngine;

public class Marker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RobotAnim _ra;
    [SerializeField] private HandleDefectUI _defectUI;
    
    private bool _isRelocating;
    private bool _canAnalysis;

    #region monos

    private void Start()
    {
        _canAnalysis = true;
    }

    private void OnEnable()
    {
        
        StatusChange.OnStatusChanged += StatusHandler;
    }

    private void OnDisable()
    {
        StatusChange.OnStatusChanged -= StatusHandler;
    }

    #endregion
    

    private void StatusHandler(StatusChange.RobotStatus status)
    {
        _isRelocating = status is StatusChange.RobotStatus.Returning or StatusChange.RobotStatus.Navigating;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("Marker") || !_canAnalysis || _isRelocating) return;
        _canAnalysis = false;
        Vector3 markerPos = other.transform.position;
        _defectUI.CreateCoordinate(markerPos);
        _ra.AddCheckPoint(other.transform.position.x, other.transform.position.z);
        _ra.DefectDetected();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Marker")) return;
        _canAnalysis = true;
    }
    
}
