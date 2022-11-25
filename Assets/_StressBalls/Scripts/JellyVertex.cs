using UnityEngine;

public class JellyVertex {

    public Vector3 Velocity { get; private set; }
    public Vector3 Position { get; set; }

    public readonly int VerticeIndex;
    public readonly Vector3 InitialPosition;

    public JellyVertex(int verticeIndex, Vector3 initialPosition, Vector3 position, Vector3 velocity) {
        VerticeIndex = verticeIndex;
        InitialPosition = initialPosition;
        Position = position;
        Velocity = velocity;
    }

    public Vector3 GetDisplacement() {
        return Position - InitialPosition;
    }

    public void UpdateVelocity(float bounceSpeed) {
        Velocity -= GetDisplacement() * bounceSpeed * Time.deltaTime;
    }

    public void UpdatePosition() {
        Position += Velocity * Time.deltaTime;
    }

    public void Settle(float stiffness) {
        Velocity *= 1f - stiffness * Time.deltaTime;
    }

    public void ApplyPressureToVertex(Transform transform, Vector3 position, float pressure) {
        Vector3 distanceVerticePoint = Position - transform.InverseTransformPoint(position);
        float adaptedPressure = pressure / (1f + distanceVerticePoint.sqrMagnitude);
        float velocity = adaptedPressure * Time.deltaTime;
        Velocity += distanceVerticePoint.normalized * velocity;
    }
}