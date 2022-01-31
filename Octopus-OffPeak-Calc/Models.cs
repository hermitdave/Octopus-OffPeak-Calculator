using System;

public class Consumption
{
    public int count { get; set; }
    public string next { get; set; }
    public string previous { get; set; }
    public Entry[] results { get; set; }
}

public class Entry
{
    public double consumption { get; set; }
    public DateTime interval_start { get; set; }
    public DateTime interval_end { get; set; }
}