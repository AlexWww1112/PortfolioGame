using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject explosionPrefab; // 把你的爆炸 Particle System 拖进来
    public Transform followTarget;     // 模型上某个部位，比如脚、腰部、背后骨骼等

    void StartExplosion()
    {
        GameObject explosion = Instantiate(explosionPrefab, followTarget.position, Quaternion.identity);
        explosion.transform.SetParent(followTarget); // 跟着目标动
    }

    void Start()
    {
        lastPosition = transform.position;
        StartCoroutine(RandomExplosionLoop());
    }

    IEnumerator RandomExplosionLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(2f, 6f);
            yield return new WaitForSeconds(waitTime);

            if (isMoving)
            {
                StartExplosion();
            }
        }
    }
    Vector3 lastPosition;
    bool isMoving;


    void Update()
    {
        isMoving = Vector3.Distance(transform.position, lastPosition) > 0.01f;
        lastPosition = transform.position;
    }
}
