using Obi;
using UnityEngine;

[RequireComponent(typeof(ObiActor))]
public class ActorCOM : MonoBehaviour {

    private ObiActor _actor;

    private void Awake() {
        _actor = GetComponent<ObiActor>();
    }

    private void OnDrawGizmos() {
        if (_actor == null || !_actor.isLoaded) return;
        Gizmos.color = Color.red;
        Vector4 com = Vector4.zero;
        float massAccumulator = 0f;

        for (int i = 0; i < _actor.solverIndices.Length; i++) {
            int solverIndex = _actor.solverIndices[i];
            float invMass = _actor.solver.invMasses[solverIndex];

            if (invMass > 0f) {
                massAccumulator += 1.0f / invMass;
                com += _actor.solver.positions[solverIndex];
            }
        }

        com /= massAccumulator;
        Gizmos.DrawWireSphere(com, 0.1f);
    }
}
