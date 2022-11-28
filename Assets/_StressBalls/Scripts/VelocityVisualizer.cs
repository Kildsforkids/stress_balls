using Obi;
using UnityEngine;

[RequireComponent(typeof(ObiActor))]
public class VelocityVisualizer : MonoBehaviour {

    private ObiActor _actor;

    private void Awake() {
        _actor = GetComponent<ObiActor>();
    }

    private void OnDrawGizmos() {
        if (_actor == null || !_actor.isLoaded) return;
        //Gizmos.color = Color.red;
        //Gizmos.matrix = _actor.solver.transform.localToWorldMatrix;

        for (int i = 0; i < _actor.solverIndices.Length; i++) {
            int solverIndex = _actor.solverIndices[i];
            Vector3 position = _actor.solver.positions[solverIndex];
            Vector3 value = _actor.solver.rigidbodyLinearDeltas[solverIndex];
            Debug.DrawRay(position, value, Color.red);
            //Vector3 restPosition = _actor.solver.restPositions[solverIndex];
            //float restDensity = _actor.solver.restDensities[solverIndex];

            //var value = _actor.solver.positionDeltas[solverIndex];
            //Debug.Log(value);

            //Vector3 vector = restDensities;
            //Gizmos.DrawRay(position,
            //    vector * Time.fixedDeltaTime);
        }
    }
}
