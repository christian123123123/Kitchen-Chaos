using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Windows;

public class CuttingCounter : BaseCounter, IHasProgress {

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;
    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData() {
        OnAnyCut = null;
    }


    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private int cuttingProgress;

    public override void Interact(Player player) {
        if (!HasKitchObject()) {
            if (player.HasKitchObject()) {
                if (HasRecipeWithInput(player.GetKitchObject().GetKitchenObjectsSO())) {
                    KitchenObject kitchenObject = player.GetKitchObject();
                    player.GetKitchObject().SetKitchObjectParent(this);
                    InteractLogicServerRpc();
                }
            } else {
                return;
            }
        } else {
            if (player.HasKitchObject()) {
                if (player.GetKitchObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(GetKitchObject().GetKitchenObjectsSO())) {
                        KitchenObject.DestroyKitchenObject(GetKitchObject());
                    }
                }
            } else {
                GetKitchObject().SetKitchObjectParent(player);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void InteractLogicServerRpc() {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    public void InteractLogicClientRpc() {
        cuttingProgress = 0;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
            progressNormalized = 0f
        });
    }



    public override void InteractAlternate(Player player) {
        if (HasKitchObject() && HasRecipeWithInput(GetKitchObject().GetKitchenObjectsSO())) {
            InteractAlternateLogicServerRpc();
            TestCuttingProgressServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractAlternateLogicServerRpc() {
        InteractAlternateLogicClientRpc();
    }

    [ClientRpc]
    private void InteractAlternateLogicClientRpc() {
        cuttingProgress++;
        OnCut?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSO(GetKitchObject().GetKitchenObjectsSO());
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
            progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
        });
    }


    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressServerRpc() {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSO(GetKitchObject().GetKitchenObjectsSO());
        if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax) {
            KitchenObjectsSO outputKitchenObjectSO = GetOutputForInput(GetKitchObject().GetKitchenObjectsSO());
            KitchenObject.DestroyKitchenObject(GetKitchObject());
            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
        }
    }

    private bool HasRecipeWithInput(KitchenObjectsSO input) {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSO(input);
        return cuttingRecipeSO != null;
    }

    private KitchenObjectsSO GetOutputForInput(KitchenObjectsSO input) {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSO(input);
        if (cuttingRecipeSO != null) {
            return cuttingRecipeSO.output;
        } else {
            return null;
        }
    }

    private CuttingRecipeSO GetCuttingRecipeSO(KitchenObjectsSO input) {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray) {
            if (cuttingRecipeSO.input == input) {
                return cuttingRecipeSO;
            }
        }
        return null;
    }
}
