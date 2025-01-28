using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour {

    public static ScoreUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;
    private int scoreInt = 0;

    private void Awake() {
        Instance = this;
        scoreText.text = scoreInt.ToString();
        if (DeliveryManager.Instance != null) {
            DeliveryManager.Instance.OnRecipeSuccess += Instance_OnRecipeSuccess;
            DeliveryManager.Instance.OnRecipeFailed += Instance_OnRecipeFailed;
            DeliveryManager.Instance.OnMegaSucess += Instance_OnMegaSucess;
        }

        CookingGameManager.Instance.OnGameStateChange += Instance_OnGameStateChange; ;
    }

    private void Instance_OnGameStateChange(object sender, System.EventArgs e) {
        if (CookingGameManager.Instance.IsGameOver()) {
            Hide();
        }
    }

    private void Update() {
        if (scoreInt < 200) {
            scoreText.color = Color.red;
        } else if (scoreInt >= 200 && scoreInt < 350) {
            scoreText.color = Color.yellow;
        } else if (scoreInt > 350) {
            scoreText.color = Color.green;
        }
    }

    private void Instance_OnMegaSucess(object sender, System.EventArgs e) {
        scoreInt += 150;
        scoreText.text = scoreInt.ToString();
    }

    private void Instance_OnRecipeFailed(object sender, System.EventArgs e) {
        scoreInt -= 50;
        if (scoreInt < 0) scoreInt = 0;
        scoreText.text = scoreInt.ToString();
    }

    private void Instance_OnRecipeSuccess(object sender, System.EventArgs e) {
        scoreInt += 100;
        scoreText.text = scoreInt.ToString();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    public int GetScore() {
        return scoreInt;
    }
}
