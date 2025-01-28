using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter {
    [SerializeField] private KitchenObjectsSO plateKitchenObjectsSo;
    [SerializeField] private float spawnPlateTimerMax = 4f;

    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlatePickup;


    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;

    private float spawnPlateTimer;

    private void Update() {
        if (!IsServer) {
            return;
        }

        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax) {
            spawnPlateTimer = 0f;
            if (CookingGameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax) {
                SpawnPlateServerRpc();
            }
        }
    }

    [ServerRpc]
    private void SpawnPlateServerRpc() {
        SpawnPlateClientRpc();
    }

    [ClientRpc]
    private void SpawnPlateClientRpc() {
        platesSpawnedAmount++;

        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }

    public override void Interact(Player player) {
        if (!player.HasKitchObject()) {
            if (platesSpawnedAmount > 0) {

                KitchenObject.SpawnKitchenObject(plateKitchenObjectsSo, player);
                InteractLogicServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc() {
        platesSpawnedAmount--;

        OnPlatePickup?.Invoke(this, EventArgs.Empty);

    }
}
