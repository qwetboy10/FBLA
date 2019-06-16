using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Dragon is a subclass of Enemy that defines
 * its own specific attack, defense windows, and health.
 */
public class Dragon : Enemy {

	// Use this for initialization
	public override float delta
	{
		get
		{
			return -2.3f;
		}
	}
	public override float minDefendTime
	{
		get
		{
			return 1.25f;
		}
	}
	public override float maxDefendTime
	{
		get
		{
			return 1.917f;
		}
	}
	public override int background
	{
		get
		{
			return 1;
		}
	}
	public Animator myAnim;
	public override int getPointsPerKill() {return 500;}
	void Start () {
		myAnim = GetComponent<Animator>();
	}
	//handles attack sequence
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
	//handles defense sequence
	override public IEnumerator Defend() {
		myAnim.SetTrigger("hurt");
		yield return new WaitForSeconds(.3f);
		do {
			yield return null;
		} while (myAnim.GetCurrentAnimatorStateInfo(0).IsName("dragonHurt"));
	}
	//returns health that scales iwth difficulty
	override public int getMaxHealth() {
		return 100*(DataModel.getDifficulty()+2);
	}
	override public IEnumerator Die() {
		yield return null;
	}
	public IEnumerator waitThenTrigger() {
		yield return new WaitForSeconds (1.5f);
		RoomManagerScript.instance.damagePlayer (1);
	}
}
