// Enemy wave bar
// Fills up wave after wave
public class WaveBarUI : IndicatedProgressBarUI
{
    protected override float getLatestValue()
    {
        return WaveSpawner.instance.getWaveProgression();
    }
}
