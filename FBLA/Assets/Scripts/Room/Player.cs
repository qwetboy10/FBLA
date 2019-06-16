using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
/*
 * Player is the class that handles how a player interacts with other things
 * in the room. It includes methods that enable walking in and out, as well as
 * playing defense and attack animations.
 */
public class Player : Entity {
	public Vector3 leftOffScreen,rightOffScreen,playerIdleLoc;
	public Animator myAnim;
	void Start () {
		myAnim = GetComponent<Animator>();
	}
	//walk in
	public Coroutine Enter()
	{
		return MoveFromTo(leftOffScreen, playerIdleLoc);
	}
	//walk out
	public Coroutine Exit()
	{
		return StartCoroutine(MoveTo(rightOffScreen));
	}
	//move to location, play attack animation and damage enemy, walk back if the enemy hasn't died
	public IEnumerator Attack(Vector3 playerAttackLoc, int dmg, Action<int> damageEnemy, bool dead)
	{
		
		//move to enemy
		yield return StartCoroutine(MoveTo(playerAttackLoc));
		//play attack animation
		myAnim.SetTrigger("attack");
		StartCoroutine(waitThenDamage (dmg, damageEnemy));
		Debug.Log("Dealt " + dmg + " damage");
		yield return new WaitForSeconds(.3f);
		AudioManager.playAttack();
		do {
			yield return null;
		} while (myAnim.GetCurrentAnimatorStateInfo(0).IsName("boySword"));
		//move back
		if(dead) yield return StartCoroutine(MoveTo(playerIdleLoc));
		//.SetBool("moving",false);
		faceRight ();
	}
	//move to enemy player, play attack animation and damage enemy, walk back if the other player hasn't died
	public IEnumerator attackPlayer(Vector3 startLocation, Vector3 enemyAttackLocation, float dx,Player other,int dmg,bool hurtA)
	{
		yield return MoveTo(enemyAttackLocation - new Vector3(dx,0,0));
		AudioManager.playAttack();
		myAnim.SetTrigger("attack");
		yield return new WaitForSeconds(.3f);
		StartCoroutine(MUIController.instance.showDamage(dmg));
		if(hurtA) 
			DataModel.aChangeHearts(-dmg);
		else
			DataModel.bChangeHearts(-dmg);
		StartCoroutine(other.Defend());
		do {
			yield return null;
		} while (myAnim.GetCurrentAnimatorStateInfo(0).IsName("boySword"));
		yield return MoveTo(startLocation);
	}
	//starts potion drink animation
	public IEnumerator drinkPotion()
	{
		myAnim.SetTrigger("potion");
		yield return new WaitForSeconds(1f);
		AudioManager.playPotion();
		do {
			yield return null;
		} while (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("boyDrink"));
		do {
			yield return null;
		} while (myAnim.GetCurrentAnimatorStateInfo(0).IsName("boyDrink"));
	}
	//manages defense process
	public IEnumerator Defend() {
		myAnim.SetTrigger("hurt");
		do {
			yield return null;
		} while (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("playerHurt"));
		do {
			yield return null;
		} while (myAnim.GetCurrentAnimatorStateInfo(0).IsName("playerHurt"));
	}
	//waits for attack animation to play half way, then damages enemy
	public IEnumerator waitThenDamage(int damage, Action<int> damageEnemy) {
		yield return new WaitForSeconds (.45f);
		damageEnemy (damage);
	}
	//handles block animation
	public IEnumerator block()
	{
		Debug.Log("defend play");
		myAnim.SetTrigger("defend");
		do {
			yield return null;
		} while (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("boyDefend"));
		do {
			yield return null;
		} while (myAnim.GetCurrentAnimatorStateInfo(0).IsName("boyDefend"));
	}

}
