using UnityEngine;
using UnityEngine.XR.Hands;

public class DoorHandleController : MonoBehaviour {

  [Tooltip(
      "The collider region that joints must be in to rotate the door handle")]
  [SerializeField]
  private BoxCollider doorHandleRegion;

  private IHandTrackingService d_handService;

  public void Initialize(IHandTrackingService handService) {
    d_handService = handService;
  }

  void Update() {
    bool useRight =
        d_handService.TryGetPalmPose(Handedness.Right, out var rightPalmPose);
    bool useLeft =
        d_handService.TryGetPalmPose(Handedness.Left, out var leftPalmPose);

    bool rightInHandle =
        useRight && doorHandleRegion.bounds.Contains(rightPalmPose.position);
    bool leftInHandle =
        useLeft && doorHandleRegion.bounds.Contains(leftPalmPose.position);
  }

  private float ComputeFingerCurl(XRHandJoint proximal, XRHandJoint tip,
                                  XRHandJoint wrist) {

    if (!proximal.TryGetPose(out Pose p_Pose) ||
        !tip.TryGetPose(out Pose t_Pose) || !wrist.TryGetPose(out Pose w_Pose))
      return 0f;

    Vector3 proximalPos = (p_Pose.position - w_Pose.position).normalized;
    Vector3 tipPos = (t_Pose.position - p_Pose.position).normalized;

    return Vector3.Angle(proximalPos, tipPos);
  }

  private bool IsFist(Handedness hand) {
    if (!d_handService.TryGetHand(hand, out XRHand xrHand))
      return false;

    XRHandJoint wrist = xrHand.GetJoint(XRHandJointID.Wrist);

    float curlIndex =
        ComputeFingerCurl(xrHand.GetJoint(XRHandJointID.IndexProximal),
                          xrHand.GetJoint(XRHandJointID.IndexTip), wrist);

    float curlMiddle =
        ComputeFingerCurl(xrHand.GetJoint(XRHandJointID.MiddleProximal),
                          xrHand.GetJoint(XRHandJointID.MiddleTip), wrist);

    float curlRing =
        ComputeFingerCurl(xrHand.GetJoint(XRHandJointID.RingProximal),
                          xrHand.GetJoint(XRHandJointID.RingTip), wrist);

    float curlPinky =
        ComputeFingerCurl(xrHand.GetJoint(XRHandJointID.LittleProximal),
                          xrHand.GetJoint(XRHandJointID.LittleTip), wrist);

    return curlIndex > 50f && curlMiddle > 50f && curlRing > 50f &&
           curlPinky > 50f;
  }
}
