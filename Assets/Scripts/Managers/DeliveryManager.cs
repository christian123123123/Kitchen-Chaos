using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class DeliveryManager : NetworkBehaviour {


    public event EventHandler OnRecipeSpawn;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeFailed;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnMegaSucess;



    public static DeliveryManager Instance { get; private set; }
    [SerializeField] private LevelRecipeSO levelRecipeListSO;
    private List<RecipeSO> currentLevelRecipes;
    private List<RecipeSO> waitingRecipeSOList;

    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;
    private int recipesSucceeded;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
        recipesSucceeded = 0;
        int level = SceneManager.GetActiveScene().buildIndex;
        InitializeLevelRecipes(level);
    }

    public void InitializeLevelRecipes(int levelIndex) {
        LevelRecipeSO.LevelRecipe levelRecipe = levelRecipeListSO.levelRecipes
            .Find(lr => lr.levelIndex == levelIndex);

        if (levelRecipe != null) {
            currentLevelRecipes = levelRecipe.recipes;
        } else {
            Debug.LogWarning($"No recipes found for level {levelIndex}, using default.");
            currentLevelRecipes = new List<RecipeSO>();
        }
    }

    private void Update() {
        if (!IsServer) {
            return;
        }
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f) {
            spawnRecipeTimer = spawnRecipeTimerMax;
            if (CookingGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax) {
                int waitingRecipeSOIndex = Random.Range(0, currentLevelRecipes.Count);
                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);
            }
        }
    }

    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSoIndex) {
        RecipeSO waitingRecipeSo = currentLevelRecipes[waitingRecipeSoIndex];
        waitingRecipeSOList.Add(waitingRecipeSo);
        OnRecipeSpawn?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
        for (int i = 0; i < waitingRecipeSOList.Count; i++) {
            RecipeSO waitRecipeSo = waitingRecipeSOList[i];

            if (waitRecipeSo.kitchenObjectsSoList.Count == plateKitchenObject.GetKitchenObjectsSoList().Count) {
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectsSO kitchenObjectsSoRecipe in waitRecipeSo.kitchenObjectsSoList) {
                    bool ingredientFound = false;
                    foreach (KitchenObjectsSO kitchenObjectsSoPlate in plateKitchenObject.GetKitchenObjectsSoList()) {
                        if (kitchenObjectsSoPlate == kitchenObjectsSoRecipe) {
                            ingredientFound = true;
                            break;
                        }
                    }

                    if (!ingredientFound) {
                        plateContentsMatchesRecipe = false;
                    }
                }

                if (plateContentsMatchesRecipe) {
                    DeliverCorrectRecipeServerRpc(i);
                    return;
                }
            }
        }
        DeliverIncorrectRecipeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int index) {
        DeliverCorrectRecipeClientRpc(index);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int index) {
        if (waitingRecipeSOList[index].recipeName.Contains("Mega")) {
            waitingRecipeSOList.RemoveAt(index);
            OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
            OnMegaSucess?.Invoke(this, EventArgs.Empty);
            recipesSucceeded++;
        } else {
            waitingRecipeSOList.RemoveAt(index);
            OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
            OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
            recipesSucceeded++;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc() {
        DeliverIncorrectRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc() {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList() {
        return waitingRecipeSOList;
    }

    public int GetRecipesSucceeded() {
        return recipesSucceeded;
    }
}
