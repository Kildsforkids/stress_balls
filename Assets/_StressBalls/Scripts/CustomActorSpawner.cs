using Obi;
using UnityEngine;

namespace StressBalls {
    public class CustomActorSpawner : MonoBehaviour {

        [SerializeField] private ObiActor template;

        [SerializeField] private int basePhase = 2;
        [SerializeField] private int maxInstances = 32;
        [SerializeField] private float spawnDelay = 0.3f;

        private int _phase = 0;
        private int _instances = 0;
        private float _timeFromLastSpawn = 0;

        void Update() {

            _timeFromLastSpawn += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space) && _instances < maxInstances && _timeFromLastSpawn > spawnDelay) {
                SpawnActor();
            }
        }

        private void SpawnActor() {
            Instantiate(template.gameObject, transform.position, transform.rotation, transform.parent)
                .GetComponent<ObiActor>().SetFilterCategory(basePhase + _phase);
            _phase++;
            _instances++;
            _timeFromLastSpawn = 0;
        }
    }
}
