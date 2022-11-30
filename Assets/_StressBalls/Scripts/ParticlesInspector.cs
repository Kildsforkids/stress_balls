using Obi;
using UnityEngine;
using UnityEngine.UI;

public class ParticlesInspector : MonoBehaviour {

    [SerializeField] private ObiSolver solver;
    [SerializeField] private Transform marker;
    [SerializeField] private Text debugText;
    [SerializeField] private ObiActorBlueprint blueprint;

    private ObiParticlePicker.ParticlePickEventArgs _pickArgs;

    private bool _isUsed;
    private Vector3 _elasticityForceVector;
    private Vector3 _targetPosition;

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
        var particleIndex = _pickArgs.particleIndex;
        //Vector3 targetPosition = GetParticlePosition(_pickArgs.particleIndex);
        Vector3 velocity = solver.velocities[particleIndex];

        _targetPosition = GetParticlePosition(_pickArgs.particleIndex);

        _elasticityForceVector = GetElasticityForceVector();

        float force = GetElasticityForce() * 100f;

        //Debug.Log($"{blueprintPosition - position} | {position}");

        if (marker) {
            marker.transform.position = _targetPosition;
        }
        if (debugText) {
            debugText.text = force.ToString("N2");
        }
        //Debug.Log($"{position} | {blueprintPosition} | {offset}");
        //Debug.DrawRay(targetPosition, velocity, Color.blue);

        Debug.DrawRay(_targetPosition, _elasticityForceVector, Color.green);
    }

    public float GetElasticityForce() => _elasticityForceVector.magnitude;

    private Vector3 GetElasticityForceVector() {
        var particleIndex = _pickArgs.particleIndex;
        var blueprintIndex = particleIndex;
        if (particleIndex > blueprint.particleCount) {
            blueprintIndex = particleIndex - blueprint.particleCount;
        }

        Vector3 solverPosition = solver.transform.position;
        //Quaternion solverRotation = solver.transform.rotation;

        ObiActor actor = solver.particleToActor[particleIndex].actor;

        //Vector3 position = solver.positions[_pickArgs.particleIndex];
        Vector3 targetPosition = GetParticlePosition(_pickArgs.particleIndex);

        Vector3 velocity = solver.velocities[particleIndex];
        Vector3 offset = actor.transform.position - solverPosition;

        //Debug.Log($"{particleIndex} | {blueprintIndex}");

        //Vector3 positionDelta = _picker.solver.positionDeltas[_pickArgs.particleIndex];
        Vector3 blueprintPosition = blueprint.GetParticlePosition(blueprintIndex);
        //Vector3 blueprintPosition = blueprint.positions[blueprintIndex];

        //position += offset;

        //var v = position - solverPosition; //the relative vector from P2 to P1.
        //v = solverRotation * v; //rotatate
        //position = solverPosition + v; //bring back to world space

        Vector3 position;

        Quaternion rotation = actor.transform.rotation;

        var rotatedPoint = RotateAroundPivot(targetPosition, actor.transform.position, Quaternion.Inverse(rotation));

        position = rotatedPoint;

        position -= offset;

        return blueprintPosition - position;
    }

    public void Picker_OnParticlePicked(ObiParticlePicker.ParticlePickEventArgs eventArgs) {
        if (!_isUsed) return;
        _pickArgs = eventArgs;
    }

    private Vector3 RotateAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation) =>
        rotation * (point - pivot) + pivot;

    private Vector3 GetParticlePosition(int particleIndex) {
        Vector3 position = solver.positions[_pickArgs.particleIndex];
        return solver.transform.TransformPoint(position);
    }

    private void TurnOn() {
        _isUsed = true;
    }

    private void TurnOff() {
        _isUsed = false;
    }
}
