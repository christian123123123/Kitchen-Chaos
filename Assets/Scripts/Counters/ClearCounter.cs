using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter {
    [SerializeField] private KitchenObjectsSO kitchenObjectsSO;

    public override void Interact(Player player) {
        if (!HasKitchObject()) {
            if (player.HasKitchObject()) {
                player.GetKitchObject().SetKitchObjectParent(this);
            } else {
                return;
            }
        } else {
            if (player.HasKitchObject()) {
                if (player.GetKitchObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(GetKitchObject().GetKitchenObjectsSO())) {
                        KitchenObject.DestroyKitchenObject(GetKitchObject());
                    }
                } else {
                    if (GetKitchObject().TryGetPlate(out plateKitchenObject)) {
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchObject().GetKitchenObjectsSO())) {
                            KitchenObject.DestroyKitchenObject(player.GetKitchObject());
                        }
                    }
                }
            } else {
                GetKitchObject().SetKitchObjectParent(player);
            }
        }
    }
}
