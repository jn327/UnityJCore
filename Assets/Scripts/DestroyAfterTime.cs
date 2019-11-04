using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour 
{
	[SerializeField]
    private MinMaxFloat _lifetime = new MinMaxFloat( 0.5f, 1.0f );

	void Start () 
	{
		StartCoroutine(DoDestroy());
	}

	IEnumerator DoDestroy()
	{
		yield return new WaitForSeconds(_lifetime.GetRandom());
		Destroy(this.gameObject);
	}
}
