public class MsgSwitchWeapon : MsgBase
{
    public MsgSwitchWeapon()
    {
        protoName = "MsgSwitchWeapon";
    }

    public string id;
    public int weaponId;
}

public class MsgSyncWeaponPosition : MsgBase
{
    public MsgSyncWeaponPosition()
    {
        protoName = "MsgSyncWeaponPosition";
    }

    public bool isScope;

    public string id;
}