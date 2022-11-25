using UnityEngine;

public class JellyMesh : MonoBehaviour {

    [SerializeField] private float intensity = 1f;
    [SerializeField] private float mass = 1f;
    [SerializeField] private float stiffness = 1f;
    [SerializeField] private float damping = 0.75f;
    [SerializeField] MeshColliderUpdater meshColliderUpdater;

    private Mesh _originalMesh;
    private Mesh _meshClone;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private JellyVertex[] _jellyVertices;
    private Vector3[] _vertexArray;

    private void Awake() {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start() {
        _originalMesh = _meshFilter.sharedMesh;
        _meshClone = Instantiate(_originalMesh);
        meshColliderUpdater.SetMesh(_meshClone);
        _meshFilter.sharedMesh = _meshClone;
        int vertexCount = _meshClone.vertexCount;
        _jellyVertices = new JellyVertex[vertexCount];
        for (int i = 0; i < vertexCount; i++) {
            _jellyVertices[i] = new JellyVertex(i, transform.TransformPoint(_meshClone.vertices[i]));
        }
    }

    private void FixedUpdate() {
        _vertexArray = _originalMesh.vertices;
        for (int i = 0; i < _jellyVertices.Length; i++) {
            int jellyId = _jellyVertices[i].Id;
            Vector3 target = transform.TransformPoint(_vertexArray[jellyId]);
            float intensity = (1f - (_meshRenderer.bounds.max.y - target.y) / _meshRenderer.bounds.size.y) * this.intensity;
            _jellyVertices[i].Shake(target, mass, stiffness, damping);
            target = transform.InverseTransformPoint(_jellyVertices[i].Position);
            _vertexArray[jellyId] = Vector3.Lerp(_vertexArray[jellyId], target, intensity);
        }
        _meshClone.vertices = _vertexArray;
    }

    private class JellyVertex {

        public int Id;
        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Force;

        public JellyVertex(int id, Vector3 position) {
            Id = id;
            Position = position;
        }

        public void Shake(Vector3 target, float mass, float stiffness, float damping) {
            Force = (target - Position) * stiffness;
            Debug.DrawRay(Position, Force);
            Velocity = (Velocity + Force / mass) * damping;
            Position += Velocity;
            if ((Velocity + Force + Force / mass).magnitude < 0.001f) {
                Position = target;
            }
        }
    }
}
