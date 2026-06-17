using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private float durationSeconds = 90f;
    [SerializeField] private TMP_Text countdownText;
    public bool startOnEnable = false;

    float remainingTime;
    bool isRunning;

    public bool IsRunning => isRunning;
    public float RemainingTime => remainingTime;

    public void StartCountdown()
    {
        remainingTime = durationSeconds;
        isRunning = true;
        UpdateDisplay();
    }

    public void StartCountdown(float seconds)
    {
        durationSeconds = seconds;
        StartCountdown();
    }

    public void StopCountdown()
    {
        isRunning = false;
    }

    void Update()
    {
        if (!isRunning)
            return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            isRunning = false;
        }

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (countdownText == null)
            return;

        int totalSeconds = Mathf.CeilToInt(remainingTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        countdownText.text = $"{minutes:00}:{seconds:00}";

        if (totalSeconds == 0)
        {
            countdownText.text = $"";
        }
    }
}
