using UnityEngine;
using System.Collections.Generic;

public class Marker : MonoBehaviour
{
    [Header("Marker Position")]
    [SerializeField] private GameObject _markerPrefab;
    [SerializeField] private Transform _markerSpawnPoint;
    [SerializeField] private float _markerYCorrection;
    
    [Header("Start Button Reference")]
    [SerializeField] private UnityEngine.UI.Button _startButton;
    
    private readonly List<GameObject> _markers = new List<GameObject>();
    private StatusChange.RobotStatus _currentStatus;

    #region monos
    
    private void OnEnable()
    {
        _startButton.onClick.AddListener(RemoveMarks);
        StatusChange.OnTaskChanged += DoMark;
        StatusChange.OnStatusChanged += ChangeStatus;
    }

    private void OnDisable()
    {
        _startButton.onClick.RemoveListener(RemoveMarks);
        StatusChange.OnTaskChanged -= DoMark;
        StatusChange.OnStatusChanged -= ChangeStatus;
    }
    
    #endregion

    private void RemoveMarks()
    {
        if(_currentStatus != StatusChange.RobotStatus.Idle) return;
        foreach (GameObject marker in _markers)
        {
            Destroy(marker);
        }
        _markers.Clear();
    }
    
    private void DoMark(StatusChange.RobotTasks task)
    {
        if (task == StatusChange.RobotTasks.DefectAnalysis)
        {
            Vector3 correctPosition = new Vector3(_markerSpawnPoint.position.x, _markerYCorrection, _markerSpawnPoint.position.z);
            GameObject temp = Instantiate(_markerPrefab, correctPosition, Quaternion.identity);
            temp.SetActive(true);
            _markers.Add(temp);
        }
    }

    private void ChangeStatus(StatusChange.RobotStatus status)
    {
        _currentStatus = status;
    }
}
