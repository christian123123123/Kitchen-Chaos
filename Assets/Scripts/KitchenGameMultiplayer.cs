using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameMultiplayer : NetworkBehaviour {
    public static KitchenGameMultiplayer Instance { get; private set; }
    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;

    private void Awake() {
        Instance = this;
    }

    public void SpawnKitchenObject(KitchenObjectsSO kitchenObjectsSO, IKitchenObjectsParents parent) {
        SpawnKitchenObjectServerRpc(GetKichenObjectSoIndex(kitchenObjectsSO), parent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectsSOIndex, NetworkObjectReference parentNetworkObjectReference) {
        KitchenObjectsSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectsSOIndex);
        Transform kitchenObjectsTransform = Instantiate(kitchenObjectSO.prefab);
        NetworkObject kitchenNetworkObject = kitchenObjectsTransform.GetComponent<NetworkObject>();
        kitchenNetworkObject.Spawn(true);
        KitchenObject kitchenObject = kitchenObjectsTransform.GetComponent<KitchenObject>();
        parentNetworkObjectReference.TryGet(out NetworkObject networkObject);
        IKitchenObjectsParents kitchenObjectParent = networkObject.GetComponent<IKitchenObjectsParents>();
        kitchenObject.SetKitchObjectParent(kitchenObjectParent);
    }

    public int GetKichenObjectSoIndex(KitchenObjectsSO kitchenObjectsSO) {
        return kitchenObjectListSO.kitchenObjectsSOList.IndexOf(kitchenObjectsSO);
    }

    public KitchenObjectsSO GetKitchenObjectSOFromIndex(int kitchenObjectsSOIndex) {
        return kitchenObjectListSO.kitchenObjectsSOList[kitchenObjectsSOIndex];
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject) {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference networkObjectReference) {
        networkObjectReference.TryGet(out NetworkObject networkObject);
        KitchenObject kitchenObject = networkObject.GetComponent<KitchenObject>();
        ClearKitchenObjectClientRpc(networkObjectReference);
        kitchenObject.DestroySelf();
    }


    [ClientRpc]
    private void ClearKitchenObjectClientRpc(NetworkObjectReference networkObjectReference) {
        networkObjectReference.TryGet(out NetworkObject networkObject);
        KitchenObject kitchenObject = networkObject.GetComponent<KitchenObject>();
        kitchenObject.ClearKitchenObjectOnParent();
    }


}
