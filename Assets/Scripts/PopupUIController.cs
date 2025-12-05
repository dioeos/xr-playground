using UnityEngine;
using TMPro;

public class PopupUIController : MonoBehaviour {
  [SerializeField]
  private GameObject plane;
  public TextMeshPro textField;

  // Start is called once before the first execution of Update after the
  // MonoBehaviour is created
  private void Awake() {
    if (plane == null) {
      Debug.LogError("Plane not assigned in Popup UI controller", this);
    }
  }

  private void Start() { Hide(); }

  public void Show(string tmpMessage) {
    Debug.Log("Showing popup with message: " + tmpMessage);
    textField.text = tmpMessage;
    plane.SetActive(true);
  }

  public void Hide() {
    Debug.Log("Hiding popup.");
    plane.SetActive(false);
  }

  void Update() {}
}
