using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarbarianWavePlayerUIController : MonoBehaviour
{
    private BarbarianWavePlayer player;

    public Text timer;
    public Text unitCount;

    private int timerAmt;

    private Coroutine currentTimer;

    public void Start()
    {
        if (timer == null || unitCount == null)
        {
            Debug.LogError("BarbarianWavePlayerUIController is missing some fields. Fix that");
        }

        player = BarbarianWavePlayer.Get();
        player.nextWaveReady.AddListener(updateValues);

        updateValues((int)BarbarianWaveSettings.WAVE_TIME_BASE, BarbarianWaveSettings.BARBARIAN_WAVE_UNIT_COUNT_BASE);
    }

    private void updateValues(int time, int count)
    {
        timerAmt = time;
        timer.text = time.ToString();
        unitCount.text = "x" + count.ToString();
        restartTimer();
    }

    private void restartTimer()
    {
        if (currentTimer != null)
        {
            StopCoroutine(currentTimer);
        }
        currentTimer = StartCoroutine(countDown());
    }

    private IEnumerator countDown()
    {
        while (timerAmt > 0)
        {
            yield return new WaitForSeconds(1);
            timerAmt--;
            timer.text = timerAmt.ToString();
        }
    }
}
