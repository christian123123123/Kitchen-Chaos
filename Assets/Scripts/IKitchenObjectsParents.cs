using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IKitchenObjectsParents {
    public Transform GetKitchenObjectFollowTransform();
    public void SetKitchObject(KitchenObject kitchenObject);
    public KitchenObject GetKitchObject();
    public void ClearKitchenObject();
    public bool HasKitchObject();

    public NetworkObject GetNetworkObject();
}
