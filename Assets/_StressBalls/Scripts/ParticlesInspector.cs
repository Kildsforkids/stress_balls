using Obi;
using UnityEngine;
using UnityEngine.Events;

namespace StressBalls {
    public class ParticlesInspector : MonoBehaviour {

        [SerializeField] private ObiSolver solver;
        [SerializeField] private ObiParticlePicker particlePicker;
        [Header("For Debug")]
        [SerializeField] private GameObject markerPrefab;

        public UnityEvent<Force> OnElasticityChange;
        public UnityEvent<Vector3, Vector3> OnVelocityChange;
        public UnityEvent<float> OnElasticLimitChange;

        public Force ElasticityForce =>
            _particle != null ? _particle.ElasticityForce : new Force();

        private Particle _particle;
        private Transform _marker;
        private bool _isUsed;

        private void Start() {
            CreateMarker();
        }

        private void OnEnable() {
            particlePicker.OnParticlePicked.AddListener(Picker_OnParticlePicked);
        }

        private void OnDisable() {
            particlePicker.OnParticlePicked.RemoveListener(Picker_OnParticlePicked);
        }

        private void CreateMarker() {
            _marker = Instantiate(markerPrefab, Vector3.zero, Quaternion.identity).transform;
            _marker.gameObject.SetActive(false);
        }

        private void Update() {
            InputHandle();
        }

        private void InputHandle() {
            if (Input.GetKeyDown(KeyCode.LeftControl)) {
                TurnOn();
            }
            if (Input.GetKeyUp(KeyCode.LeftControl)) {
                TurnOff();
            }
        }

        private void FixedUpdate() {
            if (_particle != null) {
                Pick();
            }
        }

        private void Pick() {

            float stiffness = (_particle.Actor as ObiSoftbody).deformationResistance;

            _particle.UpdateForces();

            _particle.ElasticityForce.SetCoef(stiffness);

            OnElasticityChange?.Invoke(_particle.ElasticityForce);
            OnVelocityChange?.Invoke(_particle.Position, _particle.Velocity);

            float force = _particle.ElasticityForce.Value;
            float max = stiffness;
            float result = Mathf.Clamp01(force / max) * 100f;

            OnElasticLimitChange?.Invoke(result);

            if (_marker) {
                _marker.gameObject.SetActive(true);
                _marker.position = _particle.Position;
            }
        }

        private void Picker_OnParticlePicked(ObiParticlePicker.ParticlePickEventArgs eventArgs) {
            if (!_isUsed) return;
            _particle = new Particle(eventArgs.particleIndex, solver);
        }

        private void TurnOn() {
            _isUsed = true;
        }

        private void TurnOff() {
            _isUsed = false;
        }
    }
}
