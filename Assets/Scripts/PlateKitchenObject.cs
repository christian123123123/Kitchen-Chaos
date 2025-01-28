using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject {

    public event EventHandler<OnIngredientAddedArgs> OnIngredientAdded;

    public class OnIngredientAddedArgs : EventArgs {
        public KitchenObjectsSO kitchenObjectsSo;
    }

    [SerializeField] private List<KitchenObjectsSO> validKitchenObjectsSoList;
    private List<KitchenObjectsSO> kitchenObjectsSoList;

    protected override void Awake() {
        base.Awake();
        kitchenObjectsSoList = new List<KitchenObjectsSO>();
    }
    public bool TryAddIngredient(KitchenObjectsSO kitchenObjectsSo) {
        if (!validKitchenObjectsSoList.Contains(kitchenObjectsSo)) {
            return false;
        }
        if (kitchenObjectsSoList.Contains(kitchenObjectsSo)) {
            return false;
        }
        AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetKichenObjectSoIndex(kitchenObjectsSo));
        return true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex) {
        AddIngredientClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex) {
        KitchenObjectsSO kitchenObjectsSo = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        kitchenObjectsSoList.Add(kitchenObjectsSo);
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedArgs {
            kitchenObjectsSo = kitchenObjectsSo
        });
    }

    public List<KitchenObjectsSO> GetKitchenObjectsSoList() {
        return kitchenObjectsSoList;
    }
}
