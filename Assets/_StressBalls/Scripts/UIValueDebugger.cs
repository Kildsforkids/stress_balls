using UnityEngine;
using UnityEngine.UI;

namespace StressBalls {
    public class UIValueDebugger : MonoBehaviour {

        [SerializeField] private Text text;
        [SerializeField] private string appendedString;

        public void UpdateValue(float value) {
            text.text = value.ToString("N2") + appendedString;
        }
    }
}
