using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChooseLevelUI : MonoBehaviour {

    public static ChooseLevelUI Instance { get; private set; }

    [SerializeField] private List<Button> levelButtons;

    [SerializeField] private Button backButton;

    private void Awake() {
        Instance = this;
        Hide();
        backButton.onClick.AddListener(() => {
            Hide();
        });
        levelButtons[0].onClick.AddListener(() => {
            Loader.Load(Loader.Scene.GameScene);
        });
        levelButtons[1].onClick.AddListener(() => {
            Loader.Load(Loader.Scene.GameScene2);
        });
        levelButtons[2].onClick.AddListener(() => {
            Loader.Load(Loader.Scene.GameScene3);
        });
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
