using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour {

    [SerializeField] private float springForce = 20f;
    [SerializeField] private float damping = 5f;
    [SerializeField] private MeshCollider meshCollider;

    private Mesh _deformingMesh;
    private Vector3[] _originalVertices;
    private Vector3[] _displacedVertices;
    private Vector3[] _vertexVelocities;
    private float _uniformScale = 1f;

    private void Start() {
        _uniformScale = Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        _deformingMesh = GetComponent<MeshFilter>().mesh;
        _originalVertices = _deformingMesh.vertices;
        _displacedVertices = new Vector3[_originalVertices.Length];
        for (int i = 0; i < _originalVertices.Length; i++) {
            _displacedVertices[i] = _originalVertices[i];
        }
        _vertexVelocities = new Vector3[_originalVertices.Length];
    }

    private void Update() {
        for (int i = 0; i < _displacedVertices.Length; i++) {
            UpdateVertex(i);
        }
        _deformingMesh.vertices = _displacedVertices;
        _deformingMesh.RecalculateNormals();
        UpdateMeshCollider();
    }

    public void AddDeformingForce(Vector3 point, float force) {
        point = transform.InverseTransformPoint(point);
        for (int i = 0; i < _displacedVertices.Length; i++) {
            AddForceToVertex(i, point, force);
        }
    }

    private void AddForceToVertex(int i, Vector3 point, float force) {
        Vector3 pointToVertex = _displacedVertices[i] - point;
        pointToVertex *= _uniformScale;
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
        float velocity = attenuatedForce * Time.deltaTime;
        _vertexVelocities[i] += pointToVertex.normalized * velocity;
    }

    private void UpdateVertex(int i) {
        Vector3 velocity = _vertexVelocities[i];
        Vector3 displacement = _displacedVertices[i] - _originalVertices[i];
        displacement *= _uniformScale;
        velocity -= displacement * springForce * Time.deltaTime;
        velocity *= 1f - damping * Time.deltaTime;
        _vertexVelocities[i] = velocity;
        _displacedVertices[i] += velocity * (Time.deltaTime / _uniformScale);
    }

    private void UpdateMeshCollider() {
        if (!meshCollider) return;
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = _deformingMesh;
    }
}
