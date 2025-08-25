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
        Returning,
        Error,
    }

    public enum RobotTasks
    {
        None,
        SurfaceScan,
        DefectAnalysis,
        Logging,
    }
    
    public delegate void StatusChangedEvent(RobotStatus status);
    public static event StatusChangedEvent OnStatusChanged;
    
    public delegate void TaskChangedEvent(RobotTasks task);
    public static event TaskChangedEvent OnTaskChanged;
    
    public delegate void TotalDurationEvent(int totalDuration);
    public static event TotalDurationEvent OnTotalDurationCalculated;
    
    private RobotStatus _currentStatus = RobotStatus.Idle;
    private RobotTasks _currentTask = RobotTasks.None;
    
    [Header("Duration")]
    [Tooltip("Sets the minimum robot scan time in seconds")]
    [SerializeField] private int _minScanTime = 15;
    [Tooltip("Sets the maximum robot scan time in seconds")]
    [SerializeField] private int _maxScanTime = 20;
    
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
        int duration = Random.Range(_minScanTime, _maxScanTime + 1);
        int defectCount = Random.Range(_minDefectCount, _maxDefectCount + 1);
        _workingCoroutine = StartCoroutine(DoWork(duration,defectCount));
        OnTotalDurationCalculated?.Invoke(duration);
    }
    
    private void OnStopClicked()
    {
        if(_currentStatus == RobotStatus.Idle) return;
        StopCoroutine(_workingCoroutine);
        ResetProgress();
        SetTask(RobotTasks.None);
        OnTotalDurationCalculated?.Invoke(-1);
    }
    #endregion

    #region StatusHandlers
    private void ResetProgress()
    {
        _slider.value = 0f;
    }

    public void SetStatus(RobotStatus status)
    {
        _currentStatus = status;
        _statusText.text = $"<b>Status: <smallcaps>{_currentStatus.ToString()}</smallcaps></b>";
        OnStatusChanged?.Invoke(status);
    }
    
    public void SetTask(RobotTasks task)
    {
        _currentTask = task;
        _taskText.text = $"<b>Task: <smallcaps>{_currentTask.ToString()}</smallcaps></b>";
        OnTaskChanged?.Invoke(task);
    }
    #endregion
    
    private System.Collections.IEnumerator DoWork(float duration, float defectCount)
    {
        SetTask(RobotTasks.SurfaceScan);
        
        float elapsed = 0f;
        _slider.value = 0f;
        
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
                RobotStatus originalStatus = _currentStatus;
                float pausedTime = 0f;
                SetTask(RobotTasks.DefectAnalysis);
                SetStatus(RobotStatus.Paused);
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
                SetStatus(originalStatus);
                SetTask(RobotTasks.SurfaceScan);
                nextDefectIndex++;
            }
            elapsed += Time.deltaTime;
            _slider.value = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        ResetProgress();
    }

    #region Getter-Setter
    //private RobotStatus _currentStatus = RobotStatus.Idle;
    //private RobotTasks _currentTask = RobotTasks.None;

    public RobotStatus CurrentStatus => _currentStatus;
    public RobotTasks CurrentTask => _currentTask;

    public int GetDefectPauseTime()
    {
        return _defectPauseTime;
    }

    #endregion
}
