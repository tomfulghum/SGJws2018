using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimultaneousActivation : MonoBehaviour
{

    public GameObject[] simultaneous;
    public GameObject endScreen;
    private bool[] flag;

    void Start()
    {
        flag = new bool[simultaneous.Length];
    }

    private void OnEnable()
    {
        Point.OnDeactivate += AddMassSpring;
    }


    private void AddMassSpring(GameObject gm)
    {
        for (int i = 0; i < flag.Length; i++)
        {
            if (simultaneous[i] == gm)
                flag[i] = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < flag.Length; i++)
            if (!flag[i])
            {
                return;
            }
        GameObject.Instantiate(endScreen);
        Destroy(this.gameObject);
    }
}
