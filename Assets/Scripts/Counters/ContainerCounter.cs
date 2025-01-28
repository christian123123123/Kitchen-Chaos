using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter {

    public event EventHandler OnPlayerGrabObject;
    [SerializeField] private KitchenObjectsSO kitchenObjectsSO;

    public override void Interact(Player player) {
        if (!player.HasKitchObject()) {
            KitchenObject.SpawnKitchenObject(kitchenObjectsSO, player);
            InteractLogicServerRpc();
        } else {
            return;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc() {
        OnPlayerGrabObject?.Invoke(this, EventArgs.Empty);
    }
}
