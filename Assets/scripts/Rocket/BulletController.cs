using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

	public GameObject explosionPrefab;

	void Update () {
		transform.Translate (0, 0.2f, 0);

		if (transform.position.y > 8) {
			Destroy (gameObject);
		}
	}

	/*void OnTriggerEnter2D(Collider2D coll) {
		// 衝突したときにスコアを更新する
		GameObject.Find ("Canvas").GetComponent<UIController> ().AddScore ();

               // 爆発エフェクトを生成する	
		GameObject effect = Instantiate (explosionPrefab, transform.position, Quaternion.identity) as GameObject;
		Destroy (effect, 1.0f);

		Destroy (coll.gameObject);	
		Destroy (gameObject);
	}*/

    //tagを使って判定する
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Rock")
        {
            // 衝突したときにスコアを更新する
            GameObject.Find("Canvas").GetComponent<UIController>().AddScore(10);

            // 爆発エフェクトを生成する
            GameObject effect;
            if (explosionPrefab != null) {
                effect = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            } else {
                effect = new GameObject("Explosion");
                effect.transform.position = transform.position;
                ParticleSystem ps = effect.AddComponent<ParticleSystem>();
                var main = ps.main;
                main.duration = 1f;
                main.startLifetime = 0.5f;
                main.startSpeed = 5f;
                main.startSize = 0.5f;
                main.startColor = new Color(1f, 0.5f, 0f);
                var emission = ps.emission;
                emission.rateOverTime = 0;
                emission.SetBursts(new ParticleSystem.Burst[]{ new ParticleSystem.Burst(0f, 30) });
                var shape = ps.shape;
                shape.shapeType = ParticleSystemShapeType.Sphere;
            }
            Destroy(effect, 1.0f);

            Destroy(coll.gameObject);
            Destroy(gameObject);
        }
    }
}