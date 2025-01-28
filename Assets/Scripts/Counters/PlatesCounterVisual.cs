using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour {
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private Transform plateVisualPrefab;
    [SerializeField] private PlatesCounter platesCounter;

    private List<GameObject> plateVisualGameObjectsList;

    private void Awake() {
        plateVisualGameObjectsList = new List<GameObject>();
    }

    private void Start() {
        platesCounter.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
        platesCounter.OnPlatePickup += PlatesCounter_OnPlatePickup;
    }

    private void PlatesCounter_OnPlatePickup(object sender, System.EventArgs e) {
        GameObject plateGameObject = plateVisualGameObjectsList[plateVisualGameObjectsList.Count - 1];
        plateVisualGameObjectsList.Remove(plateGameObject);
        Destroy(plateGameObject);
    }

    private void PlatesCounter_OnPlateSpawned(object sender, System.EventArgs e) {
        Transform plateVisualTransform = Instantiate(plateVisualPrefab, counterTopPoint);

        float plateOffsetY = .1f;

        plateVisualTransform.localPosition = new Vector3(0, plateOffsetY * plateVisualGameObjectsList.Count, 0);

        plateVisualGameObjectsList.Add(plateVisualTransform.gameObject);
    }
}
