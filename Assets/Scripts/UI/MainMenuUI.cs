using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button chooseLevelButton;

    private void Awake() {
        playButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.GameScene);
        });
        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
        chooseLevelButton.onClick.AddListener(() => {
            ChooseLevelUI.Instance.Show();
        });

        Time.timeScale = 1f;
    }


}
