using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class RaceLauncher : MonoBehaviourPunCallbacks
{
    public InputField inputName;

    byte maxPlayersPerRoom = 4;
    bool isConnecting;
    public Text networkText;
    string gameVersion = "2.0";

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            inputName.text = PlayerPrefs.GetString("PlayerName");
        }
    }

    public void SetName(string name)
    {
        PlayerPrefs.SetString("PlayerName", name);
    }

    public void Connect()
    {
        networkText.text = "";
        isConnecting = true;
        PhotonNetwork.NickName = inputName.text;
        if (PhotonNetwork.IsConnected)
        {
            networkText.text += "Joining room... \n";
            Debug.Log("Joining room... \n");
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            networkText.text += "Connecting to a server... \n";
            Debug.Log("Connecting to a server... \n");
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            networkText.text += "Connected to a server.\n";
            Debug.Log("Connected to a server.\n");
            PhotonNetwork.JoinRandomRoom();
        }
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        networkText.text = "Failed to join a room... \n";
        networkText.text = "Cause: " + message + "\n";
        networkText.text = "Code: " + returnCode + "\n";

        Debug.Log("Cause: " + message + "\n");
        Debug.Log("Code: " + returnCode + "\n");

        PhotonNetwork.CreateRoom(null, new RoomOptions() {MaxPlayers = maxPlayersPerRoom});
    }
    public override void OnJoinedRoom()
    {
        networkText.text = "Joined room with " + PhotonNetwork.CurrentRoom.PlayerCount + " players.\n";
        PhotonNetwork.LoadLevel("SampleScene");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        networkText.text = "Disconnected cuz of: " + cause + "\n";
        isConnecting = false;
    }
    #endregion
}
