using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Photon.Pun;

public class EnemyManager : MonoBehaviour
{
    public GameObject player;
    public Animator enemyAnimator;
    public float damage = 20f;
    public float health = 100;
    public GameManager gameManager;
    public bool playerInReach;
    private float attackDelayTimer;
    public float howMuchEarlierStartAttackAnim;
    public float delayBetweenAttacks;
    public AudioClip[] zombieSounds;
    public AudioSource audioSource;
    public int points = 20;
    GameObject[] playerInScene;
    public PhotonView photonView;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerInScene = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
           
            audioSource.clip = zombieSounds[Random.Range(0, zombieSounds.Length)];
            audioSource.Play();
        }

        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
        {
            return;
        }

        GetClosestPlayer();
        if (player != null)
        {
            GetComponent<NavMeshAgent>().destination = player.transform.position;
        }

        if (GetComponent<NavMeshAgent>().velocity.magnitude > 1)
        {
            enemyAnimator.SetBool("isRunning", true);
        }
        else
        {
            enemyAnimator.SetBool("isRunning", false);
        }
    }

    private void GetClosestPlayer()
    {
        float minDistance = Mathf.Infinity;
        Vector3 currPosition = transform.position;
        foreach (GameObject thisPlayer in playerInScene)
        {
            if (thisPlayer != null)
            {
                float distance = Vector3.Distance(thisPlayer.transform.position, currPosition);
                if (distance < minDistance)
                {
                    player = thisPlayer;
                    minDistance = distance;
                }
            }

        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player)
        {
            playerInReach = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (playerInReach)
        {
            attackDelayTimer += Time.deltaTime;
        }

        if (attackDelayTimer >= delayBetweenAttacks - howMuchEarlierStartAttackAnim && attackDelayTimer <= delayBetweenAttacks && playerInReach)
        {
            enemyAnimator.SetTrigger("isAttacking");
        }

        if (attackDelayTimer >= delayBetweenAttacks && playerInReach)
        {
            player.GetComponent<PlayerManager>().Hit(damage);
            attackDelayTimer = 0;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == player)
        {
            playerInReach = false;
            attackDelayTimer = 0;
        }
    }

    public void Hit(float damage)
    {
        photonView.RPC("TakeDamage", RpcTarget.All, damage, photonView.ViewID);
    }

    [PunRPC]

    public void TakeDamage(float damage, int viewID)
    {
        if (photonView.ViewID == viewID)
        {
            health -= damage;
            if (health <= 0)
            {
                enemyAnimator.SetTrigger("isDead");
                Destroy(gameObject, 10f);
                if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
                {
                    gameManager.enemiesAlive--;
                }
                Destroy(GetComponent<NavMeshAgent>());
                Destroy(GetComponent<EnemyManager>());
                Destroy(GetComponent<CapsuleCollider>());
            }
        }
    }
}
