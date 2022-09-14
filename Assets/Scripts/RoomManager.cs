using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    private void Awake()
    {
        if (Instance) // create singleton
        {
           
            Destroy(Instance);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance); // one room manager per server
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) // spawn player
    {
    
        Vector3 spawnPosition = new Vector3(841, 2, 30);

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.Instantiate("FPSController", spawnPosition, Quaternion.identity); // public
        }
        else
        {
            Instantiate(Resources.Load("FPSController"), spawnPosition, Quaternion.identity); //solo
        }

        
    }

    ////////// scene managers


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
