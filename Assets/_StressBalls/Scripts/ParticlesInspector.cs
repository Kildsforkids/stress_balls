using Obi;
using UnityEngine;
using UnityEngine.Events;

namespace StressBalls {
    public class ParticlesInspector : MonoBehaviour {

        [SerializeField] private ObiSolver solver;
        [SerializeField] private ObiParticlePicker particlePicker;
        [Header("For Debug")]
        [SerializeField] private GameObject markerPrefab;

        public UnityEvent<Force> OnElasticityChanged;
        public UnityEvent<Vector3, Vector3> OnVelocityChange;
        public UnityEvent<float> OnElasticLimitChange;

        public Force ElasticityForce =>
            _particle != null ? _particle.ElasticityForce : new Force();

        private Particle _particle;
        private Transform _marker;
        private bool _isUsed;

        float[] norms;
        int[] counts;

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

        private void GetBatchInfo() {

            var dc = _particle.Actor.GetConstraintsByType(Oni.ConstraintType.ShapeMatching) as ObiConstraints<ObiShapeMatchingConstraintsBatch>;
            var sc = _particle.Solver.GetConstraintsByType(Oni.ConstraintType.ShapeMatching) as ObiConstraints<ObiShapeMatchingConstraintsBatch>;

            if (_particle != null) {
                if (dc != null && sc != null) {
                    for (int j = 0; j < dc.batches.Count; ++j) {
                        var batch = dc.batches[j];
                        var solverBatch = sc.batches[j];

                        for (int i = 0; i < batch.activeConstraintCount; i++) {
                            // use rotation-invariant Frobeniums norm to get amount of deformation.
                            int offset = _particle.Actor.solverBatchOffsets[(int)Oni.ConstraintType.ShapeMatching][j];

                            // use frobenius norm to estimate deformation.
                            float deformation = solverBatch.linearTransforms[offset + i].FrobeniusNorm() - 2;

                            for (int k = 0; k < batch.numIndices[i]; ++k) {
                                int p = batch.particleIndices[batch.firstIndex[i] + k];
                                int or = _particle.Actor.solverIndices[p];

                                //if (or == _particle.Index) {
                                //    Debug.Log($"Found in {i} activeConstraint of {j} batch");
                                //}
                                if (_particle.Solver.invMasses[or] > 0) {
                                    norms[p] += deformation;
                                    counts[p]++;
                                }
                            }
                        }
                    }
                }

                int particleIndex = _particle.BlueprintIndex;
                if (counts[particleIndex] > 0) {
                    float deformation = norms[particleIndex] / counts[particleIndex] * 20f + 0.5f;
                    //float deformation = norms[particleIndex] / counts[particleIndex];
                    //Color color = gradient.Evaluate(deformation);
                    //Debug.Log($"<color=green>[{particleIndex}] {deformation}</color>");
                    //Debug.Log(deformation.ToString("N2"));
                }

                //for (int i = 0; i < softbody.solverIndices.Length; ++i) {
                //    if (counts[i] > 0) {
                //        int solverIndex = softbody.solverIndices[i];
                //        //softbody.solver.colors[solverIndex] = gradient.Evaluate(norms[i] / counts[i] * deformationScaling + 0.5f);
                //        float deformation = norms[i] / counts[i] * 10f + 0.5f;
                //        if (solverIndex == particleIndex) {
                //            Debug.Log($"<color=green>[{solverIndex}] {deformation}</color>");

                //        } else {
                //            Debug.Log($"[{solverIndex}] {deformation}");
                //        }
                //        norms[i] = 0;
                //        counts[i] = 0;
                //    }
                //}
            }
        }

        private void Softbody_OnEndStep(ObiActor actor, float substepTime) {
            if (_particle == null) return;
            //GetBatchInfo();
        }

        private void Pick() {

            float stiffness = (_particle.Actor as ObiSoftbody).deformationResistance;

            _particle.UpdateForces();

            _particle.ElasticityForce.SetCoef(stiffness);

            OnElasticityChanged?.Invoke(_particle.ElasticityForce);
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
            if (_particle != null) {
                _particle.Actor.OnEndStep -= Softbody_OnEndStep;
            }
            _particle = new Particle(eventArgs.particleIndex, solver);
            _particle.Actor.OnEndStep += Softbody_OnEndStep;

            norms = new float[_particle.Actor.particleCount];
            counts = new int[_particle.Actor.particleCount];
        }

        private void TurnOn() {
            _isUsed = true;
        }

        private void TurnOff() {
            _isUsed = false;
        }
    }
}
