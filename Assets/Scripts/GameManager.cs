using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    public int enemiesAlive = 0;
    public int round = 0;
    public GameObject[] spawnPoints;
    public GameObject enemyPrefab;
    public Text roundNumber;
    public GameObject endScreen;
    public Text roundsSurvived;
    public GameObject pauseMenu;
    public PhotonView photonView;
    private int zombieincreaser = 2;

    void Start()
    {
        
        roundNumber.text = "Wave: " + roundNumber.ToString();
        roundsSurvived.text = "Wave: " + roundsSurvived.ToString();
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawners");
}

    void Update()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            if (enemiesAlive == 0)
            {
                round++;
                if (round == 1)
                {
                    zombieincreaser = zombieincreaser + 1; // add an extra zombie on top of the already increasing number of zombies per round
                }
                else
                {
                    zombieincreaser = zombieincreaser + 8; // add an extra zombie on top of the already increasing number of zombies per round
                }
               
                NextWave(round + zombieincreaser);
               
                if (PhotonNetwork.InRoom)
                {
                    Hashtable hash = new Hashtable();
                    hash.Add("currentRound", round);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                }
                else
                {
                    DisplayNextRound(round);
                }
             
            }
        }
        roundsSurvived.text = "Wave: " + roundsSurvived.ToString();


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseMenu.activeSelf)
            {
                Pause();
            }
            else
            {
                Continue();
            }
        }
    }

    private void DisplayNextRound(int round)
    {
        roundNumber.text = round.ToString();
    }

    public void NextWave(int round) // spawn zombies after end of round
    {
      for (var x = 0; x < zombieincreaser; x++) // increment zombie count
      {
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemySpawned;

            if (PhotonNetwork.InRoom) // instantiate for all
            {
               enemySpawned = PhotonNetwork.Instantiate("Zombie", spawnPoint.transform.position, Quaternion.identity);
            }
            else
            {
               enemySpawned = Instantiate(Resources.Load("Zombie"), spawnPoint.transform.position, Quaternion.identity) as GameObject;
            }
            enemySpawned.GetComponent<EnemyManager>().gameManager = GetComponent<GameManager>();
            enemiesAlive++;
      }
    }

    public void EndGame()
    {
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 0;

        }
        Cursor.lockState = CursorLockMode.None;
        endScreen.SetActive(true);
        roundsSurvived.text = round.ToString();
        Invoke("LoadMainMenuScene", 0.4f);

    }

    public void Restart()
    {
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 1;
        }
        SceneManager.LoadScene(1);
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        AudioListener.volume = 0;
         if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 0;
        }
        pauseMenu.SetActive(true);
    }

    public void Continue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        AudioListener.volume = 1;
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 1;
        }
        pauseMenu.SetActive(false);
    }

    public void BackToMainMenu()
    {
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 1;
        }
        Invoke("LoadMainMenuScene", 0.4f);
    }

    public void LoadMainMenuScene()
    {
        AudioListener.volume = 1;
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {

     

        if (photonView.IsMine)
        {
            if (changedProps["currentRound"] != null)
            {
                DisplayNextRound((int)changedProps["currentRound"]);
            }
        }
        
    }
}
