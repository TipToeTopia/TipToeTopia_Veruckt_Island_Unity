using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class ShopManager : MonoBehaviour
{
    public int price = 50;


    PlayerManager playerManager;
    bool playerIsInReach = false;

    public bool HealthStation;
    public bool ammoStation;
    public bool Door;
    public bool DoshShare;
    public PhotonView photonView;

    // Update is called once per frame
    void Update()
    {
        if (playerIsInReach)
        {
            if (DoshShare)
            {
               
                if (PhotonNetwork.InRoom) // giving money
                {
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        playerManager.currentPoints += 50;
                        photonView.RPC("DestroyCashSync", RpcTarget.All, photonView.ViewID); // rpc callback for all players
                    }
                    
                }
            }
                

            if (Input.GetKeyDown(KeyCode.E)) // if object is health or ammo stations we call respective functions
            {
                if (ammoStation || HealthStation)
                {
                    BuyShop();
                }
                else if (Door)
                {
                    OpenDoor();
                }
            }
        }
    }

    void OnTriggerEnter(Collider player) // if player is in reach
    {
        if (player.CompareTag("Player"))
        {
            playerIsInReach = true;
            playerManager = player.gameObject.GetComponent<PlayerManager>();
        }
    }


    void OnTriggerExit(Collider player) // player not in reach anymore
    {
        if (player.CompareTag("Player"))
        {
        
            playerIsInReach = false;
        }
    }

    public void BuyShop() // private function callbacks
    {
        if (playerManager.currentPoints >= price) // if they have enough money
        {
            playerManager.currentPoints -= price; // deduct private money

            if (HealthStation)
            {
                playerManager.health = playerManager.healthCap;
                playerManager.healthNumber.text = playerManager.health.ToString();
            }

            if (ammoStation)
            {
                foreach (Transform child in playerManager.weaponHolder.transform)
                {
                    if (child.gameObject.activeSelf)
                    {
                        WeaponManager weaponManager = child.gameObject.GetComponent<WeaponManager>();
                        weaponManager.reserveAmmo = weaponManager.ammoCap;
                        StartCoroutine(weaponManager.Reload(weaponManager.reloadTime));
                        weaponManager.reserveAmmoText.text = weaponManager.reserveAmmo.ToString();
                    }
                }
            }
        }
       
    }

    public void OpenDoor() // public function for all players across network
    {
        if (playerManager.currentPoints >= price)
        {
            playerManager.currentPoints -= price;

            if (Door)
            {
                // destroy door for all in session

                photonView.RPC("DestroyDoorSync", RpcTarget.All, photonView.ViewID);

                if (PhotonNetwork.InRoom)
                {
                    photonView.RPC("DestroyDoorSync", RpcTarget.All, photonView.ViewID);
                    
                }
                else
                {
                    DestroyDoorSync(photonView.ViewID);
                }
            }
        }
    }

    [PunRPC]

    public void DestroyDoorSync(int viewID)
    {
        if (photonView.ViewID == viewID)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]

    public void DestroyCashSync(int viewID)
    {
        if (photonView.ViewID == viewID)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }


}

