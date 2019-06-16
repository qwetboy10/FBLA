using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Ghost is an unused testing enemy with
 * its own defined health and attack. 
 */
public class Ghost : Enemy {

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
	public Vector3 enemyAttackLoc;
	public override int getPointsPerKill() {return 200;}
	//start attack sequence
	override public IEnumerator Attack()
    {
        //move to player
		yield return StartCoroutine(MoveTo(enemyAttackLoc));
        //play attack animation

        //move back
		yield return StartCoroutine(MoveTo(enemyIdleLoc));
		faceLeft ();
    }
	override public IEnumerator Defend() {
		yield return null;
	}
	override public int getMaxHealth() {
		return 0;
	}
	override public IEnumerator Die() {
		yield return null;
	}
}
