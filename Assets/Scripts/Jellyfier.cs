using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Jellyfier : MonoBehaviour {

    [SerializeField] private float bounceSpeed;
    [SerializeField] private float stiffness;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private bool updateMeshCollider;
    [SerializeField] private float updateSeconds = 1f;

    private MeshFilter _meshFilter;
    private Mesh _mesh;
    private JellyVertex[] _jellyVertices;
    private Vector3[] _meshVertices;
    private Coroutine _updateMeshColliderCoroutine;

    private void Awake() {
        _meshFilter = GetComponent<MeshFilter>();
    }

    private void Start() {
        _mesh = _meshFilter.mesh;
        GetVertices();
    }

    private void Update() {
        UpdateVertices();
        //UpdateMeshCollider();
    }

    private void OnEnable() {
        _updateMeshColliderCoroutine = StartCoroutine(UpdateMeshColliderCoroutine(updateSeconds));
    }

    private void OnDisable() {
        if (_updateMeshColliderCoroutine == null) return;
        StopCoroutine(_updateMeshColliderCoroutine);
        _updateMeshColliderCoroutine = null;
    }

    private IEnumerator UpdateMeshColliderCoroutine(float time) {
        var waitForSeconds = new WaitForSeconds(time);
        while (updateMeshCollider) {
            UpdateMeshCollider();
            yield return waitForSeconds;
        }
    }

    private void UpdateMeshCollider() {
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = _mesh;
    }

    private void OnCollisionEnter(Collision collision) {
        float impulse = collision.impulse.sqrMagnitude;
        if (impulse > 0.1f) {
            ContactPoint[] collisionPoints = new ContactPoint[collision.contactCount];
            collision.GetContacts(collisionPoints);
            for (int i = 0; i < collisionPoints.Length; i++) {
                //Debug.DrawRay(collisionPoints[i].point, collision.impulse, Color.red);
                //Debug.DrawRay(collisionPoints[i].point, collision.relativeVelocity);

                //Debug.Log(collision.impulse.sqrMagnitude);

                ApplyPressureToPoint(collisionPoints[i].point * 1.1f, impulse);
            }
        }
    }

    public void ApplyPressureToPoint(Vector3 point, float pressure) {
        for (int i = 0; i < _jellyVertices.Length; i++) {
            _jellyVertices[i].ApplyPressureToVertex(transform, point, pressure);
        }
    }

    private void UpdateVertices() {
        for (int i = 0; i < _jellyVertices.Length; i++) {
            JellyVertex jellyVertex = _jellyVertices[i];
            jellyVertex.UpdateVelocity(bounceSpeed);
            jellyVertex.Settle(stiffness);
            jellyVertex.UpdatePosition();

            //jellyVertex.Position += jellyVertex.Velocity * Time.deltaTime;
            _meshVertices[i] = jellyVertex.Position;
        }

        _mesh.vertices = _meshVertices;
        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
        _mesh.RecalculateTangents();
    }

    private void GetVertices() {
        int verticesCount = _mesh.vertexCount;
        _jellyVertices = new JellyVertex[verticesCount];
        _meshVertices = new Vector3[verticesCount];
        for (int i = 0; i < verticesCount; i++) {
            Vector3 vertex = _mesh.vertices[i];
            _jellyVertices[i] = new JellyVertex(i, vertex, vertex, Vector3.zero);
            _meshVertices[i] = vertex;
        }
    }
}
