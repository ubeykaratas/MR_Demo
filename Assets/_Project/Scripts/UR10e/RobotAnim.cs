using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RobotAnim : MonoBehaviour
{
    private readonly int DURATION_LIMIT_COEFFICIENT = 10;
    
    [Header("Movement Area")]
    [SerializeField] private float _minX;
    [SerializeField] private float _maxX;
    [SerializeField] private float _minZ;
    [SerializeField] private float _maxZ;
    [SerializeField] private float _stepZ;
    
    [Header("References")]
    [SerializeField] private StatusChange _rs;
    
    
    private Vector3 _startPos;
    private Vector3 _target;
    private List<(Vector3 pos, bool isMovingRight, bool isMovingZ)> _checkPoints;
    private (Vector3 pos, bool isMovingRight, bool isMovingZ) _prePos;
    private float _speed;
    private float _duration;
    private float _currentZ;
    private bool _isMovingRight;
    private bool _isMovingZ;
    private bool _isRechecking;
    
    public delegate void TotalTimeChanged();
    public static event TotalTimeChanged OnTotalTimeChanged;
    
    public delegate void TotalDefectChanged();
    public static event TotalDefectChanged OnTotalDefectChanged;
    
    public delegate void CompletedTasksChanged();
    public static event CompletedTasksChanged OnCompletedTasksChanged;

    private static float _totalTime;
    private static int _totalDefect;
    private static int _completedTasks;
    private Coroutine _pauseCoroutine;
    private Coroutine _surfaceScanTimeCoroutine;

    private int _counter;

    #region monos

    private void Start()
    {
        transform.position = new Vector3(_minX, transform.position.y, _minZ);
        _startPos = transform.position;
        _currentZ = _minZ;
        _checkPoints = new List<(Vector3, bool, bool)>();
        StatusChange.OnTotalDurationCalculated += StartWorking;
    }

    private void OnDestroy()
    {
        StatusChange.OnTotalDurationCalculated -= StartWorking;
    }

    private void Update()
    {
        if (_rs.CurrentStatus is StatusChange.RobotStatus.Idle or StatusChange.RobotStatus.Analysing or StatusChange.RobotStatus.Paused) return;
        if (_rs.CurrentTask is StatusChange.RobotTasks.SurfaceScan or StatusChange.RobotTasks.None)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target, _speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _target) < 0.01f) HandleTargetReached();
        }
    }

    #endregion

    #region EventHandler
    
    private void StartWorking(int time)
    {
        if (time == -1)
        {
            _rs.SetStatus(StatusChange.RobotStatus.Returning);
            SetTarget();
            return;
        }
        _checkPoints.Clear();
        _duration = time;
        _rs.SetStatus(StatusChange.RobotStatus.Working);
        _rs.SetTask(StatusChange.RobotTasks.SurfaceScan);
        ResetProgress();
        CalculateSpeed();
        SetTarget();
        _surfaceScanTimeCoroutine = StartCoroutine(CountUp());
    }

    #endregion

    private System.Collections.IEnumerator CountUp()
    {
        while (true)
        {
            yield return CheckIfPaused();
            yield return new WaitForSeconds(1f);
            IncrementTotalTime(1f);
            if (++_counter >= _duration * DURATION_LIMIT_COEFFICIENT) yield break;
        }
    }
    private System.Collections.IEnumerator DefectAnalysis(float defectPauseTime, float logPauseTime)
    {
        yield return CheckIfPaused();
        StatusChange.RobotStatus originalStatus = _rs.CurrentStatus;
        StatusChange.RobotTasks originalTask = _rs.CurrentTask;
        _rs.SetStatus(StatusChange.RobotStatus.Analysing);
        _rs.SetTask(StatusChange.RobotTasks.DefectAnalysis);
        IncrementTotalDefect();
        yield return new WaitForSeconds(defectPauseTime);
        yield return CheckIfPaused();
        IncrementCompletedTasks();
        _rs.SetTask(StatusChange.RobotTasks.Logging);
        yield return new WaitForSeconds(logPauseTime);
        yield return CheckIfPaused();
        IncrementCompletedTasks();
        _rs.SetStatus(originalStatus);
        _rs.SetTask(originalTask);
    }

    private System.Collections.IEnumerator CheckIfPaused()
    {
        while (_rs.CurrentStatus == StatusChange.RobotStatus.Paused) yield return null;
    }
    
    private void HandleTargetReached()
    {
        if (_rs.CurrentStatus is StatusChange.RobotStatus.Navigating)
        {
            if(_checkPoints.Count > 0) _checkPoints.RemoveAt(_checkPoints.Count - 1);
            _rs.SetStatus(StatusChange.RobotStatus.Working);
            if (!_isRechecking)
            {
                if(_isMovingZ) _target = new Vector3(transform.position.x, transform.position.y, _currentZ + _stepZ);
                else SetTarget();
            }
            _rs.OnPauseClicked();
        }
        else if (_isRechecking)
        {
            _isRechecking = false;
            _rs.SetStatus(StatusChange.RobotStatus.Navigating);
            SetTarget();
        }
        else if (_rs.CurrentStatus == StatusChange.RobotStatus.Working && !_isMovingZ)
        {
            float nextZ = Mathf.Min(_currentZ + _stepZ, _maxZ);
            if (nextZ <= _currentZ + 0.01f)
            {
                _rs.SetStatus(StatusChange.RobotStatus.Returning);
                IncrementCompletedTasks();
                SetTarget();
            }
            else
            {
                _isMovingZ = true;
                _target = new Vector3(transform.position.x, transform.position.y, nextZ);
            }
        }
        else if (_rs.CurrentStatus == StatusChange.RobotStatus.Working)
        {
            _currentZ = transform.position.z;
            _isMovingRight = !_isMovingRight;
            _isMovingZ = false;
            SetTarget();
        }
        else if (_rs.CurrentStatus == StatusChange.RobotStatus.Returning)
        {
            _rs.SetStatus(StatusChange.RobotStatus.Idle);
            _rs.SetTask(StatusChange.RobotTasks.None);
        }
    }
    
    private void CalculateSpeed()
    {
        if (_duration <= 0) return;

        float width = _maxX - _minX;
        float height = _maxZ - _minZ;
        if (width <= 0 || height < 0 || _stepZ <= 0) return;
        
        float numZStepsF = height / _stepZ;
        int numZSteps = Mathf.CeilToInt(numZStepsF);
        int numRows = numZSteps + 1;
        float totalXDistance = numRows * width;
        float totalZDistance = height;
        float totalDistance = totalXDistance + totalZDistance;
        _speed = totalDistance / _duration;
    }

    private void SetTarget()
    {
        switch (_rs.CurrentStatus)
        {
            case StatusChange.RobotStatus.Navigating:
                if (_isRechecking) //Get to the recheck position
                    AssignTarget(_checkPoints[^1].pos, _checkPoints[^1].isMovingRight, _checkPoints[^1].isMovingZ);
                else //Get back to original position
                {
                    AssignTarget(_prePos.pos, _prePos.isMovingRight, _prePos.isMovingZ);
                } 
                _rs.SetTask(StatusChange.RobotTasks.SurfaceScan);
                return;
            case StatusChange.RobotStatus.Returning:
                if (_pauseCoroutine != null) StopCoroutine(_pauseCoroutine);
                if (_surfaceScanTimeCoroutine != null) StopCoroutine(_surfaceScanTimeCoroutine);
                _target = _startPos;
                return;
            case StatusChange.RobotStatus.Working when !_isMovingZ:
                float targetX = _isMovingRight ? _maxX : _minX;
                _target = new Vector3(targetX, transform.position.y, _currentZ);
                return;
        }
    }

    private void AssignTarget(Vector3 pos, bool isMovingRight, bool isMovingZ)
    {
        _isMovingRight = isMovingRight;
        _isMovingZ = isMovingZ;
        _target = pos;
    }

    private void ResetProgress()
    {
        _counter = 0;
        _currentZ = _minZ;
        _isMovingRight = true;
        _isMovingZ = false;
        transform.position = _startPos;
    }

    #region Private Setters

    private void IncrementTotalTime(float time)
    {
        if (time < 0f) return;
        _totalTime += time;
        OnTotalTimeChanged?.Invoke();
    }

    private void IncrementTotalDefect()
    {
        ++_totalDefect;
        OnTotalDefectChanged?.Invoke();
    }

    private void IncrementCompletedTasks()
    {
        ++_completedTasks;
        OnCompletedTasksChanged?.Invoke();
    }

    #endregion
    
    
    
    #region Gizmos
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 bottomLeft = new Vector3(_minX, transform.position.y, _minZ);
        Vector3 bottomRight = new Vector3(_maxX, transform.position.y, _minZ);
        Vector3 topLeft = new Vector3(_minX, transform.position.y, _maxZ);
        Vector3 topRight = new Vector3(_maxX, transform.position.y, _maxZ);
        
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
    
    #endregion

    #region API

    public void AddCheckPoint(float x, float z)
    {
        if (_checkPoints.Count > 0 &&
            Mathf.Approximately(_checkPoints[^1].pos.x, x) && 
            Mathf.Approximately(_checkPoints[^1].pos.z, z))
        {
            return;
        }
        
        Vector3 pos = new Vector3(x, transform.position.y, z);
        _checkPoints.Add((pos, _isMovingRight, _isMovingZ));
    }

    public void NavigateToCoordinate()
    {
        if(_rs.CurrentStatus is not (StatusChange.RobotStatus.Paused or StatusChange.RobotStatus.Navigating))
            _prePos = (transform.position, _isMovingRight, _isMovingZ);
        _isRechecking = true;
        _rs.SetStatus(StatusChange.RobotStatus.Navigating);
        SetTarget();
    }
    
    public void DefectDetected()
    {
        _pauseCoroutine = StartCoroutine(DefectAnalysis(_rs.GetDefectPauseTime(), _rs.GetLogPauseTime()));
    }
    
    public float TotalTime => _totalTime;
    public int TotalDefect => _totalDefect;
    public int CompletedTasks => _completedTasks;

    #endregion
}