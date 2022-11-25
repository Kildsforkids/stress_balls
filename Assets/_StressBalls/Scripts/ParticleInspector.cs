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
        var go = new GameObject($"Point ${eventArgs.particleIndex}");
        go.transform.position = eventArgs.worldPosition;
        Vector4 velocity = _picker.solver.velocities[eventArgs.particleIndex];
        Debug.Log(velocity);
    }
}
