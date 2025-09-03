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
        _isRelocating = status is StatusChange.RobotStatus.Returning or StatusChange.RobotStatus.Navigating or StatusChange.RobotStatus.Paused;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("Marker") || !_canAnalysis || _isRelocating) return;
        _canAnalysis = false;
        Vector3 markerPos = other.transform.position;
        _defectUI.CreateCoordinate(markerPos);
        _ra.AddCheckPoint(markerPos.x, markerPos.z);
        _ra.DefectDetected();
        LogManager.Log(LogSource.System, LogEvent.DefectDetected, $"Koordinat: {markerPos.x:F2}, {markerPos.y:F2}, {markerPos.z:F2}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Marker")) return;
        _canAnalysis = true;
    }
    
}
