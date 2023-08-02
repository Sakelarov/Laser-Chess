using UnityEngine;
using System.Collections;

public class ShotBehavior : MonoBehaviour
{
	private float currentFlyDistance;
	private float flyDistance;
	
	public void Setup(float distance)
	{
		flyDistance = distance;
		gameObject.SetActive(true);
	}
	private void Update ()
	{
		currentFlyDistance += transform.forward.magnitude * Time.deltaTime * 10f;
		
		if (currentFlyDistance < flyDistance)
			transform.position += transform.forward * (Time.deltaTime * 10f);
		else Destroy(gameObject);
		
	}
}
