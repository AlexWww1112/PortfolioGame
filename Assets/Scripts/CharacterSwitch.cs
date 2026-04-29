using UnityEngine;

public class CharacterSwitch : MonoBehaviour
{
    public GameObject firstPerson1; // 第一个First Person Controller
    public GameObject firstPerson2; // 第二个First Person Controller

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 确保只有玩家可以触发
        {
            // 禁用第一个First Person Controller
            firstPerson1.SetActive(false);

            // 启用第二个First Person Controller
            firstPerson2.SetActive(true);
        }
    }
}
