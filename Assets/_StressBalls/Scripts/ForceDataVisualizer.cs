using UnityEngine;
using UnityEngine.UI;

namespace StressBalls {
    public class ForceDataVisualizer : MonoBehaviour {

        [SerializeField] private Text text;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private bool drawDebugRay;
        [SerializeField] private Color debugRayColor;

        public void UpdateForce(Force force) {
            if (text) {
                text.text = force.Value.ToString("N2");
            }
            if (lineRenderer) {
                lineRenderer.SetPosition(0, force.Origin);
                lineRenderer.SetPosition(1, force.Origin + force.Vector);
            }
            if (drawDebugRay) {
                Debug.DrawRay(force.Origin, force.Vector, debugRayColor);
            }
        }

        public void UpdateVector(Vector3 origin, Vector3 vector) {
            if (text) {
                text.text = vector.magnitude.ToString("N2");
            }
            if (lineRenderer) {
                lineRenderer.SetPosition(0, origin);
                lineRenderer.SetPosition(1, origin + vector);
            }
            if (drawDebugRay) {
                Debug.DrawRay(origin, vector, debugRayColor);
            }
        }
    }
}