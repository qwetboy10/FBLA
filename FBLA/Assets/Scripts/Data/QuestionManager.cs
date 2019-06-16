using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * QuestionManager handles the creation and accessing of questions.
 * This ranges from loading questions from a data file to getting random questions by category and/or difficulty.
 */
public static class QuestionManager
{
    //stores all the questions loaded, without a category mappings
	private static List<List<Question>[]> values;
	//stores the questions in a Dictionary
	//Category => List of questions array, one for each available difficulty
    private static Dictionary<string, List<Question>[]> qdict = new Dictionary<string, List<Question>[]>();
	private static bool loaded;
    public static void init()
    {
		if(loaded) return;
        string[] lines = Resources.Load<TextAsset>("QuestionData").text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0) lines = System.Text.Encoding.Default.GetString(Resources.Load<TextAsset>("QuestionData").bytes)
               .Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        int i = 0;
		loaded = true;
        while (i < lines.Length)
        {
			//prevent frameshift errors by ensuring categories start with the word "Category"
            if (lines[i].Contains("Category"))
            {
				//initialize list arrays for question loading
                String cat = lines[i++].Substring(9);
                List<Question>[] qlist = new List<Question>[5];
                for (int asdf = 0; asdf < 5; asdf++)
                    qlist[asdf] = new List<Question>();
				//prevent frameshift errors by ensuring question start with the word "Question"
                while (i < lines.Length && lines[i].Contains("Question"))
                {
                    i++;
                    int n = Int32.Parse(lines[i++]);
					int diff = Int32.Parse (lines [i++]);
                    string question = lines[i++];
                    string[] answers = new string[n];
                    for (int j = 0; j < n; j++)
                        answers[j] = lines[i++];
                    int correctIndex = Int32.Parse(lines[i++]);
                    qlist[diff].Add(new Question(question, answers, correctIndex,cat));
					//any question above 148 characters does not fit in the room text box
                    if(question.Length > 148) Debug.LogWarning("QUESTION TO LONG");
                }
                qdict.Add(cat.Trim(), qlist);
            }
            else
            {
				//question data is not properly formed, dont load invalid questions
                qdict.Clear();
                Debug.Log("Question Data is malformed; error occured at line " + i + ": " + lines[i]);
				loaded = false;
                break;
            }
        }
		if(loaded) 
			values = new List<List<Question>[]> (qdict.Values);
    }
	//randomly picks a category, and picks a random question from a given difficulty 
    public static Question getRandomQuestion(int dif)
    {
        List<Question> qlist = values[(int)UnityEngine.Random.Range(0, values.Count)][dif];
        Question q = qlist[UnityEngine.Random.Range(0, qlist.Count)];
        q.shuffle();
        return q;
    }
	//picks a random question from a given difficulty and category
    public static Question getQuestionByCategory(string cat, int dif)
    {
        List<Question> qlist = qdict[cat.Trim()][dif];
        Question q = qlist[UnityEngine.Random.Range(0, qlist.Count)];
        q.shuffle();
        return q;
    }
	//returns a list of categories
    public static List<string> getCategories()
    {
        List<string> cat = new List<string>(qdict.Keys);
        return cat;
    }
}