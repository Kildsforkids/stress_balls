using UnityEngine;

namespace StressBalls {
    public class Force {

        public Vector3 Vector { get; private set; }
        public Vector3 Origin { get; private set; }
        public float Value => Vector.magnitude * _coef;

        private float _coef;

        public Force() {
            Origin = Vector3.zero;
            Vector = Vector3.zero;
            _coef = 1f;
        }

        public void Update(Vector3 origin, Vector3 vector) {
            Origin = origin;
            Vector = vector;
        }

        public void SetCoef(float coef) {
            _coef = coef;
        }
    }
}