using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI recipesDeliveredText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button nextLevelButton;
    int currentLevelIndex;
    int nextLevelIndex;


    private void Start() {
        CookingGameManager.Instance.OnGameStateChange += CookingGameManager_OnGameStateChange;
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        nextLevelIndex = currentLevelIndex + 1;
        Hide();
    }

    private void Awake() {
        nextLevelButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();

            if (currentLevelIndex == 3) {
                Loader.Load(Loader.Scene.MainMenuScene);
                return;
            }
            if (currentLevelIndex == 1) {
                Loader.Load(Loader.Scene.GameScene2);
            } else if (currentLevelIndex == 2) {
                Loader.Load(Loader.Scene.GameScene3);
            }
        });
    }

    private void CookingGameManager_OnGameStateChange(object sender, System.EventArgs e) {
        if (CookingGameManager.Instance.IsGameOver()) {
            Show();
            int scoreInt = ScoreUI.Instance.GetScore();
            recipesDeliveredText.text = DeliveryManager.Instance.GetRecipesSucceeded().ToString();
            scoreText.text = scoreInt.ToString();
            if (scoreInt < 200) {
                scoreText.color = Color.red;
            } else if (scoreInt >= 200 && scoreInt < 350) {
                scoreText.color = Color.yellow;
            } else if (scoreInt > 350) {
                scoreText.color = Color.green;
            }
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
