using TMPro;
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
        Analysing,
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
    
    [Header("Defect and Log Settings")]
    [SerializeField] private int _defectPauseTime = 1;
    [SerializeField] private int _logPauseTime = 1;
    
    [Header("UI Elements")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private TextMeshProUGUI _testText;
    [SerializeField] private Button _stopButton;
    [SerializeField] private Slider _slider;
    [SerializeField] private TMPro.TextMeshProUGUI _statusText;
    [SerializeField] private TMPro.TextMeshProUGUI _taskText;
    
    private Coroutine _sliderCoroutine;
    private RobotStatus _preStatus = RobotStatus.Idle;

    #region monos
    private void Start()
    {
        _startButton.onClick.AddListener(OnStartClicked);
        _pauseButton.onClick.AddListener(OnPauseClicked);
        _stopButton.onClick.AddListener(OnStopClicked);
        ResetProgress();
        _slider.value = 0f;
    }

    

    #endregion
    
    #region UI Callbacks
    private void OnStartClicked()
    {
        if (_currentStatus != RobotStatus.Idle) return;
        int duration = Random.Range(_minScanTime, _maxScanTime + 1);
        _sliderCoroutine = StartCoroutine(HandleSlider(duration));
        OnTotalDurationCalculated?.Invoke(duration);
    }

    private void OnPauseClicked()
    {
        switch (_currentStatus)
        {
            case RobotStatus.Paused:
                SetStatus(_preStatus);
                _testText.text = "Pause";
                break;
            default:
                _preStatus = _currentStatus;
                SetStatus(RobotStatus.Paused);
                _testText.text = "Continue";
                break;
        }
        
    }
    
    private void OnStopClicked()
    {
        if(_currentStatus == RobotStatus.Idle) return;
        StopCoroutine(_sliderCoroutine);
        OnTotalDurationCalculated?.Invoke(-1);
        ResetProgress();
    }
    #endregion

    #region StatusHandlers
    private void ResetProgress()
    {
        SetTask(RobotTasks.None);
        if(_currentStatus == RobotStatus.Returning) return;
        _slider.value = 0f;
        SetStatus(RobotStatus.Idle);
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
    private System.Collections.IEnumerator HandleSlider(int duration)
    {
         float elapsed = 0f;
        _slider.value = 0f;

        while (elapsed < duration)
        {
            if (_currentStatus is not (RobotStatus.Working or RobotStatus.Returning))
            {
                yield return null;
                continue;
            }
            
            elapsed += Time.deltaTime;
            _slider.value = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
    }

    #region Getter-Setter
    public RobotStatus CurrentStatus => _currentStatus;
    public RobotTasks CurrentTask => _currentTask;

    public int GetDefectPauseTime()
    {
        return _defectPauseTime;
    }
    
    public int GetLogPauseTime()
    {
        return _logPauseTime;
    }

    #endregion
}
