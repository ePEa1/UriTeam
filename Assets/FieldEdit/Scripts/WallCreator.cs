using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WallCreator : MonoBehaviour
{
    [SerializeField, LabelText("Wall Object")] GameObject wall;
    [SerializeField, LabelText("벽 길이")] float length;

    // Start is called before the first frame update
    void Awake()
    {
        GameObject w = Instantiate(wall);
        w.transform.parent = transform;
        w.transform.localPosition = new Vector3(0.0f, 0.5f, length / 2.0f);
        w.transform.localEulerAngles = Vector3.zero;
        w.GetComponent<BoxCollider>().size = new Vector3(length, 10.0f, 1.0f);

        w = Instantiate(wall);
        w.transform.parent = transform;
        w.transform.localPosition = new Vector3(0.0f, 0.5f, -length / 2.0f);
        w.transform.localEulerAngles = Vector3.zero;
        w.GetComponent<BoxCollider>().size = new Vector3(length, 10.0f, 1.0f);

        w = Instantiate(wall);
        w.transform.parent = transform;
        w.transform.localPosition = new Vector3(length / 2.0f, 0.5f, 0.0f);
        w.transform.localEulerAngles = Vector3.zero;
        w.GetComponent<BoxCollider>().size = new Vector3(1.0f, 10.0f, length);

        w = Instantiate(wall);
        w.transform.parent = transform;
        w.transform.localPosition = new Vector3(-length / 2.0f, 0.5f, 0.0f);
        w.transform.localEulerAngles = Vector3.zero;
        w.GetComponent<BoxCollider>().size = new Vector3(1.0f, 10.0f, length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
