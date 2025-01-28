using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TrashCounter : BaseCounter {

    public static event EventHandler OnObjectTrash;

    new public static void ResetStaticData() {
        OnObjectTrash = null;
    }

    public override void Interact(Player player) {
        if (player.HasKitchObject()) {
            KitchenObject.DestroyKitchenObject(player.GetKitchObject());
            InteractLogicServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc() {
        OnObjectTrash?.Invoke(this, EventArgs.Empty);
    }
}
