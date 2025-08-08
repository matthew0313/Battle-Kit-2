using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;

public class RoundTimerUI : MonoBehaviour
{
    [SerializeField] TMP_Text timeLeftText, roundInProgressText;
    [SerializeField] LangText roundInProgress, roundNotInProgress;
    private void Start()
    {
        GameManager.Instance.timeLeft.OnValueChanged += OnTimeLeftChange;
        GameManager.Instance.roundInProgress.OnValueChanged += OnRoundInProgressChange;
        OnTimeLeftChange(0, GameManager.Instance.timeLeft.Value);
        OnRoundInProgressChange(false, GameManager.Instance.roundInProgress.Value);
    }
    void OnRoundInProgressChange(bool prev, bool isRoundInProgress)
    {
        roundInProgressText.text = isRoundInProgress ? roundInProgress.text : roundNotInProgress.text;
    }
    void OnTimeLeftChange(int prev, int timeLeft)
    {
        timeLeftText.text = $"{timeLeft / 60}:{(timeLeft % 60 < 10 ? '0' : null)}{timeLeft % 60}";
    }
}