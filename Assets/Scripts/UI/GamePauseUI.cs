using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour {

    public static GamePauseUI Instance { get; private set; }

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button optionsButton;



    private void Start() {
        CookingGameManager.Instance.OnGamePause += Instance_OnGamePause;
        CookingGameManager.Instance.OnGameUnpause += Instance_OnGameUnpause;
        Hide();
    }

    private void Awake() {
        Instance = this;
        resumeButton.onClick.AddListener(() => {
            CookingGameManager.Instance.PauseGame();
        });

        mainMenuButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        optionsButton.onClick.AddListener(() => {
            OptionsUI.Instance.Show();
            Hide();
        });
    }


    private void Instance_OnGameUnpause(object sender, System.EventArgs e) {
        Hide();
    }

    private void Instance_OnGamePause(object sender, System.EventArgs e) {
        Show();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
