using UnityEngine;

public class Rotator : MonoBehaviour {

    public Transform pointA;
    public Transform pointB;
    public Transform pointC;

    [ContextMenu("GetPositions")]
    public void GetPositions() {
        var pointAPosition = pointA.localPosition;
        var pointBPosition = pointB.localPosition;
        var pointCPosition = pointC.localPosition;
        Debug.Log($"{pointAPosition}, {pointBPosition}, {pointCPosition}");
    }

    [ContextMenu("Reset Point")]
    public void ResetPoint() {
        pointC.position = Vector3.zero;
    }

    [ContextMenu("Rotate")]
    public void Rotate() {
        var pointBPosition = pointB.position;
        var pointBLocalPosition = pointB.localPosition;

        Quaternion rotation = pointB.parent.rotation;

        var rotatedPoint = RotateAroundPivot(pointBPosition, pointB.parent.position, Quaternion.Inverse(rotation));

        pointC.position = rotatedPoint;

        Vector3 offset = pointB.parent.position - pointA.parent.position;

        pointC.position -= offset;

        Debug.Log(pointA.position - pointC.position);
        //Debug.Log($"{pointBPosition}, {pointBLocalPosition}, {rotatedPoint}");
    }

    private Vector3 RotateAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation) =>
        rotation * (point - pivot) + pivot;
}
