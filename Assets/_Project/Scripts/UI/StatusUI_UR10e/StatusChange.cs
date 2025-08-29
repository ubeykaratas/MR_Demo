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
    [SerializeField] private Button _stopButton;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private TextMeshProUGUI _taskText;
    
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
                ChangeToPauseButton(true);
                break;
            default:
                _preStatus = _currentStatus;
                ChangeToPauseButton(false);
                SetStatus(RobotStatus.Paused);
                break;
        }
        
    }
    
    private void OnStopClicked()
    {
        if(_currentStatus == RobotStatus.Idle) return;
        StopCoroutine(_sliderCoroutine);
        OnTotalDurationCalculated?.Invoke(-1);
        ChangeToPauseButton(true);
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
    
    private void ChangeToPauseButton(bool change)
    {
        switch (change)
        {
            case true:
                _pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pause";
                _pauseButton.GetComponent<Image>().color = new Color(217f/255f, 136f/255f, 38/255f, 1f);
                break;
            case false:
                _pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
                _pauseButton.GetComponent<Image>().color = new Color(15f/255f, 164f/255f, 56f/255f, 1f);
                break;
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
