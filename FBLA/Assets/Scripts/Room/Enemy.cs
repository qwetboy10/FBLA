using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Parent class for many enemies, creates framework
 * for methods such as Attack and Defend that represent
 * basic inheritance principles of polymorphism.
 */
public abstract class Enemy : Entity {
	public abstract float delta
    {
        get;
    }
	public abstract float minDefendTime
	{
		get;
	}
	public abstract float maxDefendTime
	{
		get;
	}
	public abstract int background
	{
		get;
	}
	public Vector3 enemyIdleLoc;
	//methods that each enemy must have but are still unique to each enemy
	abstract public IEnumerator Attack();
	abstract public IEnumerator Defend();
	abstract public IEnumerator Die();
	abstract public int getMaxHealth ();
	abstract public int getPointsPerKill ();
    // Use this for initialization
    void Start () {
		TeleportTo (enemyIdleLoc);
	}
	public Vector3 getEnemyIdleLoc()
	{
		return enemyIdleLoc;
	}
	public Vector3 getPlayerAttackLoc()
	{
		return new Vector3((float) (enemyIdleLoc.x + delta),-1.7f,0f);
	}
}
