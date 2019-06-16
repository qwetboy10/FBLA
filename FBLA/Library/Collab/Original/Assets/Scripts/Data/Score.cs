using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
public class Score : IComparable<Score>
{
    string name;
    int score;
    public Score(string s, int i)
    {
        name = s;
        score = i;
    }
    public int CompareTo(Score o)
    {
        return o.score - score;
    }
    public string toString()
    {
        if(name.Equals("---"))
        return "------" + " --- " + score.ToString("000000");
        else return name + " --- " + score.ToString("000000");
    }
    public Score(string scoreInput)
    {
        string[] k = scoreInput.Split(new string[] { " --- " }, StringSplitOptions.None);
        name = k[0];
        String kk = k[1].TrimStart('0');
        //score = kk.Length==0 ? 0 : int.Parse(kk);
        try
        {
            score = int.Parse(kk);
        }
        catch
        {
            score = 0;
        }
    }
}