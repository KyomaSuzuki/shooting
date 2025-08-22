using UnityEngine;
using System.Collections;

public class ItemGenerator : MonoBehaviour {

    public GameObject[] items; // 複数のアイテムプレハブを保持する配列

    // アイテム生成の時間
    float genItemTime;

    void Start () {
        // スコアの取得
        int score = GameObject.Find("Canvas").GetComponent<UIController>().score;
        // スコアに応じてアイテム生成の時間を変更
        this.genItemTime = 1.0f + 0.05f * score;

        // 生成する設定
        InvokeRepeating("GenItem", genItemTime, 1);
    }
    
    // アイテムを生成する
    void GenItem () {
        // ランダムにアイテムを選択
        if (items.Length == 0) {
            Debug.LogError("Items array is empty!");
            return;
        }

        int index = Random.Range(0, items.Length);
        GameObject item = items[index];

        // アイテムを生成
        Vector3 spawnPosition = new Vector3(-2.5f + 3 * Random.value, 2, 0);
        Instantiate(item, spawnPosition, Quaternion.identity);

    }
}