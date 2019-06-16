using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Basic class that players and enemies derive from.
 * This gives utility methods such as movement and
 * sprite reflection to make them move in the right direction.
 */
public class Entity : MonoBehaviour
{
    public float speed;
	private bool moving;
	//enables smooth moving from current location to another
	public IEnumerator MoveTo(Vector3 tar)
    {
		Animator anim = GetComponent<Animator> ();
		bool right = false;
		if (right = transform.position.x < tar.x)
			faceRight ();
		else
			faceLeft ();
		if(anim != null) anim.SetBool ("moving", true);
		while((transform.position - tar).sqrMagnitude > float.Epsilon && (transform.position.x < tar.x == right)) {
			transform.position = Vector3.MoveTowards (transform.position, tar, speed * Time.deltaTime);
			yield return null;
		}
		if(anim != null) anim.SetBool ("moving", false);
		moving = false;
    }
	//sets current location, then moves to another
	public Coroutine MoveFromTo(Vector3 from, Vector3 to)
    {
        transform.position = from;
		return StartCoroutine(MoveTo(to));
    }
    public void TeleportTo(Vector3 loc)
    {
        transform.position = loc;
    }
	public bool isMoving()
	{
		return moving;
	}
	//utilizes GameConstants to change direction (in case the default sprite facing direction changes)
	public void faceRight() {
		GameConstants.faceRight (GetComponent<SpriteRenderer> ());
	}
	public void faceLeft() {
		GameConstants.faceLeft (GetComponent<SpriteRenderer> ());
	}
}
