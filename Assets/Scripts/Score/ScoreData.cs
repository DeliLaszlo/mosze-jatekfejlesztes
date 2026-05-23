using System;
using System.Collections.Generic;

[Serializable]
public class ScoreEntry
{
    public string playerName;
    public int score;
}

[Serializable]
public class ScoreData
{
    public List<ScoreEntry> scores = new List<ScoreEntry>();
}