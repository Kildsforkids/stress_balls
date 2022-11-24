using UnityEngine;

public class MeshDeformInput : MonoBehaviour {

    [SerializeField] private Camera camera;
    [SerializeField] private LayerMask layerMask;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Raycast(CreateRay());
        }
    }

    private void Raycast(Ray ray) {
        if (Physics.Raycast(ray, out RaycastHit hit, layerMask)) {
            Debug.DrawRay(ray.origin, ray.direction * 10f, Color.yellow, 2f);
            Debug.Log(hit.transform.name);
        }
    }

    private Ray CreateRay() =>
        camera.ScreenPointToRay(Input.mousePosition);
}
