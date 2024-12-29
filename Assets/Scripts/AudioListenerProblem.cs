using UnityEngine;
using Unity.Netcode; // Use PhotonNetwork if using Photon Networking

public class PlayerSetup : NetworkBehaviour // or MonoBehaviour for other networking libraries
{
    public AudioListener audioListener;

    void Start()
    {
        if (IsLocalPlayer) // or `photonView.IsMine` for Photon
        {
            audioListener.enabled = true;
        }
        else
        {
            audioListener.enabled = false;
        }
    }
}
