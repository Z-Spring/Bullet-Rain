using System;

[Serializable]
public class TankInfo
{
    public string id = "";
    public int camp;
    public int hp;

    public float x, y, z;
    public float ex, ey, ez;
}

public class MsgEnterBattle : MsgBase
{
    public MsgEnterBattle()
    {
        protoName = "MsgEnterBattle";
    }

    public TankInfo[] tanks;

    // 地图id，目前用不到，因为只有一个地图
    public int mapId = 0;
}

public class MsgLeaveBattle : MsgBase
{
    public MsgLeaveBattle()
    {
        protoName = "MsgLeaveBattle";
    }

    public string id = "";
}

public class MsgBattleResult : MsgBase
{
    public MsgBattleResult()
    {
        protoName = "MsgBattleResult";
    }

    // 获胜的阵营，1红胜 2蓝胜
    public int winCamp = 0;
}