using UnityEngine;
using System.Collections.Generic;

public class Marker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RobotAnim _ra;
    [SerializeField] private HandleDefectUI _defectUI;
    
    [Header("Materials")]
    [SerializeField] private Material _markerMaterial;
    [SerializeField] private Material _baseDefectMaterial;
    
    private bool _isRelocating;
    private bool _canAnalysis;

    private List<GameObject> _defects;

    #region monos

    private void Start()
    {
        _canAnalysis = true;
        _defects = new List<GameObject>();
    }

    private void OnEnable()
    {
        StatusChange.OnStatusChanged += StatusHandler;
        StatusChange.OnTotalDurationCalculated += Clear;
    }

    private void OnDisable()
    {
        StatusChange.OnStatusChanged -= StatusHandler;
        StatusChange.OnTotalDurationCalculated += Clear;
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
        Renderer objRenderer = other.gameObject.GetComponent<Renderer>();
        if (objRenderer) objRenderer.material = _markerMaterial;
        _defects.Add(other.gameObject);
        LogManager.Log(LogSource.System, LogEvent.DefectDetected, $"Koordinat: {markerPos.x:F2}, {markerPos.y:F2}, {markerPos.z:F2}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Marker")) return;
        _canAnalysis = true;
    }

    private void Clear(int duration)
    {
        if (_defects.Count <= 0 || duration == -1) return;
        
        foreach (var defect in _defects)
        {
            Renderer objRenderer = defect.GetComponent<Renderer>();
            if(!objRenderer) Debug.LogError("NULL");
            if(objRenderer) objRenderer.material = _baseDefectMaterial;
        }
        
        _defects.Clear();
    }
    
}
