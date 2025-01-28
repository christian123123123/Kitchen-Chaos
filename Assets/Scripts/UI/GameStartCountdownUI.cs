using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour {

    private const string NUMBER_POPUP = "NumberPopup";

    [SerializeField] private TextMeshProUGUI countdownText;

    private Animator animator;
    private int previousCountdownNumber;
    private void Awake() {
        animator = GetComponent<Animator>();
    }
    private void Start() {
        CookingGameManager.Instance.OnGameStateChange += CookingGameManager_OnGameStateChange;
        Hide();
    }

    private void Update() {
        int countdown = Mathf.CeilToInt(CookingGameManager.Instance.GetCountdownToStartTimer());
        countdownText.text = countdown.ToString();

        if (previousCountdownNumber != countdown) {
            previousCountdownNumber = countdown;
            animator.SetTrigger(NUMBER_POPUP);
            SoundManager.Instance.PlayCountDownSounds();
        }

    }

    private void CookingGameManager_OnGameStateChange(object sender, System.EventArgs e) {
        if (CookingGameManager.Instance.isCountdownToStartActive()) {
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
