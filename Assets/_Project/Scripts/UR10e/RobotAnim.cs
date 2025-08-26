using UnityEngine;

public class RobotAnim : MonoBehaviour
{
    [Header("Movement Area")]
    [SerializeField] private float _minX;
    [SerializeField] private float _maxX;
    [SerializeField] private float _minZ;
    [SerializeField] private float _maxZ;
    [SerializeField] private float _stepZ;
    
    [Header("RobotStatusRef")]
    [SerializeField] private StatusChange _rs;
    
    private Vector3 _startPos;
    private Vector3 _target;
    private float _speed;
    private float _duration;
    private float _currentZ;
    private bool _isMovingRight;
    private bool _isMovingZ;
    private bool _isAnalysing;
    private bool _isPaused;

    #region monos

    private void Start()
    {
        transform.position = new Vector3(_minX, transform.position.y, _minZ);
        _startPos = transform.position;
        _currentZ = _minZ;
        StatusChange.OnTaskChanged += HandleTaskChange;
        StatusChange.OnTotalDurationCalculated += StartWorking;
    }

    private void OnDestroy()
    {
        StatusChange.OnTaskChanged -= HandleTaskChange;
        StatusChange.OnTotalDurationCalculated -= StartWorking;
    }

    private void Update()
    {
        if (_rs.CurrentStatus is StatusChange.RobotStatus.Idle or StatusChange.RobotStatus.Paused) return;
        if (_rs.CurrentTask is StatusChange.RobotTasks.SurfaceScan or StatusChange.RobotTasks.None)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target, _speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _target) < 0.01f) HandleTargetReached();
        }
    }

    #endregion

    #region EventHandler

    private void HandleTaskChange(StatusChange.RobotTasks task)
    {
        if (_isAnalysing && task is not (StatusChange.RobotTasks.DefectAnalysis or StatusChange.RobotTasks.Logging)) return;
        StartCoroutine(PauseMovement(_rs.GetDefectPauseTime()));
    }
    
    private void StartWorking(int time)
    {
        if (time == -1)
        {
            _rs.SetStatus(StatusChange.RobotStatus.Returning);
            SetTarget();
            return;
        }
        _duration = time;
        _rs.SetStatus(StatusChange.RobotStatus.Working);
        ResetProgress();
        CalculateSpeed();
        SetTarget();
    }

    #endregion
    
    private System.Collections.IEnumerator PauseMovement(float time)
    {
        _isAnalysing = true;
        yield return new WaitForSeconds(time);
        _isAnalysing = false;
    }
    
    private void HandleTargetReached()
    {
        if (_rs.CurrentStatus == StatusChange.RobotStatus.Working && !_isMovingZ)
        {
            float nextZ = Mathf.Min(_currentZ + _stepZ, _maxZ);
            if (nextZ <= _currentZ + 0.01f)
            {
                _rs.SetStatus(StatusChange.RobotStatus.Returning);
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
            case StatusChange.RobotStatus.Returning:
                _target = _startPos;
                return;
            case StatusChange.RobotStatus.Working when !_isMovingZ:
                float targetX = _isMovingRight ? _maxX : _minX;
                _target = new Vector3(targetX, transform.position.y, _currentZ);
                return;
        }
    }

    private void ResetProgress()
    {
        _currentZ = _minZ;
        _isMovingRight = true;
        _isMovingZ = false;
        _isAnalysing = false;
        transform.position = _startPos;
    }
    
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
    
}