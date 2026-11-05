using System.Collections;
using UnityEngine;

public class RockGenerator : MonoBehaviour
{
    public GameObject[] rockPrefabs;
    private UIController ui;
    IEnumerator Start()
    {
        ui = FindFirstObjectByType<UIController>();
        while (true)
        {
            yield return new WaitForSeconds(Mathf.Clamp(1f - (ui == null ? 0 : ui.score) * 0.0005f, 0.5f, 1f));
            if (ui != null && ui.IsRunFinished) yield break;
            if (rockPrefabs != null && rockPrefabs.Length > 0)
                Instantiate(rockPrefabs[Random.Range(0, rockPrefabs.Length)], new Vector3(Random.Range(-2.5f, 2.5f), 6f, 0f), Quaternion.identity);
        }
    }
}
