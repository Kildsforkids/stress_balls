using Obi;
using UnityEngine;
using UnityEngine.UI;

public class ParticlesInspector : MonoBehaviour {

    [SerializeField] private ObiSolver solver;
    [SerializeField] private GameObject markerPrefab;
    [SerializeField] private Text debugText;
    [SerializeField] private ObiActorBlueprint blueprint;
    [SerializeField] private LineRenderer lineRenderer;

    private ObiParticlePicker.ParticlePickEventArgs _pickArgs;

    private Transform _marker;
    private bool _isUsed;

    private void Start() {
        _marker = Instantiate(markerPrefab, Vector3.zero, Quaternion.identity).transform;
        _marker.gameObject.SetActive(false);
    }

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
        Vector3 targetPosition = GetParticlePosition(_pickArgs.particleIndex);
        Vector3 elasticityForceVector = GetElasticityForceVector();

        float force = GetElasticityForce();

        if (_marker) {
            _marker.gameObject.SetActive(true);
            _marker.position = targetPosition;
        }
        if (debugText) {
            debugText.text = force.ToString("N2");
        }

        Debug.DrawRay(targetPosition, elasticityForceVector, Color.green);
        DrawForceVector(targetPosition, elasticityForceVector);
    }

    public float GetElasticityForce() => GetElasticityForceVector().magnitude * 100f;

    private Vector3 GetElasticityForceVector() {
        var particleIndex = _pickArgs.particleIndex;
        var blueprintIndex = particleIndex;
        int particleCount = blueprint.particleCount;
        if (particleIndex > blueprint.particleCount) {
            if (particleIndex % particleCount == 0) {
                blueprintIndex = particleIndex - particleCount * (particleIndex / particleCount - 1);
            } else {
                blueprintIndex = particleIndex - particleCount * (particleIndex / particleCount);
            }
        }

        Vector3 solverPosition = solver.transform.position;

        ObiActor actor = solver.particleToActor[particleIndex].actor;

        Vector3 targetPosition = GetParticlePosition(_pickArgs.particleIndex);

        Vector3 offset = actor.transform.position - solverPosition;
        Vector3 blueprintPosition = blueprint.GetParticlePosition(blueprintIndex);

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

    private void DrawForceVector(Vector3 from, Vector3 forceVector) {
        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, from + forceVector);
    }

    private void TurnOn() {
        _isUsed = true;
    }

    private void TurnOff() {
        _isUsed = false;
    }
}
