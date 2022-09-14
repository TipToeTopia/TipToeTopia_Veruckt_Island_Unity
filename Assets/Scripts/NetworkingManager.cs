using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;


public class NetworkingManager : MonoBehaviourPunCallbacks
{
    public GameObject connecting;
    public GameObject multiPlayer;

    void Start() // joining photon server
    {

        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnConnectedToMaster() // joining lobby
    {

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() // connected to network - ready to play
    {

        connecting.SetActive(false);
        multiPlayer.SetActive(true);
    }

    public void FindMatch()
    {

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message) // if no room available we make room
    {
        MakeRoom();
    }

    void MakeRoom() // set parameters for room
    {
        int randomRoomName = Random.Range(0, 5000);

        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true, IsOpen = true, MaxPlayers = 4, PublishUserId = true
            
            
        };

        PhotonNetwork.CreateRoom("RoomName_" + randomRoomName, roomOptions);
    }

    public override void OnJoinedRoom() // switch scene
    {
        PhotonNetwork.LoadLevel(2);
    }
}
