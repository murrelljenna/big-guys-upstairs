using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarbarianWavePlayerUIController : MonoBehaviour
{
    private BarbarianWavePlayer player;

    public Text timer;
    public Text unitCount;

    public void Start()
    {
        if (timer == null || unitCount == null)
        {
            Debug.LogError("BarbarianWavePlayerUIController is missing some fields. Fix that");
        }

        player = BarbarianWavePlayer.Get();
        player.nextWaveReady.AddListener(updateValues);

        timer.text = BarbarianWaveSettings.WAVE_TIME_BASE.ToString();
        unitCount.text = BarbarianWaveSettings.BARBARIAN_WAVE_COUNT_BASE.ToString();
    }

    private void updateValues(int time, int count)
    {
        timer.text = time.ToString();
        unitCount.text = count.ToString();
    }
}
