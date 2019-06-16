using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * TrainingDummy is the enemy that appears in the Tutorial.
 * It has different stats than that of the dragon to ensure
 * the tutorial is unfailable.
 */
public class TrainingDummy : Enemy {

	// Use this for initialization
	public override float delta
	{
		get
		{
			return -1.5f;
		}
	}
	public override float minDefendTime
	{
		get
		{
			return 1;
		}
	}
	public override float maxDefendTime
	{
		get
		{
			return 2;
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
	void Start () {
		myAnim = GetComponent<Animator>();
	}
	public override int getPointsPerKill() {return 200;}
	//attack sequence
	override public IEnumerator Attack()
	{	
		myAnim.SetTrigger("attack");
		StartCoroutine (waitThenTrigger ());
		yield return new WaitForSeconds(.3f);
		do {
			yield return null;
		} while (myAnim.GetCurrentAnimatorStateInfo(0).IsName("dragonAttack"));
		yield return null;
	}
	//defense sequence
	override public IEnumerator Defend() {
		myAnim.SetTrigger("hurt");
		yield return new WaitForSeconds(.3f);
		do {
			yield return null;
		} while (myAnim.GetCurrentAnimatorStateInfo(0).IsName("dragonHurt"));
	}
	override public int getMaxHealth() {
		return 100;
	}
	override public IEnumerator Die() {
		yield return null;
	}
	public IEnumerator waitThenTrigger() {
		yield return new WaitForSeconds (1.5f);
		TutorialRoomManager.instance.damagePlayer (1);
	}
}
