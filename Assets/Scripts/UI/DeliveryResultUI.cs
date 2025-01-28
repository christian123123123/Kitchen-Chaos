using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour {

    private const string POPUP = "Popup";

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Color sucessColor;
    [SerializeField] private Color failColor;
    [SerializeField] private Sprite sucessSprite;
    [SerializeField] private Sprite failSprite;

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        DeliveryManager.Instance.OnMegaSucess += Instance_OnMegaSucess;

        gameObject.SetActive(false);
    }

    private void Instance_OnMegaSucess(object sender, System.EventArgs e) {
        gameObject.SetActive(true);
        backgroundImage.color = sucessColor;
        iconImage.sprite = sucessSprite;
        messageText.text = "Delivery\nSuceeded";
        animator.SetTrigger(POPUP);
    }

    private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e) {
        gameObject.SetActive(true);
        backgroundImage.color = failColor;
        iconImage.sprite = failSprite;
        messageText.text = "Delivery\nFailed";
        animator.SetTrigger(POPUP);
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e) {
        gameObject.SetActive(true);
        backgroundImage.color = sucessColor;
        iconImage.sprite = sucessSprite;
        messageText.text = "Delivery\nSuceeded";
        animator.SetTrigger(POPUP);
    }
}
