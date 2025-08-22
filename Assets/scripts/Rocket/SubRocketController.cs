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
            GameObject effect = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 1.0f);

            Destroy(coll.gameObject); // Rock を破壊
            Destroy(gameObject);      // SubRocket を破壊
        }
    }
}
