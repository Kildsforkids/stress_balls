using Obi;
using UnityEngine;
using UnityEngine.Events;

namespace StressBalls {
    public class ParticlesInspector : MonoBehaviour {

        [SerializeField] private ObiSolver solver;
        [SerializeField] private ObiParticlePicker particlePicker;
        [SerializeField] private ObiActorBlueprint blueprint;
        [Header("For Debug")]
        [SerializeField] private GameObject markerPrefab;
        [SerializeField] private ObiSoftbody softbody;

        public UnityEvent<Force> OnElasticityChanged;

        private Particle _particle;
        private Transform _marker;
        private bool _isUsed;

        float[] norms;
        int[] counts;

        private void Start() {
            norms = new float[softbody.particleCount];
            counts = new int[softbody.particleCount];
            CreateMarker();

            softbody.OnEndStep += Softbody_OnEndStep;
        }

        private void OnDestroy() {
            softbody.OnEndStep -= Softbody_OnEndStep;
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

        private void Softbody_OnEndStep(ObiActor actor, float substepTime) {
            var dc = softbody.GetConstraintsByType(Oni.ConstraintType.ShapeMatching) as ObiConstraints<ObiShapeMatchingConstraintsBatch>;
            var sc = softbody.solver.GetConstraintsByType(Oni.ConstraintType.ShapeMatching) as ObiConstraints<ObiShapeMatchingConstraintsBatch>;

            if (dc != null && sc != null) {

                for (int j = 0; j < dc.batches.Count; ++j) {
                    var batch = dc.batches[j];
                    var solverBatch = sc.batches[j];

                    for (int i = 0; i < batch.activeConstraintCount; i++) {
                        // use rotation-invariant Frobeniums norm to get amount of deformation.
                        int offset = softbody.solverBatchOffsets[(int)Oni.ConstraintType.ShapeMatching][j];

                        // use frobenius norm to estimate deformation.
                        float deformation = solverBatch.linearTransforms[offset + i].FrobeniusNorm() - 2;

                        for (int k = 0; k < batch.numIndices[i]; ++k) {
                            int p = batch.particleIndices[batch.firstIndex[i] + k];
                            int or = softbody.solverIndices[p];
                            if (softbody.solver.invMasses[or] > 0) {
                                norms[p] += deformation;
                                counts[p]++;
                            }
                        }
                    }
                }

                // average force over each particle, map to color, and reset forces:
                for (int i = 0; i < softbody.solverIndices.Length; ++i) {
                    if (counts[i] > 0) {
                        //int solverIndex = softbody.solverIndices[i];
                        Debug.Log($"{norms[i]} | {counts[i]}");
                        //softbody.solver.colors[solverIndex] = gradient.Evaluate(norms[i] / counts[i] * deformationScaling + 0.5f);
                        norms[i] = 0;
                        counts[i] = 0;
                    }
                }

                //var surfaceBlueprint = softbody.softbodyBlueprint as ObiSoftbodySurfaceBlueprint;
                //if (surfaceBlueprint != null && skin != null && skin.sharedMesh != null) {
                //    for (int i = 0; i < colors.Length; ++i) {
                //        int particleIndex = surfaceBlueprint.vertexToParticle[i];
                //        colors[i] = softbody.solver.colors[particleIndex];
                //    }
                //    skin.sharedMesh.colors = colors;
                //}
            }
        }

        private void Pick() {
            _particle.UpdateForces();
            OnElasticityChanged?.Invoke(_particle.ElasticityForce);

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
