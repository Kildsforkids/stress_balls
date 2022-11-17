using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour {

    private Mesh _deformingMesh;
    private Vector3[] _originalVertices;
    private Vector3[] _displacedVertices;
    private Vector3[] _vertexVelocities;

    private void Start() {
        _deformingMesh = GetComponent<MeshFilter>().mesh;
        _originalVertices = _deformingMesh.vertices;
        _displacedVertices = new Vector3[_originalVertices.Length];
        for (int i = 0; i < _originalVertices.Length; i++) {
            _displacedVertices[i] = _originalVertices[i];
        }
        _vertexVelocities = new Vector3[_originalVertices.Length];
    }

    public void AddDeformingForce(Vector3 point, float force) {

    }
}
