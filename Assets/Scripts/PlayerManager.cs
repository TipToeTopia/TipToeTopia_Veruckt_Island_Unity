using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public float health = 100;
    public Text healthNumber;
    public GameManager gameManager;

    public CanvasGroup hurtPanel;

    public GameObject weaponHolder;
    public GameObject playerCamera;
    public GameObject lookCamera;


    int activeWeaponIndex;
    GameObject activeWeapon;

    public float currentPoints;
    public Text pointsNumber;
    public float healthCap;
    public PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        healthNumber.text = health.ToString();

        weaponSwitch(0);

        healthCap = health;
        
    }

    private void Update()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            playerCamera.SetActive(false);
            return;

        }
        if (hurtPanel.alpha > 0)
        {
            hurtPanel.alpha -= Time.deltaTime;
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            weaponSwitch(activeWeaponIndex + 1);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (currentPoints <= 0)
            {
                return;
            }
            else
            {
                currentPoints = currentPoints - 50;
             
                GameObject enemySpawned;
                if (PhotonNetwork.InRoom)
                {
                    enemySpawned = PhotonNetwork.Instantiate("Cash", activeWeapon.transform.position + new Vector3(0, 0, 5), Quaternion.identity);
                }
                else
                {
                    enemySpawned = Instantiate(Resources.Load("Cash"), activeWeapon.transform.position, Quaternion.identity) as GameObject;
                }
            }
        }

        pointsNumber.text = "Dosh: " + currentPoints.ToString() + " $";
    }

    public void Hit(float damage)
    {
       if (PhotonNetwork.InRoom)
       {

            if (health <= 0)
            {
                return;
            }
            else
            {
                photonView.RPC("PlayerTakeDamage", RpcTarget.All, damage, photonView.ViewID);
            }

            
        }
     
    }

    [PunRPC]

    public void PlayerTakeDamage(float damage, int viewID)
    {
        if (photonView.ViewID == viewID)
        {
            health -= damage;
            healthNumber.text = health.ToString();

            if (health <= 0)
            {
             
                PhotonNetwork.LeaveRoom();
                LoadMainMenuScene();
              
                
            }
            else
            {

                hurtPanel.alpha = 1;
            }
        }
    }

    public void weaponSwitch(int weaponIndex)
    {
        int index = 0;
        int amountOfWeapons = weaponHolder.transform.childCount;

        if (weaponIndex > amountOfWeapons - 1)
        {
            weaponIndex = 0;
        }
        foreach (Transform child in weaponHolder.transform)
        {
            if (child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(false);
            }
            if (index == weaponIndex)
            {
                child.gameObject.SetActive(true);
                activeWeapon = child.gameObject;
            }
            index++;
        }
        activeWeaponIndex = weaponIndex;
        if (photonView.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("weaponIndex", weaponIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!photonView.IsMine && targetPlayer == photonView.Owner && changedProps["weaponIndex"] != null)
        {
            weaponSwitch((int)changedProps["weaponIndex"]);
        }
    }

    [PunRPC]

    public void WeaponShootVFX(int viewID)
    {
        activeWeapon.GetComponent<WeaponManager>().ShootVFX(viewID);
    }

    public void EndGame()
    {
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 0;

        }
        Cursor.lockState = CursorLockMode.None;
      
        Invoke("LoadMainMenuScene", 0.4f);

    }

    public void LoadMainMenuScene()
    {
        AudioListener.volume = 1;
        PhotonNetwork.LoadLevel(0);
    }


}
