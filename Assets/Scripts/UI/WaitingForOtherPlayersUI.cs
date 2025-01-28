using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForOtherPlayersUI : MonoBehaviour {

    private void Start() {
        CookingGameManager.Instance.OnPlayerReady += Instance_OnPlayerReady;
        CookingGameManager.Instance.OnGameStateChange += Instance_OnGameStateChange;

        Hide();
    }

    private void Instance_OnGameStateChange(object sender, System.EventArgs e) {
        if (CookingGameManager.Instance.isCountdownToStartActive()) {
            Hide();
        }
    }

    private void Instance_OnPlayerReady(object sender, System.EventArgs e) {
        if (CookingGameManager.Instance.IsPlayerReady()) {
            Show();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
