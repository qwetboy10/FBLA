using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
/*
 * Score is a comparable class that allows for 
 * sorting of high scores, as well as easy retrival
 * of names and their associated scores.
 */
public class Score : IComparable<Score>
{
    string name;
    int score;
    public Score(string s, int i)
    {
        name = s;
        score = i;
    }
	//required method that defines comparisons between 2 scores
    public int CompareTo(Score o)
    {
        return o.score - score;
    }
	//return a string based off name and score
    public string toString()
    {
        if(name.Equals("---"))
        return "------" + " --- " + score.ToString("000000");
        else return name + " --- " + score.ToString("000000");
    }
	//parse a score from a given string
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