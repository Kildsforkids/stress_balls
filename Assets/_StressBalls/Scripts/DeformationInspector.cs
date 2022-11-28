using Obi;
using UnityEngine;

public class DeformationInspector : MonoBehaviour {

    [SerializeField] private ObiSoftbody softbody;
    [SerializeField] private float deformationScaling = 10f;

    private void Start() {
        softbody.OnEndStep += Softbody_OnEndStep;
    }

    private void OnDestroy() {
        softbody.OnEndStep -= Softbody_OnEndStep;
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
                    //int offset = softbody.solverBatchOffsets[(int)Oni.ConstraintType.ShapeMatching][j];

                    // use frobenius norm to estimate deformation.
                    //float deformation = solverBatch.linearTransforms[offset + i].FrobeniusNorm() - 2;
                    float deformation = solverBatch.linearTransforms[i].FrobeniusNorm() - 2;

                    //Debug.Log($"{offset} | {deformation}");
                    Debug.Log(deformation);

                    //for (int k = 0; k < batch.numIndices[i]; ++k) {
                    //    int p = batch.particleIndices[batch.firstIndex[i] + k];
                    //    int or = softbody.solverIndices[p];
                    //    if (softbody.solver.invMasses[or] > 0) {
                    //        norms[p] += deformation;
                    //        counts[p]++;
                    //    }
                    //}
                }
            }
        }
    }
}
