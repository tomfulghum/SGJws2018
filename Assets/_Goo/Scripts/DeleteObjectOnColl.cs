﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObjectOnColl : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9)
        {
            Destroy(this);
        }
    }

}
