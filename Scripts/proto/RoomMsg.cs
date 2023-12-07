using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 查询战绩
public class MsgGetAchieve : MsgBase
{
    public MsgGetAchieve()
    {
        protoName = "MsgGetAchieve";
    }

    public int win = 0;
    public int lost = 0;
}

// 房间信息
[Serializable]
public class RoomInfo
{
    public int id = 0;

    public int count = 0;

    // 0-准备中 1-战斗中
    public int status = 0;
}

// 获取房间列表
public class MsgGetRoomList : MsgBase
{
    public MsgGetRoomList()
    {
        protoName = "MsgGetRoomList";
    }

    public RoomInfo[] rooms;
}

// 创建房间
public class MsgCreateRoom : MsgBase
{
    public MsgCreateRoom()
    {
        protoName = "MsgCreateRoom";
    }

    public int result = 0;
    public string reason = "";
}

// 进入房间
public class MsgEnterRoom : MsgBase
{
    public MsgEnterRoom()
    {
        protoName = "MsgEnterRoom";
    }

    // player id
    public int id = 0;
    public int result = 0;
}

// 离开房间
public class MsgLeaveRoom : MsgBase
{
    public MsgLeaveRoom()
    {
        protoName = "MsgLeaveRoom";
    }

    public int result = 0;
}

// 开始战斗
public class MsgStartBattle : MsgBase
{
    public MsgStartBattle()
    {
        protoName = "MsgStartBattle";
    }

    public int result = 0;
    public string reason = "";
}

// 房间玩家
[Serializable]
public class PlayerInfo
{
    public string id = "spring";
    public int camp = 0;
    public int win = 0;

    public int lost = 0;

    // 1-房主
    public int isOwner = 0;
}

// 获取房间信息
public class MsgGetRoomInfo : MsgBase
{
    public MsgGetRoomInfo()
    {
        protoName = "MsgGetRoomInfo";
    }

    public PlayerInfo[] players;
}