using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_02_Shoot : MonoBehaviour
{
    public GameObject damageOrb;
    public Transform shootingPoint;

    private Character cc;

    private void Awake()
    {
        cc = GetComponent<Character>();
    }

    private void ShootTheDamageOrb()
    {
        Instantiate(damageOrb, shootingPoint.position, Quaternion.LookRotation(shootingPoint.forward));
    }

    private void Update()
    {
        cc.RotateToTarget();
    }
}
