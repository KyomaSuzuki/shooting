using UnityEngine;
using System.Collections;

public class RockGenerator : MonoBehaviour {

	public GameObject[] rockPrefabs;
    //隕石生成の時間
    float GenRockTime;

	void Start () {

        //スコアの取得
        int score = GameObject.Find("Canvas").GetComponent<UIController>().score;
        //スコアに応じて隕石生成の時間を変更
        this.GenRockTime = 1.0f - 0.05f * score;

        //生成する設定
		InvokeRepeating ("GenRock", GenRockTime, 1);
	}
	
    //生成する
    void GenRock () {
        //ランダムに隕石を生成
        int index = Random.Range(0, rockPrefabs.Length);
        GameObject rock = rockPrefabs[index];

        Instantiate (rock, new Vector3 (-2.5f + 5 * Random.value, 6, 0), Quaternion.identity);
    
    }
}
