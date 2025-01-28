using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BaseCounter : NetworkBehaviour, IKitchenObjectsParents {

    [SerializeField] private Transform counterTopPoint;
    public static event EventHandler OnDrop;

    public static void ResetStaticData() {
        OnDrop = null;
    }

    private KitchenObject kitchenObject;

    public virtual void Interact(Player player) {
    }

    public virtual void InteractAlternate(Player player) {
    }

    public Transform GetKitchenObjectFollowTransform() {
        return counterTopPoint;
    }

    public void SetKitchObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null) {
            OnDrop?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchObject() {
        return kitchenObject;
    }

    public void ClearKitchenObject() {
        kitchenObject = null;
    }

    public bool HasKitchObject() {
        return kitchenObject != null;
    }

    public NetworkObject GetNetworkObject() {
        return NetworkObject;
    }
}
