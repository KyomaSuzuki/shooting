using System.Collections;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public GameObject[] items;
    IEnumerator Start()
    {
        var ui = FindFirstObjectByType<UIController>();
        while (true)
        {
            yield return new WaitForSeconds(3f);
            if (ui != null && ui.IsRunFinished) yield break;
            if (items != null && items.Length > 0)
                Instantiate(items[Random.Range(0, items.Length)], new Vector3(Random.Range(-2f, 2f), 5.5f, 0f), Quaternion.identity);
        }
    }
}
