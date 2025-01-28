using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress {
    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSoArray;

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs : EventArgs {
        public State state;
    }

    public enum State {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);
    private NetworkVariable<float> timer = new NetworkVariable<float>(0f);
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);
    private FryingRecipeSO fryingRecipeSo;
    private BurningRecipeSO burningRecipeSo;

    public override void OnNetworkSpawn() {
        timer.OnValueChanged += Timer_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }

    private void Timer_OnValueChanged(float previousValue, float newValue) {
        float fryingTimerMax = fryingRecipeSo != null ? fryingRecipeSo.fryingTimerMax : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
            progressNormalized = timer.Value / fryingTimerMax,
        });
    }

    private void State_OnValueChanged(State previousState, State newState) {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
            state = state.Value,
        });

        if (state.Value == State.Burned || state.Value == State.Idle) {
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                progressNormalized = 0f
            });
        }
    }

    private void BurningTimer_OnValueChanged(float previousValue, float newValue) {
        float burningTimerMax = burningRecipeSo != null ? burningRecipeSo.burningTimerMax : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
            progressNormalized = burningTimer.Value / burningTimerMax,
        });
    }

    private void Update() {
        if (!IsServer) {
            return;
        }
        if (HasKitchObject()) {
            switch (state.Value) {
                case State.Idle:
                    break;
                case State.Frying:
                    timer.Value += Time.deltaTime;

                    if (timer.Value > fryingRecipeSo.fryingTimerMax) {
                        KitchenObject.DestroyKitchenObject(GetKitchObject());
                        KitchenObject.SpawnKitchenObject(fryingRecipeSo.output, this);
                        state.Value = State.Fried;
                        burningTimer.Value = 0;
                        SetBurningRecipeSOClientRpc(KitchenGameMultiplayer.Instance.GetKichenObjectSoIndex(GetKitchObject().GetKitchenObjectsSO()));
                    }
                    break;
                case State.Fried:
                    burningTimer.Value += Time.deltaTime;

                    if (burningTimer.Value > burningRecipeSo.burningTimerMax) {
                        KitchenObject.DestroyKitchenObject(GetKitchObject());
                        KitchenObject.SpawnKitchenObject(burningRecipeSo.output, this);
                        state.Value = State.Burned;
                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }

    public override void Interact(Player player) {
        if (!HasKitchObject()) {
            if (player.HasKitchObject()) {
                if (HasRecipeWithInput(player.GetKitchObject().GetKitchenObjectsSO())) {
                    KitchenObject kitchenObject = player.GetKitchObject();
                    kitchenObject.SetKitchObjectParent(this);
                    InteractLogicPlaceObjectOnCounterServerRpc(KitchenGameMultiplayer.Instance.GetKichenObjectSoIndex(kitchenObject.GetKitchenObjectsSO()));
                }
            } else {
                return;
            }
        } else {
            if (player.HasKitchObject()) {
                if (player.GetKitchObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(GetKitchObject().GetKitchenObjectsSO())) {
                        KitchenObject.DestroyKitchenObject(GetKitchObject());
                        SetStateIdleServerRpc();
                    }
                }
            } else {
                GetKitchObject().SetKitchObjectParent(player);
                SetStateIdleServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc() {
        state.Value = State.Idle;
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex) {
        timer.Value = 0f;
        state.Value = State.Frying;
        SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int kitchenObjectSoIndex) {
        fryingRecipeSo = GetFryingSORecipe(KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSoIndex));
    }

    [ClientRpc]
    private void SetBurningRecipeSOClientRpc(int kitchenObjectSoIndex) {
        burningRecipeSo = GetBurningSORecipe(KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSoIndex));
    }

    private bool HasRecipeWithInput(KitchenObjectsSO input) {
        FryingRecipeSO fryingRecipeSO = GetFryingSORecipe(input);
        return fryingRecipeSO != null;
    }

    private KitchenObjectsSO GetOutputForInput(KitchenObjectsSO input) {
        FryingRecipeSO fryingRecipeSO = GetFryingSORecipe(input);
        if (fryingRecipeSO != null) {
            return fryingRecipeSO.output;
        } else {
            return null;
        }
    }

    private FryingRecipeSO GetFryingSORecipe(KitchenObjectsSO input) {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray) {
            if (fryingRecipeSO.input == input) {
                return fryingRecipeSO;
            }
        }
        return null;
    }
    private BurningRecipeSO GetBurningSORecipe(KitchenObjectsSO input) {
        foreach (BurningRecipeSO burningRecipeSo in burningRecipeSoArray) {
            if (burningRecipeSo.input == input) {
                return burningRecipeSo;
            }
        }
        return null;
    }
    public bool IsFried() {
        return state.Value == State.Fried;
    }
}
