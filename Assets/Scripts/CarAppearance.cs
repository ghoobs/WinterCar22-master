using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarAppearance : MonoBehaviour
{
    public int playerNumber;

    public string playerName;
    public Color carColor;
    public Text nameText;
    public Renderer carRenderer;

    int CarId;
    bool idSet;
    public CheckpointController controller;

    public void SetAvatar(string name, Color color)
    {
        playerName = name;
        carColor = color;

        nameText.text = playerName;
        carRenderer.material.color = carColor;
        nameText.color = carColor;
    }

    public void SetLocalPlayer(int playerNumber)
    {
        this.playerNumber = playerNumber;

        FindObjectOfType<CameraController>().SetCamera(this.transform);
        playerName = PlayerPrefs.GetString("PlayerName");

        Color playerColor = MenuController.IntToColor(PlayerPrefs.GetInt("Red"), PlayerPrefs.GetInt("Green"), PlayerPrefs.GetInt("Blue"));
        carColor = playerColor;

        nameText.text = playerName;
        carRenderer.material.color = carColor;
        nameText.color = carColor;
    }

    private void LateUpdate()
    {
        if (!idSet)
        {
            CarId = Leaderboard.RegisterCar(playerName);
            idSet = true;
        }

        Leaderboard.SetPosition(CarId, controller.lap, controller.checkpoint);
    }
}
