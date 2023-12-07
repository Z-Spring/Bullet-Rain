using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgSyncPlayer : MsgBase
{
    public MsgSyncPlayer()
    {
        protoName = "MsgSyncPlayer";
    }

    public float x = 0f;
    public float y = 0f;
    public float z = 0f;

    public float ex = 0;
    public float ey = 0;
    public float ez = 0;


    public string id = "";
}

//开火
public class MsgFire : MsgBase
{
    public MsgFire()
    {
        protoName = "MsgFire";
    }

    //炮弹初始位置、旋转
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;
    public float ex = 0f;
    public float ey = 0f;
    public float ez = 0f;

    //服务端补充
    public string id = ""; //哪个坦克
}

public class MsgBullet : MsgBase
{
    public MsgBullet()
    {
        protoName = "MsgBullet";
    }

    public float x = 0f;
    public float y = 0f;
    public float z = 0f;
    public float ex = 0f;
    public float ey = 0f;
    public float ez = 0f;
}

//击中
public class MsgHit : MsgBase
{
    public MsgHit()
    {
        protoName = "MsgHit";
    }

    //击中谁
    public string targetId = "";

    //击中点 -- 暂时不用   防止作弊
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;

    //服务端补充
    public string id = ""; 
    public int hp = 0; 
    public int damage = 0; //受到的伤害
}