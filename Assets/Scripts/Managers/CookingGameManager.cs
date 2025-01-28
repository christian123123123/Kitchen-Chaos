using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CookingGameManager : NetworkBehaviour {

    public static CookingGameManager Instance { get; private set; }

    public event EventHandler OnGameStateChange;
    public event EventHandler OnGamePause;
    public event EventHandler OnGameUnpause;
    public event EventHandler OnPlayerReady;
    public event EventHandler OnMultiplayerGamePause;
    public event EventHandler OnMultiplayerGameUnpause;

    private enum State {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private bool isReady;
    private NetworkVariable<float> countDownStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    private float gamePlayingTimerMax = 90f;
    private bool isLocalGamePaused;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
    private Dictionary<ulong, bool> playersReadyDictionary;
    private Dictionary<ulong, bool> playerPauseDictionary;
    private bool autoTestGamePauseState;


    private void Awake() {
        Instance = this;
        playersReadyDictionary = new Dictionary<ulong, bool>();
        playerPauseDictionary = new Dictionary<ulong, bool>();
    }

    private void Start() {
        GameInput.Instance.OnPauseAction += Instance_OnPauseAction;
        GameInput.Instance.OnInteractAction += Instance_OnInteractAction;
    }

    public override void OnNetworkSpawn() {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += GamePause_OnValueChanged;
        if (IsServer) {
            NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;
        }
    }

    private void Singleton_OnClientDisconnectCallback(ulong clientId) {
        autoTestGamePauseState = true;
    }

    private void State_OnValueChanged(State previousValue, State newValue) {
        OnGameStateChange?.Invoke(this, EventArgs.Empty);
    }

    private void GamePause_OnValueChanged(bool previousValue, bool newValue) {
        if (isGamePaused.Value) {
            Time.timeScale = 0f;

            OnMultiplayerGamePause?.Invoke(this, EventArgs.Empty);
        } else {
            Time.timeScale = 1f;

            OnMultiplayerGameUnpause?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Instance_OnInteractAction(object sender, EventArgs e) {
        if (state.Value == State.WaitingToStart) {
            isReady = true;
            OnPlayerReady?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        playersReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!playersReadyDictionary.ContainsKey(clientId) || !playersReadyDictionary[clientId]) {
                // This player is not ready
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady) {
            state.Value = State.CountdownToStart;
        }
    }

    private void Instance_OnPauseAction(object sender, EventArgs e) {
        PauseGame();
    }

    private void Update() {
        if (!IsServer) {
            return;
        }
        switch (state.Value) {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countDownStartTimer.Value -= Time.deltaTime;
                if (countDownStartTimer.Value < 0) {
                    state.Value = State.GamePlaying;
                    gamePlayingTimer.Value = gamePlayingTimerMax;
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value < 0) {
                    state.Value = State.GameOver;
                }
                break;
            case State.GameOver:
                break;
        }
    }

    private void LateUpdate() {
        if (autoTestGamePauseState) {
            autoTestGamePauseState = false;
            TestGamePauseState();
        }
    }
    public bool IsGamePlaying() {
        return state.Value == State.GamePlaying;
    }
    public bool IsGameOver() {
        return state.Value == State.GameOver;
    }
    public bool isCountdownToStartActive() {
        return state.Value == State.CountdownToStart;
    }
    public bool IsPlayerReady() {
        return isReady;
    }
    public float GetCountdownToStartTimer() {
        return countDownStartTimer.Value;
    }
    public float GetPlayingTimerNormalized() {
        return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
    }
    public void PauseGame() {
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused) {
            PauseGameServerRpc();
            OnGamePause?.Invoke(this, EventArgs.Empty);
        } else {
            UnPauseGameServerRpc();
            OnGameUnpause?.Invoke(this, EventArgs.Empty);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default) {
        playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = true;

        TestGamePauseState();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnPauseGameServerRpc(ServerRpcParams serverRpcParams = default) {
        playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = false;

        TestGamePauseState();
    }

    private void TestGamePauseState() {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (playerPauseDictionary.ContainsKey(clientId) && playerPauseDictionary[clientId]) {
                isGamePaused.Value = true;
                return;
            }
        }
        isGamePaused.Value = false;
    }
}
