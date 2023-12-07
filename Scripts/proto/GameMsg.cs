public class MsgGetGameTime : MsgBase
{
    public MsgGetGameTime()
    {
        protoName = "MsgGetGameTime";
    }

    public float gameTime;
}