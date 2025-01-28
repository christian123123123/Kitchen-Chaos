using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour {

    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button wButton;
    [SerializeField] private Button aButton;
    [SerializeField] private Button sButton;
    [SerializeField] private Button dButton;
    [SerializeField] private Button escButton;
    [SerializeField] private Button fButton;
    [SerializeField] private Button eButton;


    [SerializeField] private TextMeshProUGUI soundEffectsText;
    [SerializeField] private TextMeshProUGUI musicText;

    [SerializeField] private TextMeshProUGUI wButtonText;
    [SerializeField] private TextMeshProUGUI aButtonText;
    [SerializeField] private TextMeshProUGUI sButtonText;
    [SerializeField] private TextMeshProUGUI dButtonText;
    [SerializeField] private TextMeshProUGUI escButtonText;
    [SerializeField] private TextMeshProUGUI fButtonText;
    [SerializeField] private TextMeshProUGUI eButtonText;
    [SerializeField] private Transform pressToRebindTransform;



    private void Awake() {
        Instance = this;
        soundEffectsButton.onClick.AddListener(() => {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        musicButton.onClick.AddListener(() => {
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        backButton.onClick.AddListener(() => {
            GamePauseUI.Instance.Show();
            Hide();
        });

        wButton.onClick.AddListener(() => { Rebind(GameInput.Binding.Move_Up); });
        sButton.onClick.AddListener(() => { Rebind(GameInput.Binding.Move_Down); });
        aButton.onClick.AddListener(() => { Rebind(GameInput.Binding.Move_Left); });
        dButton.onClick.AddListener(() => { Rebind(GameInput.Binding.Move_Right); });
        eButton.onClick.AddListener(() => { Rebind(GameInput.Binding.Interact); });
        fButton.onClick.AddListener(() => { Rebind(GameInput.Binding.Interact_Alternate); });
        escButton.onClick.AddListener(() => { Rebind(GameInput.Binding.Pause); });

    }

    private void Start() {
        CookingGameManager.Instance.OnGameUnpause += Instance_OnGameUnpause; ;

        UpdateVisual();
        HidePressToRebind();
        Hide();
    }

    private void Instance_OnGameUnpause(object sender, EventArgs e) {
        Hide();
    }

    private void UpdateVisual() {
        soundEffectsText.text = "Sound effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);
        wButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        aButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
        sButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
        dButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
        eButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        fButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact_Alternate);
        escButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    public void ShowPressToRebind() {
        pressToRebindTransform.gameObject.SetActive(true);
    }

    public void HidePressToRebind() {
        pressToRebindTransform.gameObject.SetActive(false);
    }

    private void Rebind(GameInput.Binding binding) {
        ShowPressToRebind();
        GameInput.Instance.ReBind(binding, () => {
            HidePressToRebind();
            UpdateVisual();
        });
    }
}
