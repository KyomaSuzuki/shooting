using UnityEngine;

public class RocketController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject explosionPrefab;
    public GameObject subRocketPrefab; // プレハブとしてのSubRocket
    private GameObject leftSubRocketInstance; // インスタンスとしてのSubRocket
    private GameObject rightSubRocketInstance; // インスタンスとしてのSubRocket
    private GameOverManager gameOverManager;
    private int redcount = 0;
    private bool yelloewItemFlag = false;
    private float yellowItemDuration = 10.0f;
    public  AudioClip BulletSounds;
    public AudioClip ExplosionSounds;
    public AudioClip ItemGetSounds;
    private AudioSource audioSource;

    //AudioManagerからPlaySoundを呼び出す
    
    private void PlaySound(AudioClip clip)
    {
        if(audioSource!=null&&clip!=null){
            audioSource.PlayOneShot(clip);
        }
    }

    private void DeactivateYellowItem(){
            yelloewItemFlag = false;
        }

    //bulletを呼び出す関数
    public void Bullet(float angle)
    {
        // Z軸を中心に回転させる
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        Instantiate(bulletPrefab, transform.position, rotation);

        // 弾を発射するときの効果音を再生

        if (BulletSounds != null)
        {
            PlaySound(BulletSounds);
        }
        else
        {
            Debug.LogError("BulletSounds is not set!");
        }
    }

    // 爆発エフェクトを生成する関数
    public void Explode()
    {
        GameObject effect=Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(effect, 1.0f);

        // 爆発音を再生
        if (ExplosionSounds != null)
        {
            PlaySound(ExplosionSounds);
            Debug.Log("Playing Explosion Sound");
        }
        else
        {
            Debug.LogError("ExplosionSounds is not set!");
            
        }
    }

    void Start()
    {
        gameOverManager = FindFirstObjectByType<GameOverManager>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) {
            if (transform.position.x > -2.0f) {
                transform.Translate(-0.025f, 0, 0);
            } else {
                transform.position = new Vector3(-2.0f, transform.position.y, transform.position.z);
            }
        }

        if (Input.GetKey(KeyCode.RightArrow)) {
            if (transform.position.x < 2.0f) {
                transform.Translate(0.025f, 0, 0);
            } else {
                transform.position = new Vector3(2.0f, transform.position.y, transform.position.z);
            }
        }

        /*if (Input.GetKeyDown(KeyCode.Space)) {
            Bullet(0); // 角度0で弾を発射
        }*/
        //たまの発射
        if(Input.GetKeyDown(KeyCode.Space)){
            if(yelloewItemFlag){
                Bullet(0);
                Bullet(30);
                Bullet(-30);
            }else{
                Bullet(0);
            }
        }

    }
    

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Rock") {
            // 爆発エフェクトを生成する

            Explode();

            Destroy(coll.gameObject);
            Destroy(gameObject);
            // ゲームオーバー時にSubRocketも破壊する
            DestroySubRockets();

            // SubRocketも破壊する
            /*if (subRocketInstance != null) {
                Debug.Log("SubRocket Destroyed");
                Destroy(subRocketInstance);
            } else {
                Debug.Log("SubRocket is null or not instantiated");
            }*/

            GameObject.Find("Canvas").GetComponent<UIController>().GameOver();
            

            // GameOverManagerにゲームオーバーを通知
            if (!gameOverManager.IsGameOver())
            {
                gameOverManager.TriggerGameOver();
                // ゲームオーバー時にSubRocketも破壊する
                
            }
        }
        //item_greenを取得した場合
        else if (coll.gameObject.tag == "item_green")
        {
            //効果音を再生
            if (ItemGetSounds != null)
            {
                PlaySound(ItemGetSounds);
            }
            else
            {
                Debug.LogError("ItemGetSounds is not set!");
            }
            //スコアに30点加算
            GameObject.Find("Canvas").GetComponent<UIController>().AddScore(30);
            //Itemを削除
            Destroy(coll.gameObject);
        }

        //item_redを取得した場合
       else if (coll.gameObject.tag == "item_red")
        {
            //効果音を再生
            if (ItemGetSounds != null)
            {
                PlaySound(ItemGetSounds);
            }
            else
            {
                Debug.LogError("ItemGetSounds is not set!");
            }
            // redcountを1増やす
            redcount++;

            if (redcount <= 2) {
            Vector3 pos = transform.position;
            
            // SubRocketの配置位置を調整
            if (redcount == 1) {
                // 左側に配置、画面端を超えないようにする
                float newX = Mathf.Clamp(pos.x - 1, -2.5f, 2.5f); // 範囲を指定
                leftSubRocketInstance = Instantiate(subRocketPrefab, new Vector3(newX, pos.y, pos.z), Quaternion.identity);
                // RocketのTransformをSubRocketに渡す
                leftSubRocketInstance.GetComponent<SubRocketController>().rocketTransform = this.transform;

                Debug.Log("SubRocket Instance Created at: " + leftSubRocketInstance.transform.position);
            } else if (redcount == 2) {
                // 右側に配置、画面端を超えないようにする
                float newX = Mathf.Clamp(pos.x + 1, -2.5f, 2.5f); // 範囲を指定
                rightSubRocketInstance = Instantiate(subRocketPrefab, new Vector3(newX, pos.y, pos.z), Quaternion.identity);
                // RocketのTransformをSubRocketに渡す
                rightSubRocketInstance.GetComponent<SubRocketController>().rocketTransform = this.transform;
                Debug.Log("SubRocket Instance Created at: " + rightSubRocketInstance.transform.position);
                //redcountをもとに戻す
                redcount = 0;
            }
            
            // SubRocketが画面端に出ないようにする
            if (leftSubRocketInstance != null) {
                float clampedX = Mathf.Clamp(leftSubRocketInstance.transform.position.x, -2.5f, 2.5f);
                leftSubRocketInstance.transform.position = new Vector3(clampedX, leftSubRocketInstance.transform.position.y, leftSubRocketInstance.transform.position.z);
            }
            if (rightSubRocketInstance != null) {
                float clampedX = Mathf.Clamp(rightSubRocketInstance.transform.position.x, -2.5f, 2.5f);
                rightSubRocketInstance.transform.position = new Vector3(clampedX, rightSubRocketInstance.transform.position.y, rightSubRocketInstance.transform.position.z);
            }
        }

            // Itemを削除
            Destroy(coll.gameObject);
        }
        //item_yellowを取得した場合
        else if(coll.gameObject.tag=="item_yellow"){
            //効果音を再生  
            if (ItemGetSounds != null)
            {
                PlaySound(ItemGetSounds);
            }
            else
            {
                Debug.LogError("ItemGetSounds is not set!");
            }
            yelloewItemFlag = true;
            Invoke(nameof(DeactivateYellowItem), yellowItemDuration);
            Destroy(coll.gameObject);
        }
        
    }
    public void DestroySubRockets(){
        if(leftSubRocketInstance != null){
            Destroy(leftSubRocketInstance);
            leftSubRocketInstance = null;
        }
        
        if(rightSubRocketInstance != null){
            Destroy(rightSubRocketInstance);
            rightSubRocketInstance = null;
        }
    }
    void OnDestroy()
    {
        DestroySubRockets();
    }
}