using UnityEngine;

public class MeshDeformerInput : MonoBehaviour {

    [SerializeField] private Camera camera;
    [SerializeField] private float force = 10f;

    private void Update() {
        if (Input.GetMouseButton(0)) {
            HandleInput();
        }
    }

    private void HandleInput() {
        Ray inputRay = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(inputRay, out RaycastHit hit)) {
            MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
            if (deformer) {
                Vector3 point = hit.point;
                deformer.AddDeformingForce(point, force);
                Debug.DrawLine(camera.transform.position, point);
            }
        }
    }
}
