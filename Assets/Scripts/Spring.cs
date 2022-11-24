using UnityEngine;

public class Spring : MonoBehaviour {

    [SerializeField] private MassPoint pointA;
    [SerializeField] private MassPoint pointB;
    [SerializeField] private float stiffness;
    [SerializeField] private float damping;

    private float _restLength;

    private void Start() {
        _restLength = GetLength();
    }

    private void Update() {
        float force = GetForce();
        pointA.AddSpringForce(force * 10000f);
        pointB.AddSpringForce(force * 10000f);
        Debug.Log($"Length {GetLength()}, Rest {_restLength}, Force {force}");
    }

    private float GetForce() {
        return stiffness * (GetLength() - _restLength);
    }

    private float GetLength() {
        return (pointA.transform.position - pointB.transform.position).magnitude;
    }
}
