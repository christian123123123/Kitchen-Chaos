using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HostDisconnectUI : MonoBehaviour {



    private void Start() {
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;

        Hide();
    }

    private void Singleton_OnClientConnectedCallback(ulong clientId) {
        if (clientId == NetworkManager.ServerClientId) {
            //Server quit
            Show();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
