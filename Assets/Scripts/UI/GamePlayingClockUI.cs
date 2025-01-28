using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class GamePlayingClockUI : MonoBehaviour {
    [SerializeField] private Image image;

    private void Start() {
        CookingGameManager.Instance.OnGameStateChange += CookingGameManager_OnGameStateChange;
        Hide();
    }

    private void Update() {
        image.fillAmount = CookingGameManager.Instance.GetPlayingTimerNormalized();
    }

    private void CookingGameManager_OnGameStateChange(object sender, System.EventArgs e) {
        if (CookingGameManager.Instance.IsGamePlaying()) {
            Show();
        } else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
