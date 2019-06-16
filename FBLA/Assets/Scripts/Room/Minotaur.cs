using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Minotaur is the first enemy of the game,
 * with its own predefined constants that are lower than
 * that of the Dragon. It also uses the Enemy framework to
 * ensure that all methods are usable but have unique executions.
 */
public class Minotaur : Enemy {

	// Use this for initialization
	public override float delta
	{
		get
		{
			return -2f;
		}
	}
	public override float minDefendTime
	{
		get
		{
			return .8f;
		}
	}
	public override float maxDefendTime
	{
		get
		{
			return 1.3f;
		}
	}
	public override int background
	{
		get
		{
			return 0;
		}
	}
	public Animator myAnim;
	public override int getPointsPerKill() {return 300;}
	void Start () {
		myAnim = GetComponent<Animator>();
	}
	//plays attack animations
	override public IEnumerator Attack()
    {	
		
		myAnim.SetTrigger("attack");
		StartCoroutine (waitThenTrigger ());
		yield return new WaitForSeconds(.3f);
		do {
			yield return null;
		} while (myAnim.GetCurrentAnimatorStateInfo(0).IsName("minotaurAttack"));
       	yield return null;
	}
	//plays defense animations
	override public IEnumerator Defend() {
		myAnim.SetTrigger("hurt");
		yield return new WaitForSeconds(.3f);
		do {
			yield return null;
		} while (myAnim.GetCurrentAnimatorStateInfo(0).IsName("minotaurHurt"));
       	yield return null;
	}
	//gets max health that scales with difficulty
	override public int getMaxHealth() {
		return 70*(DataModel.getDifficulty()+2);
	}
	override public IEnumerator Die() {
		yield return null;
	}
	public IEnumerator waitThenTrigger() {
		yield return new WaitForSeconds (1.5f);
		RoomManagerScript.instance.damagePlayer (1);
	}

}