// Player resistance bar
// Fills up when too many towers are used
public class ResistanceBarUI : IndicatedProgressBarUI
{
    public override float getLatestValue()
    {
        return PlayerStatistics.resistancePoints / PlayerStatistics.defaultMaxResistancePoints;
    }
}
