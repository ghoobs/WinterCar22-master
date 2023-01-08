using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class RaceController : MonoBehaviourPunCallbacks
{
    //game logic variables
    public static bool RacePending;
    public static int totalLaps = 1;

    public int timer = 3;
    CheckpointController[] controllers;

    public Text StartText;
    public GameObject endGamePanel;

    AudioSource audioSource;
    public AudioClip countClip;
    public AudioClip startClip;

    public GameObject carPrefab;
    public Transform[] spawnPositions;
    public int playerCount = 2;

    //network logic variables
    public GameObject startRaceButton;
    public GameObject waitingForStartText;
    

    [PunRPC]
    public void StartRace()
    {
        startRaceButton.SetActive(false);
        waitingForStartText.SetActive(false);

        GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
        controllers = new CheckpointController[cars.Length];
        for (int i = 0; i < cars.Length; i++)
        {
            controllers[i] = cars[i].GetComponent<CheckpointController>();
        }
        InvokeRepeating(nameof(Countdown), 3, 1);
    }

    public void StartRaceButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(StartRace), RpcTarget.All, null);
        }
    }

    void Start()
    {
        endGamePanel.SetActive(false);
        audioSource = GetComponent<AudioSource>();

        //spawnowanie graczy
        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        startRaceButton.SetActive(false);
        waitingForStartText.SetActive(false);

        Vector3 spawnPos;
        Quaternion spawnRot;
        GameObject playerCar = null;

        if (PhotonNetwork.IsConnected)
        {
            int playerNumber = playerCount - 1;

            spawnPos = spawnPositions[playerNumber].position;
            spawnRot = spawnPositions[playerNumber].rotation;

            object[] instanceData = new object[4];
            instanceData[0] = PlayerPrefs.GetString("PlayerName");
            instanceData[1] = PlayerPrefs.GetInt("Red");
            instanceData[2] = PlayerPrefs.GetInt("Green");
            instanceData[3] = PlayerPrefs.GetInt("Blue");

            if(OnlinePlayer.LocalPlayerInstance == null)
            {
                playerCar = PhotonNetwork.Instantiate(carPrefab.name, spawnPos, spawnRot, 0, instanceData);
                playerCar.GetComponent<CarAppearance>().SetLocalPlayer(playerNumber);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                startRaceButton.SetActive(true);
            }
            else
            {
                waitingForStartText.SetActive(true);
            }
        }

        playerCar.GetComponent<PlayerController>().enabled = true;       
    }

    void LateUpdate()
    {
        if (!RacePending) return;

        int finishers = 0;
        foreach(CheckpointController c in controllers)
        {
            if (c.lap == totalLaps + 1) finishers++;
        }

        if(finishers == controllers.Length && RacePending)
        {
            endGamePanel.SetActive(true);
            RacePending = false;
        }
    }

    public void RestartRace()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        int number = OnlinePlayer.LocalPlayerInstance.GetComponent<CarAppearance>().playerNumber;
        OnlinePlayer.LocalPlayerInstance.GetComponent<DrivingScript>().rb.position = spawnPositions[number].position;
        OnlinePlayer.LocalPlayerInstance.GetComponent<DrivingScript>().rb.rotation = spawnPositions[number].rotation;
    }

    void Countdown()
    {
        if (timer > 0)
        {
            StartText.text = ("Start za: " + timer);
            audioSource.PlayOneShot(countClip);
            timer--;
        }
        else
        {
            audioSource.PlayOneShot(startClip);
            StartText.text = ("Start!");
            RacePending = true;
            CancelInvoke(nameof(Countdown));

            Invoke(nameof(HideStartText), 1);
        }
    }

    void HideStartText()
    {
        StartText.gameObject.SetActive(false);
    }
}
