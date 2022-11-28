using Obi;
using UnityEngine;

[RequireComponent(typeof(ObiParticlePicker))]
public class ParticleInspector : MonoBehaviour {

    private ObiParticlePicker _picker;

    private void Awake() {
        _picker = GetComponent<ObiParticlePicker>();
    }

    private void OnEnable() {
        _picker.OnParticlePicked.AddListener(OnParticlePicked);
    }

    private void OnDisable() {
        _picker.OnParticlePicked.RemoveListener(OnParticlePicked);
    }

    private void OnParticlePicked(ObiParticlePicker.ParticlePickEventArgs eventArgs) {
        //var go = new GameObject($"Point ${eventArgs.particleIndex}");
        Vector4 position4 = _picker.solver.positions[eventArgs.particleIndex];
        Vector3 position = _picker.solver.transform.TransformPoint(position4);
        //Vector3 position = eventArgs.worldPosition;
        //go.transform.position = position;
        //go.transform.SetParent(transform);
        Vector4 velocity = _picker.solver.velocities[eventArgs.particleIndex];
        //Debug.Log(velocity);
        Debug.DrawRay(position, velocity, Color.yellow, 5f);
    }
}
