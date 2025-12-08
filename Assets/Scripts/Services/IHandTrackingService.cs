using UnityEngine;
using UnityEngine.XR.Hands;

public enum Handedness { Left, Right }

public interface IHandTrackingService {
  /// tries to get palm, gives Pose of palm if possible
  bool TryGetPalmPose(Handedness hand, out Pose pose);

  /// determines if a hand is tracked
  bool IsHandTracked(Handedness hand);
}
