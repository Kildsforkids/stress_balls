using UnityEngine;

public class MassPoint : MonoBehaviour {

    public Vector3 OriginPosition { get; private set; }

    private Rigidbody _rigidbody;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start() {
        OriginPosition = transform.position;
    }

    public void AddSpringForce(float force) {
        Vector3 direction = OriginPosition - transform.position;
        _rigidbody.AddForce(direction * force);
    }
}
