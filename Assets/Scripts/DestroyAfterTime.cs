using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour 
{
	private float _lifetime = 2.0f;

	void Start () 
	{
		StartCoroutine(DoDestroy());
	}

	IEnumerator DoDestroy()
	{
		yield return new WaitForSeconds(_lifetime);
		Destroy(this.gameObject);
	}
}
