using UnityEngine;
using UnityEngine.XR.Hands;

internal enum DoorHandleMode { None, Rotating }

public class DoorHandleController : MonoBehaviour {

  [Tooltip(
      "The collider region that joints must be in to rotate the door handle")]
  [SerializeField]
  private BoxCollider doorHandleRegion;

  [Tooltip("The visuals to rotate")]
  [SerializeField]
  private Transform handleVisual;

  [SerializeField]
  private HingeJoint hinge;

  private float maxHandleAngle = 30f;
  private float motorForce = 200f;
  private float motorSpeed = 20f;

  private float startPalmYaw;
  private float startHandleAngle;

  private IHandTrackingService d_handService;
  private Handedness activeHand;
  private DoorHandleMode mode;

  public float speed = 50f; // degrees/sec
  public float min = -45f;
  public float max = 45f;
  private int direction = 1;

  public void Initialize(IHandTrackingService handService) {
    d_handService = handService;
  }

  // void Start() {
  //
  //   var rb = GetComponent<Rigidbody>();
  //   rb.isKinematic = false;
  //   rb.useGravity = false;
  //
  //   JointLimits limits = hinge.limits;
  //   limits.min = min;
  //   limits.max = max;
  //   hinge.limits = limits;
  //   hinge.useLimits = true;
  //
  //   hinge.useMotor = true;
  // }

  void Start() {
    var rb = GetComponent<Rigidbody>();
    if (rb == null) {
      Debug.LogError("No Rigidbody on DoorHandleAnchor!");
      return;
    }

    rb.isKinematic = false;
    rb.useGravity = false;

    // // --- LIMITS ---
    // JointLimits limits = hinge.limits;
    // limits.min = 0f;  // starting angle
    // limits.max = 45f; // stop at 45 degrees
    // hinge.limits = limits;
    // hinge.useLimits = true;

    // --- MOTOR ---
    JointMotor motor = hinge.motor;
    motor.force = 1000f;
    motor.targetVelocity = 90f; // positive direction (toward max limit)
    motor.freeSpin = false;
    hinge.motor = motor;

    hinge.useMotor = true;
  }

  void Update() {
    // float angle = hinge.angle;
    //
    // // reached max? flip direction
    // if (angle >= max - 0.5f) // small buffer to avoid jitter
    //   direction = -1;
    //
    // // reached min? flip direction
    // if (angle <= min + 0.5f)
    //   direction = 1;
    //
    // JointMotor m = hinge.motor;
    // m.force = 1000f;
    // m.targetVelocity = direction * speed;
    // hinge.motor = m;

    // bool useRight =
    //     d_handService.TryGetPalmPose(Handedness.Right, out var
    //     rightPalmPose);
    // bool useLeft =
    //     d_handService.TryGetPalmPose(Handedness.Left, out var leftPalmPose);
    //
    // bool rightInHandle =
    //     useRight && doorHandleRegion.bounds.Contains(rightPalmPose.position);
    // bool leftInHandle =
    //     useLeft && doorHandleRegion.bounds.Contains(leftPalmPose.position);
    //
    // switch (mode) {
    // case DoorHandleMode.None:
    //   // kick off interactions
    //   if (rightInHandle) {
    //     HandLogger.Log("RIGHT GRABBING");
    //     mode = DoorHandleMode.Rotating;
    //     activeHand = Handedness.Right;
    //     BeginRotateInteraction(rightPalmPose);
    //   } else if (leftInHandle) {
    //     HandLogger.Log("LEFT GRABBING");
    //     mode = DoorHandleMode.Rotating;
    //     activeHand = Handedness.Left;
    //     BeginRotateInteraction(leftPalmPose);
    //   }
    //   break;
    // case DoorHandleMode.Rotating:
    //   // check if still in region
    //   if (!d_handService.TryGetPalmPose(activeHand, out var rotatePose)) {
    //     EndInteraction();
    //     return;
    //   }
    //
    //   bool handInsideRotateRegion =
    //       doorHandleRegion.bounds.Contains(rotatePose.position);
    //
    //   if (handInsideRotateRegion) {
    //     Debug.Log("ROTATING");
    //     RotateHandleOnPalmPose(rotatePose);
    //   } else {
    //     EndInteraction();
    //   }
    //   break;
    // }
  }

  private void BeginRotateInteraction(Pose palmPose) {
    Debug.LogWarning("Beginning rotate interaction");

    // 1. Remember where the hinge and palm started
    startHandleAngle = hinge.angle;
    startPalmYaw = palmPose.rotation.eulerAngles.y;

    // 2. Enable motor and set base settings
    var motor = hinge.motor;
    motor.force = motorForce;
    motor.targetVelocity = 0f; // we'll update this every frame
    hinge.motor = motor;
    hinge.useMotor = true;
  }

  private void RotateHandleOnPalmPose(Pose palmPose) {
    // current palm yaw around Y axis
    float currentYaw = palmPose.rotation.eulerAngles.y;

    // signed smallest difference, handles 0/360 wrap
    float yawDelta = Mathf.DeltaAngle(startPalmYaw, currentYaw);

    // map palm twist -> desired hinge angle
    float desiredAngle = Mathf.Clamp(startHandleAngle + yawDelta,
                                     -maxHandleAngle, maxHandleAngle);

    // how far we are from desired angle
    float error = desiredAngle - hinge.angle;
    Debug.LogWarning("Apply motor!!!");

    // proportional motor control: velocity ~ error
    var motor = hinge.motor;
    motor.force = motorForce;
    motor.targetVelocity = error * motorSpeed; // sign comes from error
    hinge.motor = motor;

    Debug.Log(
        $"Rotating: startHandle={startHandleAngle:F1}, desired={desiredAngle:F1}, " +
        $"current={hinge.angle:F1}, error={error:F1}, targetVel={motor.targetVelocity:F1}");
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

  private void EndInteraction() {
    mode = DoorHandleMode.None;
    hinge.useMotor = false;
  }
}
