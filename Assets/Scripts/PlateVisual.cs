using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateVisual : MonoBehaviour {

    [Serializable]
    public struct KitchenObjectsSo_GameObjects {
        public KitchenObjectsSO kitchenObjectsSo;
        public GameObject gameObject;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectsSo_GameObjects> kitchenObjectsSoGameObjects;

    private void Start() {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
        foreach (KitchenObjectsSo_GameObjects kitchenObjectsSoGameObjects in kitchenObjectsSoGameObjects) {
            kitchenObjectsSoGameObjects.gameObject.SetActive(false);
        }
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedArgs e) {
        foreach (KitchenObjectsSo_GameObjects kitchenObjectsSoGameObjects in kitchenObjectsSoGameObjects) {
            if (kitchenObjectsSoGameObjects.kitchenObjectsSo == e.kitchenObjectsSo) {
                kitchenObjectsSoGameObjects.gameObject.SetActive(true);
            }

        }
    }
}
