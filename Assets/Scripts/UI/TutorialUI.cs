using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI moveUp;
    [SerializeField] private TextMeshProUGUI moveDown;
    [SerializeField] private TextMeshProUGUI moveLeft;
    [SerializeField] private TextMeshProUGUI moveRight;
    [SerializeField] private TextMeshProUGUI interact;
    [SerializeField] private TextMeshProUGUI interactAlternate;
    [SerializeField] private TextMeshProUGUI pause;

    private void Start() {
        GameInput.Instance.OnKeyRebind += Instance_OnKeyRebind;
        CookingGameManager.Instance.OnPlayerReady += Instance_OnPlayerReady;
        UpdateVisual();

        Show();
    }

    private void Instance_OnPlayerReady(object sender, System.EventArgs e) {
        if (CookingGameManager.Instance.IsPlayerReady()) {
            Hide();
        }
    }

    private void Instance_OnGameStateChange(object sender, System.EventArgs e) {
        if (CookingGameManager.Instance.isCountdownToStartActive()) {
            Hide();
        }
    }

    private void Instance_OnKeyRebind(object sender, System.EventArgs e) {
        UpdateVisual();
    }

    private void UpdateVisual() {
        moveUp.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        moveDown.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
        moveLeft.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
        moveRight.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
        interact.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        interactAlternate.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact_Alternate);
        pause.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
