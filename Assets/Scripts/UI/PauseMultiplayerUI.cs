using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMultiplayerUI : MonoBehaviour {


    private void Start() {
        CookingGameManager.Instance.OnMultiplayerGamePause += Instance_OnMultiplayerGamePause;
        CookingGameManager.Instance.OnMultiplayerGameUnpause += Instance_OnMultiplayerGameUnpause;

        Hide();
    }

    private void Instance_OnMultiplayerGameUnpause(object sender, System.EventArgs e) {
        Hide();
    }

    private void Instance_OnMultiplayerGamePause(object sender, System.EventArgs e) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
