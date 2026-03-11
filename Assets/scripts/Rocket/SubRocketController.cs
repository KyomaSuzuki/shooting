using UnityEngine;

public class SubRocketController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject explosionPrefab;
    public Transform rocketTransform; // Rocket の Transform
    private RocketController rocketController;


    private void Start()
    {
        rocketTransform = GameObject.Find("Rocket").transform;
    }

    
    void Update()
    {
        // Rocket の左右に位置を固定する
        if (rocketTransform != null)
        {
            float offset = transform.position.x < rocketTransform.position.x ? -1f : 1f;
            transform.position = new Vector3(rocketTransform.position.x + offset, rocketTransform.position.y, rocketTransform.position.z);
        }

        // スペースキーで弾を発射
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        }
    }

    public void SetRocketController(RocketController controller)
    {
        rocketController = controller;
    }

    public void OnDestroy()
    {
        // SubRocketが破壊された際にRocketControllerを更新
        if (rocketController != null)
        {
            rocketController.DestroySubRockets();
        }
    }
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Rock")
        {
            // 爆発エフェクトを生成
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

            Destroy(coll.gameObject); // Rock を破壊
            Destroy(gameObject);      // SubRocket を破壊
        }
    }
}
