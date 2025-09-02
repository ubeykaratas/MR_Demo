using UnityEngine;
using UnityEngine.XR.Hands;

public class GestureDetector : MonoBehaviour
{
    private XRHandSubsystem _handSubsystem;
    public delegate void GestureChanged(Gesture gesture);
    public static event GestureChanged OnGestureChanged;
    
    [Header("Tolerances")]
    [SerializeField] private float _defaultTolerance = 0.05f;
    [SerializeField] private float _pinchTolerance = 0.02f;
    [SerializeField] private float _pokeTolerance = 0.03f;
    [SerializeField] private float _grabTolerance = 0.04f;

    public enum Gesture
    {
        None,
        Pinch,
        Poke,
        Grab,
        Default,
    }

    private Gesture _preLeftGesture;
    private Gesture _preRightGesture;
    private Gesture _currentLeftGesture;
    private Gesture _currentRightGesture;

    private void Start()
    {
        var subsystems = new System.Collections.Generic.List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(subsystems);
        if (subsystems.Count > 0) _handSubsystem = subsystems[0];
    }

    private void Update()
    {
        if (_handSubsystem is not { running: true }) return;
        
        HandleGestureChange(_handSubsystem.leftHand, ref _preLeftGesture);
        HandleGestureChange(_handSubsystem.rightHand, ref _preRightGesture);
        
        //HandleLogging(_handSubsystem.leftHand, ref _preLeftGesture, true);
        //HandleLogging(_handSubsystem.rightHand, ref _preRightGesture,false);
    }

    private void HandleGestureChange(XRHand hand, ref Gesture preGesture)
    {
        if (!hand.isTracked) return;
        
        Gesture currentGesture = GetCurrentGesture(hand);
        if(currentGesture == preGesture) return;
        OnGestureChanged?.Invoke(currentGesture);
        preGesture = currentGesture;
    }

    private void HandleLogging(XRHand hand, ref Gesture preGesture , bool isLeftHand)
    {
        if(!hand.isTracked) return;
        
        Gesture currentGesture = GetCurrentGesture(hand);
        //if(currentGesture == preGesture) return;
        //Debug.LogWarning($"Current {(isLeftHand ? "Left" : "Right")} Hand: {currentGesture}");
        preGesture = currentGesture;
    }

    private Gesture GetCurrentGesture(XRHand hand)
    {
        if (IsDefaultHand(hand)) return Gesture.Default;
        if (IsPinching(hand)) return Gesture.Pinch;
        if (IsPoking(hand)) return Gesture.Poke;
        if (IsGrabbing(hand)) return Gesture.Grab;
        return Gesture.None;
    }

    private bool IsDefaultHand(XRHand hand)
    {
        if (!hand.GetJoint(XRHandJointID.Palm).TryGetPose(out Pose palmPose)) return false;
        
        //Checks the distance between the fingertips and palm
        bool fingersDistance =
            CheckTips(hand, XRHandJointID.IndexTip, XRHandJointID.Palm, _defaultTolerance, false) &&
            CheckTips(hand, XRHandJointID.MiddleTip, XRHandJointID.Palm, _defaultTolerance, false) &&
            CheckTips(hand, XRHandJointID.RingTip, XRHandJointID.Palm, _defaultTolerance, false) &&
            CheckTips(hand, XRHandJointID.LittleTip, XRHandJointID.Palm, _defaultTolerance, false);
        
        //Checks the distance between thumb and index and makes sure not too close
        bool thumbApartFromIndex = true;
        if (hand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out Pose thumbPose) &&
            hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose indexPose))
        {
            float thumbIndexDistance = Vector3.Distance(thumbPose.position, indexPose.position);
            thumbApartFromIndex = (thumbIndexDistance > _pinchTolerance * 1.5f );
        }
        
        return fingersDistance && thumbApartFromIndex;
    }
    
    private bool IsPinching(XRHand hand)
    {
        return CheckTips(hand, XRHandJointID.IndexTip, XRHandJointID.ThumbTip, _pinchTolerance, true);
    }
    private bool IsPoking(XRHand hand)
    {
        return CheckTips(hand, XRHandJointID.IndexTip, XRHandJointID.MiddleTip, _pokeTolerance, false);
    }
    private bool IsGrabbing(XRHand hand)
    {
        return CheckTips(hand, XRHandJointID.MiddleTip, XRHandJointID.Palm, _grabTolerance, true);
    }

    private bool CheckTips(XRHand hand, XRHandJointID joint1, XRHandJointID joint2, float tolerance, bool isMinTolerance)
    {
        if (hand.GetJoint(joint1).TryGetPose(out Pose tip1) &&
            hand.GetJoint(joint2).TryGetPose(out Pose tip2))
        {
            float distance = Vector3.Distance(tip1.position, tip2.position);
            return isMinTolerance ? distance < tolerance : distance > tolerance;
        }
        
        return false;
    }
}
