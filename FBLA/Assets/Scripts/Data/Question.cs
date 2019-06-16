using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/*
 * Question stores a specific Question, as well as any data that is related to it.
 * This includes category types, answer choices, and the correct answer.
 * It also has helper methods that allow for hints to remove answer choices and a shuffling of answer choices.
 */
[System.Serializable]
public class Question {
	public string[] answers;
	private string[] backup;
	public string question;
	public int correctIndex;
	public string category;
	public bool hinted;
	public int backupIndex;
	public Question(string question, string[] answers, int correctIndex,string category) {
		this.question = question;
		this.answers = answers;
		this.correctIndex = correctIndex;
		this.category = category;
		backup = answers;
	}
	//shuffles answer choices to ensure variety; many questions have the first answer choice as correct,
	//so shuffling before use prevents predicatble correct answer locations.
	public void shuffle() {
		for (int i = 0; i < answers.Length-1; i++) {
			int j = (int) Random.Range (i+1, answers.Length);
			if (i == correctIndex)
			{
				correctIndex = j;
				backupIndex = j;
			}
			else if (j == correctIndex)
			{
				correctIndex = i;
				backupIndex = i;
			}
			string temp = answers [i];
			answers [i] = answers [j];
			answers [j] = temp;
		}
	}
	//utility method that removes incorrect answers for hints, leaving only 2
	public void hint()
	{
		if(hinted) return;
		hinted = true;
		List<string> ans = answers.ToList();
		int remove = ans.Count-2;
		for(int i=ans.Count-1;i>=0;i--)
		{
			if(i!=correctIndex&&remove>0)
			{
				ans.RemoveAt(i);
				if(correctIndex > i) correctIndex--;
				remove--;
			}
		}
		answers = ans.ToArray();
		
	}
	//reset a question to its unshuffled, unhinted version
	public void reset()
	{
		hinted = false;
		answers = backup;
		correctIndex = backupIndex;
	}
	//overriden method that allows easy debubgging of questions
	public override string ToString () {
		return question + " [" + string.Join (",", answers) + "], correct is " + answers [correctIndex];
	}
}
