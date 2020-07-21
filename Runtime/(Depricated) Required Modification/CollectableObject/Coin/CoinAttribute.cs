using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinAttribute : MonoBehaviour {

	#region Public Variables
	[Range(1,1000000)]
	public int coinToBeAwarded;
	[Range(0,1000)]
	public int requiredLevelToBeSpawned;

	[Space(2.5f)]
	[Header("Beautification")]
	public Color particleColor;
	public Color particleTrailColor;
	public TrailRenderer trailRendererReference;

	#endregion

	//----------
	#region Mono Behaviour

	void Start(){

		CoinManager.Instance.AddOre(transform,trailRendererReference);
	}

	/// <summary>
	/// Sent when another object enters a trigger collider attached to this
	/// object (2D physics only).
	/// </summary>
	/// <param name="other">The other Collider2D involved in this collision.</param>
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player"){

            //Info : Show Coin On Dispaly

            CoinManager.Instance.AddCoinToUser(coinToBeAwarded,particleColor);

            //Info : Coin Collection Effect
			

			Destroy(gameObject);
		}
	}

	public void OnGameEndCollect(){
        
		Destroy(gameObject);
	}

	#endregion
}
