using System;

[Serializable]
public class RewardBundle
{
    public int energy;
    public int memoryFragments;
    public int runes;

    public RewardBundle(int energy = 0, int memoryFragments = 0, int runes = 0)
    {
        this.energy = energy;
        this.memoryFragments = memoryFragments;
        this.runes = runes;
    }

    public void Add(RewardBundle other)
    {
        if (other == null) return;

        energy += other.energy;
        memoryFragments += other.memoryFragments;
        runes += other.runes;
    }
}
