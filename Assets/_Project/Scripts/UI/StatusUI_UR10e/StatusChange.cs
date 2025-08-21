using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StatusChange : MonoBehaviour
{
    public enum RobotStatus
    {
        Idle,
        Working,
        Paused,
        Error,
    }

    private enum RobotTasks
    {
        None,
        SurfaceScan,
        DefectAnalysis,
        Logging,
    }
    
    private RobotStatus _currentStatus = RobotStatus.Idle;
    private RobotTasks _currentTask = RobotTasks.None;
    
    [Header("Duration")]
    [Tooltip("Sets the minimum robot scan time in seconds")]
    [SerializeField] private float _minScanTime = 15f;
    [Tooltip("Sets the maximum robot scan time in seconds")]
    [SerializeField] private float _maxScanTime = 20f;
    
    [Header("Defect Settings")]
    [Tooltip("Sets the minimum number of defect that can be encountered")]
    [SerializeField] private int _minDefectCount = 1;
    [Tooltip("Sets the maximum number of defect that can be encountered")]
    [SerializeField] private int _maxDefectCount = 3;
    [SerializeField] private int _defectPauseTime = 1;
    [SerializeField] private int _logPauseTime = 1;
    
    [Header("UI Elements")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _stopButton;
    [SerializeField] private Slider _slider;
    [SerializeField] private TMPro.TextMeshProUGUI _statusText;
    [SerializeField] private TMPro.TextMeshProUGUI _taskText;
    
    private Coroutine _workingCoroutine;

    #region monos
    private void Start()
    {
        _startButton.onClick.AddListener(OnStartClicked);
        _stopButton.onClick.AddListener(OnStopClicked);
        ResetProgress();
    }
    #endregion
    
    #region UI Callbacks
    private void OnStartClicked()
    {
        if (_currentStatus != RobotStatus.Idle) return;
        float duration = Random.Range(_minScanTime, _maxScanTime + .01f);
        _workingCoroutine = StartCoroutine(DoWork(duration));
    }
    
    private void OnStopClicked()
    {
        if(_currentStatus == RobotStatus.Idle) return;
        StopCoroutine(_workingCoroutine);
        ResetProgress();
    }
    #endregion

    #region StatusHandlers
    private void ResetProgress()
    {
        _slider.value = 0f;
        SetStatus(RobotStatus.Idle);
        SetTask(RobotTasks.None);
    }

    private void SetStatus(RobotStatus status)
    {
        _currentStatus = status;
        _statusText.text = $"<b>Status: <smallcaps>{_currentStatus.ToString()}</smallcaps></b>";
    }
    
    private void SetTask(RobotTasks task)
    {
        _currentTask = task;
        _taskText.text = $"<b>Task: <smallcaps>{_currentTask.ToString()}</smallcaps></b>";
    }
    #endregion
    
    private System.Collections.IEnumerator DoWork(float duration)
    {
        SetStatus(RobotStatus.Working);
        SetTask(RobotTasks.SurfaceScan);
        
        float elapsed = 0f;
        _slider.value = 0f;

        int defectCount = Random.Range(_minDefectCount, _maxDefectCount + 1);
        System.Collections.Generic.List<float> defectTimes = new System.Collections.Generic.List<float>();
        for (int i = 0; i < defectCount; i++)
        {
            float time = Random.Range(0f, duration);
            defectTimes.Add(time);
        }
        if (defectTimes == null) throw new ArgumentNullException(nameof(defectTimes));
        defectTimes.Sort();

        int nextDefectIndex = 0;
        
        while (elapsed < duration)
        {
            if (nextDefectIndex < defectTimes.Count && elapsed >= defectTimes[nextDefectIndex])
            {
                float pausedTime = 0f;
                SetTask(RobotTasks.DefectAnalysis);
                while (pausedTime < _defectPauseTime)
                {
                    pausedTime += Time.deltaTime;
                    yield return null;
                }
                
                SetTask(RobotTasks.Logging);
                pausedTime = 0f;
                while (pausedTime < _logPauseTime)
                {
                    pausedTime += Time.deltaTime;
                    yield return null;
                }
                
                SetTask(RobotTasks.SurfaceScan);
                nextDefectIndex++;
            }
            elapsed += Time.deltaTime;
            _slider.value = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        ResetProgress();
    }
}
