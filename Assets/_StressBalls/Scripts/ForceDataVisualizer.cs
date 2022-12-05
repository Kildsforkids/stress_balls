using UnityEngine;
using UnityEngine.UI;

namespace StressBalls {
    public class ForceDataVisualizer : MonoBehaviour {

        [SerializeField] private Text text;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private bool drawDebugRays;

        public void UpdateForce(Force force) {
            if (text) {
                text.text = force.Value.ToString("N2");
            }
            if (lineRenderer) {
                lineRenderer.SetPosition(0, force.Origin);
                lineRenderer.SetPosition(1, force.Origin + force.Vector);
            }
            if (drawDebugRays) {
                Debug.DrawRay(force.Origin, force.Vector, Color.green);
            }
        }
    }
}