using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;

public class PhotonPlayer : MonoBehaviour
{

    private PhotonView PlayerView;
    public GameObject myAvatar;

    // Make sure the game Object has a Photon TransformView Component
    // The Gameobjecty must also have a PhotonView Component

    // Start is called before the first frame update
    void Start()
    {
        // The PhotonView allows us to sendRPC calls for the object abd gives a network ID
        PlayerView = GetComponent<PhotonView>();

        // Here we are picking a spawn location for the player
        int spawnPicker = Random.Range(0, GameSetup.GS.spawnPoints.Length);


        // Here we are checking that if local
        if (PlayerView.IsMine)
        {
            // This is where the player is being spawned on the Network on the spawn location
            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"), GameSetup.GS.spawnPoints[spawnPicker].position, GameSetup.GS.spawnPoints[spawnPicker].rotation, 0);
        }
    }
}