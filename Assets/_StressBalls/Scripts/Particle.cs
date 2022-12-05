using Obi;
using UnityEngine;

namespace StressBalls {
    public class Particle {

        public int Index { get; private set; }
        public ObiSolver Solver { get; private set; }
        public Force ElasticityForce { get; private set; }

        public ObiActor Actor {
            get {
                return Solver.particleToActor[Index].actor;
            }
        }
        public ObiActorBlueprint Blueprint {
            get {
                return Actor.blueprint;
            }
        }
        public Vector3 Position {
            get {
                return Solver.transform.TransformPoint(LocalPosition);
            }
        }
        public Vector3 LocalPosition {
            get {
                return Solver.positions[Index];
            }
        }
        public Vector3 BlueprintPosition {
            get {
                return Blueprint.GetParticlePosition(BlueprintIndex);
            }
        }
        public int BlueprintIndex {
            get {
                return GetParticleBlueprintIndex();
            }
        }

        public Particle(int index, ObiSolver solver) {
            Index = index;
            Solver = solver;
            ElasticityForce = new Force();
        }

        public void UpdateForces() {
            ElasticityForce.Update(Position, GetElasticityForceVector());
        }

        private Vector3 GetElasticityForceVector() {
            Vector3 solverPosition = Solver.transform.position;
            Vector3 offset = Actor.transform.position - solverPosition;

            Quaternion rotation = Actor.transform.rotation;
            var rotatedPoint = RotateAroundPivot(
                Position,
                Actor.transform.position,
                Quaternion.Inverse(rotation)
            );
            Vector3 position = rotatedPoint;
            position -= offset;

            return BlueprintPosition - position;
        }

        private int GetParticleBlueprintIndex() {
            int blueprintIndex = Index;
            int particleCount = Actor.blueprint.particleCount;
            if (Index > particleCount) {
                if (Index % particleCount == 0) {
                    blueprintIndex = Index - particleCount * (Index / particleCount - 1);
                } else {
                    blueprintIndex = Index - particleCount * (Index / particleCount);
                }
            }
            return blueprintIndex;
        }

        private Vector3 RotateAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation) =>
            rotation * (point - pivot) + pivot;
    }
}