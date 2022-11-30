using Obi;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ObiParticlePicker))]
public class ParticleInspector : MonoBehaviour {

    [SerializeField] private Transform marker;
    [SerializeField] private Text debugText;
    [SerializeField] private ObiActorBlueprint blueprint;

    private ObiParticlePicker _picker;
    private ObiParticlePicker.ParticlePickEventArgs _pickArgs;

    private bool _isUsed;

    private void Awake() {
        _picker = GetComponent<ObiParticlePicker>();
    }

    private void OnEnable() {
        _picker.OnParticlePicked.AddListener(Picker_OnParticlePicked);
    }

    private void OnDisable() {
        _picker.OnParticlePicked.RemoveListener(Picker_OnParticlePicked);
    }

    //private void OnDrawGizmos() {
    //    if (_pickArgs != null) {
    //        Gizmos.color = Color.red;
    //        Vector3 targetPosition = GetParticlePosition(_pickArgs.particleIndex);
    //        Gizmos.DrawSphere(targetPosition, 0.05f);
    //    }
    //}

    private void Update() {
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            TurnOn();
        }
        if (Input.GetKeyUp(KeyCode.LeftControl)) {
            TurnOff();
        }
    }

    private void FixedUpdate() {
        if (_pickArgs != null) {
            Pick();
        }
    }

    private void Pick() {
        Vector3 solverPosition = _picker.solver.transform.position;
        Quaternion solverRotation = _picker.solver.transform.rotation;
        Vector3 position = _picker.solver.positions[_pickArgs.particleIndex];
        Vector3 targetPosition = GetParticlePosition(_pickArgs.particleIndex);
        Vector3 velocity = _picker.solver.velocities[_pickArgs.particleIndex];
        Vector3 offset = solverPosition - transform.position;
        //Vector3 positionDelta = _picker.solver.positionDeltas[_pickArgs.particleIndex];
        Vector3 blueprintPosition = blueprint.GetParticlePosition(_pickArgs.particleIndex);

        var v = position - solverPosition; //the relative vector from P2 to P1.
        v = solverRotation * v; //rotatate
        position = solverPosition + v + offset; //bring back to world space

        if (marker) {
            marker.transform.position = targetPosition;
        }
        if (debugText) {
            debugText.text = velocity.ToString();
        }
        Debug.Log($"{position} | {blueprintPosition} | {offset}");
        Debug.DrawRay(targetPosition, velocity, Color.blue);
        Debug.DrawRay(targetPosition, (blueprintPosition - position), Color.green);
    }

    private void Picker_OnParticlePicked(ObiParticlePicker.ParticlePickEventArgs eventArgs) {
        if (!_isUsed) return;
        _pickArgs = eventArgs;
    }

    private void Picker_OnParticleReleased(ObiParticlePicker.ParticlePickEventArgs eventArgs) {
        _pickArgs = null;
    }

    private Vector3 GetParticlePosition(int particleIndex) {
        Vector3 position = _picker.solver.positions[_pickArgs.particleIndex];
        return _picker.solver.transform.TransformPoint(position);
    }

    private void TurnOn() {
        _isUsed = true;
    }

    private void TurnOff() {
        _isUsed = false;
    }
}
