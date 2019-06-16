using System;
using System.Collections;
using UnityEngine;
public static class GameConstants {
	public static readonly int[] maxHearts = new int[] {10, 8, 1};
	public static readonly Vector3 heartStart = new Vector3 (25,25,0);
	public static readonly Vector3 scoreBoardStart = new Vector3 (960/4,800/4,0);
	public static readonly Vector3 enterScoreBoardStart = new Vector3 (1420,70,0);
	public static readonly int scoreBoardDelta = 70;
	public static readonly int heartDelta = 30;
	public static readonly int scoreBoardSize = 8;
	public static void faceLeft(SpriteRenderer s) {
		s.flipX = false;
	}
	public static void faceRight(SpriteRenderer s) {
		s.flipX = true;
	}
	public static readonly float hitmarkerInitialVelocity = 13.5f;
	public static readonly float hitmarkerAcc = .4f;
	public static readonly int framesPerCycle = 140;
	public static readonly float maxDisplacement = 440;
	public static readonly int baseDamage = 100;
	public static readonly float damageNumberTime = 10f;
	public static readonly float damageNumberFadeSpeed = 1f;
	public static readonly Vector3 chestLocation = new Vector3(5,-2,0);
	public static readonly float chestDelta = 1.7f;
	public static readonly Vector3 smokeLocation = new Vector3(3.54f,-2.36f,0);
	public static readonly float[] timerLength = new float[] {40f,30f,20f};
	public static readonly int[] enemyCount = new int[] {3,4,5};
	public static readonly int[][] enemyChange = new int[][] {new int[] {2},new int[] {2},new int[] {3}};
}