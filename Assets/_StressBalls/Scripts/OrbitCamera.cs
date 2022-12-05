using UnityEngine;

namespace StressBalls {
    public class OrbitCamera : MonoBehaviour {

        [SerializeField] private Camera camera;
        [SerializeField] private Transform target;
        [SerializeField] private float distance = 10f;
        [SerializeField] private MouseButton mouseButton;

        private Vector3 _previousPosition;
        private int _mouseButton;

        private enum MouseButton {
            Left,
            Right,
            Middle
        }

        private void Start() {
            _mouseButton = (int)mouseButton;
        }

        private void Update() {
            if (Input.GetMouseButtonDown(_mouseButton)) {
                UpdatePreviousPosition(GetMousePoint());
            }

            if (Input.GetMouseButton(_mouseButton)) {
                Vector3 mousePoint = GetMousePoint();
                Vector3 direction = (_previousPosition - mousePoint).normalized;

                RotateCamera(direction);

                UpdatePreviousPosition(mousePoint);
            }
        }

        private void RotateCamera(Vector3 direction) {
            camera.transform.position = target.position;
            camera.transform.Rotate(Vector3.right, direction.y * 180f * Time.deltaTime);
            camera.transform.Rotate(Vector3.up, -direction.x * 180f * Time.deltaTime, Space.World);
            camera.transform.Translate(Vector3.back * distance);
        }

        private void UpdatePreviousPosition(Vector3 position) =>
            _previousPosition = position;

        private Vector3 GetMousePoint() =>
            camera.ScreenToViewportPoint(Input.mousePosition);
    }
}
