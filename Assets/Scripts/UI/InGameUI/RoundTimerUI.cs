using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundTimerUI : MonoBehaviour
{
    [SerializeField] TMP_Text timeLeftText, gameStateText;
    [SerializeField] LangText intermission, roundInProgress, roundFinished;
    private void Start()
    {
        GameManager.Instance.timeLeft.OnValueChanged += OnTimeLeftChange;
        GameManager.Instance.gameState.OnValueChanged += OnGameStateChange;
        OnTimeLeftChange(0, GameManager.Instance.timeLeft.Value);
        OnGameStateChange(GameState.Intermission, GameManager.Instance.gameState.Value);
    }
    void OnGameStateChange(GameState prev, GameState val)
    {
        string text = string.Empty;
        switch (val)
        {
            case GameState.Intermission: text = intermission.text; break;
            case GameState.RoundInProgress: text = roundInProgress.text; break;
            case GameState.RoundFinished: text = roundFinished.text; break;
        }
        gameStateText.text = text;
    }
    void OnTimeLeftChange(int prev, int timeLeft)
    {
        timeLeftText.text = $"{timeLeft / 60}:{(timeLeft % 60 < 10 ? '0' : null)}{timeLeft % 60}";
    }
}