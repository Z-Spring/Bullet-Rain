//注册

public class MsgRegister : MsgBase
{
    public MsgRegister()
    {
        protoName = "MsgRegister";
    }

    //客户端发
    public string id = "";

    public string pw = "";

    //服务端回（0-成功，1-失败）
    public int result = 0;
}


//登陆
public class MsgLogin : MsgBase
{
    public MsgLogin()
    {
        protoName = "MsgLogin";
    }

    //客户端发
    public string id = "";

    public string pw = "";

    //服务端回（0-成功，1-失败）
    public int result = 0;
}


//踢下线（服务端推送）
public class MsgKick : MsgBase
{
    public MsgKick()
    {
        protoName = "MsgKick";
    }

    public string reason = "";
}