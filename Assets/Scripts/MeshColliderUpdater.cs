using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class MeshColliderUpdater : MonoBehaviour {

    private Mesh _targetMesh;
    private MeshCollider _meshCollider;

    private void Awake() {
        _meshCollider = GetComponent<MeshCollider>();
    }

    private void Update() {
        UpdateMeshCollider();
    }

    private void UpdateMeshCollider() {
        if (!_targetMesh || !_meshCollider) return;
        _meshCollider.sharedMesh = null;
        _meshCollider.sharedMesh = _targetMesh;
    }

    public void SetMesh(Mesh mesh) {
        _targetMesh = mesh;
    }
}
