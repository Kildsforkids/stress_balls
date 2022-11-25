using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(Rigidbody), typeof(MeshCollider))]
public class MeshDeformer : MonoBehaviour {

    [SerializeField] private float springForce = 20f;
    [SerializeField] private float damping = 5f;
    [SerializeField] private MeshCollider meshCollider;

    private Mesh _deformingMesh;
    private Rigidbody _rigidbody;
    private Vector3[] _originalVertices;
    private Vector3[] _displacedVertices;
    private Vector3[] _vertexVelocities;
    private float _uniformScale = 1f;

    private void Awake() {
        _deformingMesh = GetComponent<MeshFilter>().mesh;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start() {
        _uniformScale = Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
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
        LogInfo();
        ClearOnFall();
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

    private void LogInfo() {
        //Debug.Log($"Velocity: {velocity}");
    }

    private void OnCollisionEnter(Collision collision) {
        //float velocity = GetVelocity();
        ContactPoint impactPoint = collision.GetContact(0);
        Vector3 impulse = collision.impulse;
        float relativeVelocity = collision.relativeVelocity.sqrMagnitude;

        //var originVelocity = _rigidbody.velocity;
        //Debug.Log($"Collided with {collision.transform.name} on velocity {velocity} and point {impactPoint.point}");
        //Debug.Log($"Impulse {impulse}");
        //Debug.Log($"Relative velocity {relativeVelocity}");
        //Debug.Log($"Original velocity {originVelocity}");
        //Debug.Log("----------------------------");
        //Debug.DrawRay(transform.position, impulse * 10f);


        if (relativeVelocity > 0.3f) {

            Vector3 mid = Vector3.zero;
            Vector3 normal = Vector3.zero;

            for (int i = 0; i < collision.contactCount; i++) {
                var contact = collision.GetContact(i);
                mid += contact.point;
                normal += contact.normal;
            }

            mid /= collision.contactCount;
            normal /= collision.contactCount;

            //Debug.Log($"Relative velocity magnitude: {relativeVelocity.sqrMagnitude}");
            //float forceOffset = 0.1f;
            //Vector3 point = impactPoint.point;
            Vector3 point = mid;
            //Vector3 point = _rigidbody.centerOfMass;
            //point += impactPoint.normal * forceOffset;
            //point += impulse * forceOffset;
            point += normal;
            AddDeformingForce(point, Mathf.Sqrt(relativeVelocity));
        }
    }

    private void ClearOnFall() {
        if (transform.position.y < -5f)
            Destroy(gameObject, 2f);
    }

    private float GetVelocity() => _rigidbody.velocity.magnitude;
}
