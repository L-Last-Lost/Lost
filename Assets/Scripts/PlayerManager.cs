using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public Player player;
    private void Awake()
    {
        // 单例模式  即一个实例并且有一个访问点
        if(instance != null)
            Destroy(instance.gameObject);
        else
            instance = this; 
    }
}
