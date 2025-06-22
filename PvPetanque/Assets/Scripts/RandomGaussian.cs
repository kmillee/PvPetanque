using System;

public class RandomGaussian : Random
{
    private bool hasSpareValue;
    private double spareValue;
    
    public float NextGaussian(double mean = 0, double stdDev = 1)
    {
        
        if (hasSpareValue)
        {
            hasSpareValue = false;
            return (float) (mean + stdDev * spareValue);
        }

        double u, v, s;
        do
        {
            u = 2.0 * NextDouble() - 1.0;
            v = 2.0 * NextDouble() - 1.0;
            s = u * u + v * v;
        } while (s >= 1.0 || s == 0);

        s = Math.Sqrt(-2.0 * Math.Log(s) / s);
        spareValue = v * s;
        hasSpareValue = true;
        
        // Clamping within 3 stdDev
        double min = mean - 3 * stdDev;
        double max = mean + 3 * stdDev;

        return (float) Math.Clamp(mean + stdDev * (u * s), min, max);
    }
}
