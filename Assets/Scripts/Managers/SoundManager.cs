using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";

    public static SoundManager Instance { get; private set; }
    [SerializeField] private SoundClipsSO soundClipsSo;

    private float volume = 1f;

    private void Awake() {
        Instance = this;
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
    }

    private void Start() {
        DeliveryManager.Instance.OnRecipeSuccess += Instance_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += Instance_OnRecipeFailed;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.OnPlayerPickup += Instance_OnPickup;
        BaseCounter.OnDrop += BaseCounter_OnDrop;
        TrashCounter.OnObjectTrash += TrashCounter_OnObjectTrash;
    }

    private void TrashCounter_OnObjectTrash(object sender, System.EventArgs e) {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(soundClipsSo.trash, trashCounter.transform.position);
    }

    private void BaseCounter_OnDrop(object sender, System.EventArgs e) {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(soundClipsSo.objectDrop, baseCounter.transform.position);
    }

    private void Instance_OnPickup(object sender, System.EventArgs e) {
        Player player = sender as Player;
        PlaySound(soundClipsSo.pickup, player.transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e) {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(soundClipsSo.chop, cuttingCounter.transform.position, 2f);
    }

    private void Instance_OnRecipeFailed(object sender, System.EventArgs e) {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(soundClipsSo.deliveryFail, deliveryCounter.transform.position);
    }

    private void Instance_OnRecipeSuccess(object sender, System.EventArgs e) {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(soundClipsSo.deliverySuccess, deliveryCounter.transform.position);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f) {
        AudioSource.PlayClipAtPoint(audioClip, position, volume * volumeMultiplier);
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f) {
        PlaySound(audioClipArray[UnityEngine.Random.Range(0, audioClipArray.Length)], position, volume);
    }

    public void PlayFootStepSounds(Vector3 position, float volume = 1f) {
        PlaySound(soundClipsSo.footSteps, position, volume);
    }

    public void PlayCountDownSounds() {
        PlaySound(soundClipsSo.warning, Vector3.zero);
    }

    public void PlayWarningSound(Vector3 position) {
        PlaySound(soundClipsSo.warning, position);
    }

    public void ChangeVolume() {
        volume += .1f;
        if (volume > 1f) {
            volume = 0f;
        }

        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume() {
        return volume;
    }
}
