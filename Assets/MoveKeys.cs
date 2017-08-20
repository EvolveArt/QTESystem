using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveKeys : MonoBehaviour {

    private static MoveKeys instance;
    public static MoveKeys Instance
    {
        get
        {
            return instance;
        }
    }

    private List<GameObject> keys = new List<GameObject>();
    private Dictionary<GameObject, Vector3> keysPositions = new Dictionary<GameObject, Vector3>();
    private Vector3 desiredPosition;

    private float speed;

    protected void Awake()
    {
        if (instance == null)
            instance = this;
    }

    protected void Start()
    {
        speed = QTE.Instance.keysSpeed;
    }

    #region KeysManagement
    public void UpdateKeysPositions()
    {
        foreach (GameObject key in keys)
        {
            keysPositions[key] = key.transform.localPosition;
        }

        speed = QTE.Instance.keysSpeed;
    }

    public void AddKeyToList(GameObject key)
    {
        keys.Add(key);
    }

    public void ClearKeysList()
    {
        if(keys.Count != 0)
            keys.Clear();
    }
    #endregion

    protected void Update()
    {
        if (!QTE.Instance.waveInProgress || !QTE.Instance.keysMoving)
            return;

        speed += .15f * Time.deltaTime;
        //elapsedTime = Time.time - startTime;

        foreach (GameObject key in keys)
        {
            if(keysPositions[key] == key.transform.localPosition)
            {
                keysPositions[key] = GetRandomPosition(key);
                Debug.Log(keysPositions[key]);
            }

            key.transform.localPosition = Vector3.MoveTowards(key.transform.localPosition, keysPositions[key], speed);
        }
    }

    private Vector3 GetRandomPosition(GameObject key)
    {
        float x = Random.Range(-Screen.width / 2 + key.transform.localScale.x*2, Screen.width / 2 - key.transform.localScale.x*2);
        float y = Random.Range(-Screen.height / 2 + key.transform.localScale.y*2, Screen.height / 2 - key.transform.localScale.y*2);
        desiredPosition = new Vector2(x, y);

        return desiredPosition;
    }

    private IEnumerator Move(GameObject go, Vector3 endPos, float speed)
    {
        while(go.transform.localPosition != endPos)
        {
            go.transform.localPosition = Vector3.Lerp(go.transform.localPosition, endPos, speed * Time.deltaTime);
            yield return null;
        }
    }
}
