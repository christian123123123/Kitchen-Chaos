using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour {


    [SerializeField] private KitchenObjectsSO kitchenObjectsSO;
    private IKitchenObjectsParents kitchenObjectsParents;
    private FollowTransform followTransform;

    public KitchenObjectsSO GetKitchenObjectsSO() { return kitchenObjectsSO; }

    protected virtual void Awake() {
        followTransform = GetComponent<FollowTransform>();
    }

    public void SetKitchObjectParent(IKitchenObjectsParents kitchenObjectsParents) {
        SetKitchObjectParentServerRpc(kitchenObjectsParents.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetKitchObjectParentServerRpc(NetworkObjectReference networkObjectReference) {
        SetKichObjectParentClientRpc(networkObjectReference);
    }

    [ClientRpc]
    private void SetKichObjectParentClientRpc(NetworkObjectReference clientObjectReference) {
        clientObjectReference.TryGet(out NetworkObject networkObject);
        IKitchenObjectsParents kitchenObjectsParents = networkObject.GetComponent<IKitchenObjectsParents>();

        if (this.kitchenObjectsParents != null) {
            this.kitchenObjectsParents.ClearKitchenObject();
        }
        this.kitchenObjectsParents = kitchenObjectsParents;
        if (kitchenObjectsParents.HasKitchObject()) {
            Debug.LogError("Counter already has a KitchenObject!");
        }
        kitchenObjectsParents.SetKitchObject(this);
        followTransform.SetTargetTransform(kitchenObjectsParents.GetKitchenObjectFollowTransform());
    }

    public IKitchenObjectsParents GetKitchenObjectsParents() { return kitchenObjectsParents; }

    public void DestroySelf() {
        Destroy(gameObject);
    }

    public void ClearKitchenObjectOnParent() {
        kitchenObjectsParents.ClearKitchenObject();
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {
        if (this is PlateKitchenObject) {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }

        plateKitchenObject = null;
        return false;
    }

    public static void SpawnKitchenObject(KitchenObjectsSO kitchenObjectsSO, IKitchenObjectsParents parent) {
        KitchenGameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectsSO, parent);
    }

    public static void DestroyKitchenObject(KitchenObject kitchenObject) {
        KitchenGameMultiplayer.Instance.DestroyKitchenObject(kitchenObject);
    }
}
