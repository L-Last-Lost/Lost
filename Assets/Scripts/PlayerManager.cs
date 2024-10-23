using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public Player player;
    private void Awake()
    {
        // ����ģʽ  ��һ��ʵ��������һ�����ʵ�
        if(instance != null)
            Destroy(instance.gameObject);
        else
            instance = this; 
    }
}
