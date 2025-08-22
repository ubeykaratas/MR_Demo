using UnityEngine;

public class RobotIK : MonoBehaviour
{
    private const int UR10e_ROBOT_JOINT_COUNT= 6; //ACTUAL JOINT NUMBER
    
    [Header("Joints")]
    [SerializeField] private Transform[] _joints;
    [SerializeField] private Transform _endEffector;
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _targetDirection;
    [SerializeField] private float _positionTolerance = 0.01f;
    [SerializeField] private int _maxIterations = 10;
    [SerializeField] private float _maxAngleStep = 5.0f;

    private Vector3[] rotationAxes = new Vector3[]
    {
        Vector3.up, //Joint-1: Y-axis
        Vector3.forward, //Joint-2: Z-axis
        Vector3.forward, //Joint-3: Z-axis
        Vector3.forward, //Joint-4: Z-axis
        Vector3.up, //Joint-5: Y-axis
        Vector3.forward, //Joint-6: Z-axis
    };

    private void Update()
    {
        DoIK();
    }

    private void DoIK()
    {
        if (_joints.Length != UR10e_ROBOT_JOINT_COUNT || !_endEffector || !_target) return;

        for (int i = 0; i < _maxIterations; i++)
        {
            bool isPositionClose = (_endEffector.position - _target.position).magnitude < _positionTolerance;
            bool isRotationClose = Mathf.Abs(Vector3.Dot(_endEffector.forward.normalized, _targetDirection.normalized)) > 0.9999f;
            
            Vector3 linkVec = _joints[4].position - _joints[3].position;
            bool isParallelClose = Mathf.Abs(Vector3.Dot(_endEffector.forward.normalized, Vector3.up)) < 0.01f;

            if (isPositionClose && isRotationClose && isParallelClose) break;

            for (int j = _joints.Length - 1; j >= 0; j--)
            {
                Transform currentJoint = _joints[j];
                Vector3 axis = rotationAxes[j];
                Vector3 axisWorld = currentJoint.TransformDirection(axis);

                if (!isPositionClose)
                {
                    Vector3 toEndEffector = _endEffector.position - currentJoint.position;
                    Vector3 toTarget = _target.position - currentJoint.position;
                    Vector3 projEndEffector = Vector3.ProjectOnPlane(toEndEffector, axisWorld);
                    Vector3 projTarget = Vector3.ProjectOnPlane(toTarget, axisWorld);
                    if (projEndEffector.magnitude >= 0.001f && projTarget.magnitude >= 0.001f)
                    {
                        float posAngle = Vector3.SignedAngle(projEndEffector, projTarget, axisWorld);
                        posAngle = Mathf.Clamp(posAngle, -_maxAngleStep, _maxAngleStep);
                        currentJoint.Rotate(axis, posAngle, Space.Self);
                    }
                }

                if (!isRotationClose && j == 5)
                {
                    Vector3 currentDir = _endEffector.forward;
                    Vector3 desiredDir = _targetDirection;
                    Vector3 projCurrentDir = Vector3.ProjectOnPlane(currentDir, axisWorld);
                    Vector3 projDesiredDir = Vector3.ProjectOnPlane(desiredDir, axisWorld);
                    if (projCurrentDir.magnitude >= 0.001f && projDesiredDir.magnitude >= 0.001f)
                    {
                        float rotAngle = Vector3.SignedAngle(projCurrentDir, projDesiredDir, axisWorld);
                        rotAngle = Mathf.Clamp(rotAngle, -_maxAngleStep, _maxAngleStep);
                        currentJoint.Rotate(axis, rotAngle, Space.Self);
                    }
                }
            }

            float j2Angle = _joints[1].localEulerAngles.z;
            float j3Angle = _joints[2].localEulerAngles.z;
            float signOfJ3 = Mathf.Sign(j3Angle);
            float rightAngle = 90f;
            float desiredJ4 = -signOfJ3 * (j3Angle - rightAngle) - j2Angle;
            _joints[3].localEulerAngles = new Vector3(_joints[3].localEulerAngles.x, _joints[3].localEulerAngles.y, desiredJ4);
            
            float desiredJ5 = (signOfJ3 > 0) ? -rightAngle : rightAngle;
            _joints[4].localEulerAngles = new Vector3(_joints[4].localEulerAngles.x, desiredJ5,_joints[4].localEulerAngles.z);
        }
        
    }
}