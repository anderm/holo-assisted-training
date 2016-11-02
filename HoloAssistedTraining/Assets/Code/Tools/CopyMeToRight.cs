using UnityEngine;
using System.Collections;

public class CopyMeToRight : MonoBehaviour
{

    public float offsetRight;
    public float offsetDown;
    public float timeBetweenSpawn;
    public bool isSpawning;
    public bool spawnNow = true;
    public int numberOfCopies;
    public int cloneNumber = 1;
    private Vector3 basePos;
    //public float waitTimeBeforeCloning;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isSpawning)
        {
            if (spawnNow)
            {
                spawnNow = false;
                GameObject go = (GameObject)Instantiate(gameObject, basePos + transform.forward * offsetDown + transform.right * offsetRight * cloneNumber, transform.rotation);
                go.AddComponent<LerpMeToOffset>();
                go.GetComponent<LerpMeToOffset>().SetOffset(offsetDown);
                SceneManager.Instance.CopiedPistons.Add(go);

                cloneNumber++;
                if (cloneNumber > numberOfCopies)
                {
                    isSpawning = false;
                }
                StartCoroutine(WaitBeforeCloning());
            }
        }
    }

    public void StartSpawning(Vector3 temp)
    {

        isSpawning = true;
        basePos = temp;

    }

    IEnumerator WaitBeforeCloning()
    {

        yield return new WaitForSeconds(timeBetweenSpawn);
        spawnNow = true;
    }
}
