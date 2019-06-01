using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngameView : MonoBehaviour
{
    [Header("Ingame Canvas")]
    public GameObject ingameCanvas;
    public TextMeshProUGUI playerScore;
    public TextMeshProUGUI botScore;
    public TextMeshProUGUI notification;

    [Header("Game over canvas")]
    public GameObject gameoverCanvas;
    public Text goNoti;
    public Button goPlayAgain;
    public Button goQuit;

    public void UpdateScore(int _playerScore, int _botScore)
    {
        playerScore.text = _playerScore.ToString();
        botScore.text = _botScore.ToString();
    }

    public void UpdateNotification(string noti)
    {
        notification.text = noti;
    }

    public void UpdateGameOverNoti(string noti)
    {
        goNoti.text = noti;
    }

    public void OpenIngameUI()
    {
        ingameCanvas.SetActive(true);
        gameoverCanvas.SetActive(false);
    }

    public void OpenGameOverUI()
    {
        ingameCanvas.SetActive(false);
        gameoverCanvas.SetActive(true);
    }
}
