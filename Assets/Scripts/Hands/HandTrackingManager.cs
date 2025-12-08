using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class HandTrackingManager : MonoBehaviour, IHandTrackingService {

  XRHandSubsystem m_HandSubsystem;

  void Awake() {
    var loader = XRGeneralSettings.Instance.Manager.activeLoader;
    if (loader == null) {
      HandLogger.LogError(
          "No active XR loader yet – hand subsystem not available.");
      return;
    }
    m_HandSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>();
  }

  // FIX: REMOVE THIS (We are not tracking anything in this manager via
  // Update(), just getting subsystem)
  void Update() {
    if (m_HandSubsystem == null || !m_HandSubsystem.running) {
      HandLogger.Log("Did not find subsystem");
      return;
    }

    XRHand leftHand = m_HandSubsystem.leftHand;
    XRHand rightHand = m_HandSubsystem.rightHand;

    // LogJointData(leftHand, "[LEFT HAND]");
    // LogJointData(rightHand, "[RIGHT HAND]");
  }

  void LogJointData(XRHand hand, string handLabel) {
    for (var i = XRHandJointID.BeginMarker.ToIndex();
         i < XRHandJointID.EndMarker.ToIndex(); i++) {
      var jointData = hand.GetJoint(XRHandJointIDUtility.FromIndex(i));
      if (jointData.TryGetPose(out Pose pose)) {
        HandLogger.Log($"{handLabel} {jointData}: {pose}");
      }
    }
  }

  public bool TryGetPalmPose(Handedness hand, out Pose pose) {
    pose = default;
    /// returns bool and out var that is the pose of palm
    if (m_HandSubsystem == null || !m_HandSubsystem.running) {
      return false;
    }

    XRHand xrHand = hand == Handedness.Left ? m_HandSubsystem.leftHand
                                            : m_HandSubsystem.rightHand;

    if (!xrHand.isTracked)
      return false;

    // retrieve palmJoint via GetJoint(id) and determine pose
    var palmJoint = xrHand.GetJoint(XRHandJointID.Palm);
    return palmJoint.TryGetPose(out pose);
  }

  public bool IsHandTracked(Handedness hand) {

    if (m_HandSubsystem == null || !m_HandSubsystem.running)
      return false;

    XRHand xrHand = hand == Handedness.Left ? m_HandSubsystem.leftHand
                                            : m_HandSubsystem.rightHand;

    return xrHand.isTracked;
  }
}
