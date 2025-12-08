using UnityEngine;

public class CompositionRoot : MonoBehaviour {
  [Header("Services")]
  [SerializeField]
  HandTrackingManager handTrackingManager;

  [Header("Consumers")]
  [SerializeField]
  DrawerController[] drawers;

  void Awake() {
    foreach (var drawer in drawers) {
      drawer.Initialize(handTrackingManager);
    }
  }
}
