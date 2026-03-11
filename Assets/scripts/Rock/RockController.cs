using UnityEngine;
using System.Collections;

public class RockController : MonoBehaviour {

	float fallSpeed;
	float rotSpeed;

	void Start () {
		this.fallSpeed = 0.01f;
		this.rotSpeed = 5f + 3f * Random.value;
	}
	
	void Update () {
		transform.Translate( 0, -fallSpeed, 0, Space.World);
		transform.Rotate(0, 0, rotSpeed);

		// 画面外に出たら自動削除（メモリ節約）
		if (transform.position.y < -7f) {
			Destroy(gameObject);
		}
	}
}
