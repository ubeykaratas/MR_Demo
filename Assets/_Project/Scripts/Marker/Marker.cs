using UnityEngine;

public class Marker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RobotAnim _ra;
    [SerializeField] private HandleDefectUI _defectUI;
    
    private bool _isReturning;

    #region monos

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
        _isReturning = status == StatusChange.RobotStatus.Returning;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Marker") || _isReturning) return;
        Vector3 markerPos = other.transform.position;
        _defectUI.CreateCoordinate(markerPos);
        _ra.DefectDetected();
    }
}
