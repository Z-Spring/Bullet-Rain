using System;

[Serializable]
public class PlayerData
{
    public string id = "";
    public int win;
    public int loss;
    public int coin;
    public string text = "";
}

[Serializable]
public class ResultInfo
{
    public string id = "";
    public string data = "";
}

[Serializable]
public class PlayerRecord
{
    public int win;
    public int lost;
}

public class MsgResult : MsgBase
{
    public MsgResult()
    {
        protoName = "MsgResult";
    }

    //服务端回
    public PlayerData[] playerData;
}