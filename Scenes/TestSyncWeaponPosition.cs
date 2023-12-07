using UnityEngine;

public class TestSyncWeaponPosition : MonoBehaviour
{
    public Transform weaponHolder;
    private Vector2 lastWeaponLocalPosition;
    private Vector3 lastWeaponLocalRotation;

    private void Start()
    {
        lastWeaponLocalPosition = new Vector2(weaponHolder.localPosition.x, weaponHolder.localPosition.z);
        lastWeaponLocalRotation = weaponHolder.localEulerAngles;
        Debug.Log("lastWeaponLocalPosition: " + lastWeaponLocalPosition);
        Debug.Log("lastWeaponLocalRotation: " + lastWeaponLocalRotation);
    }

    void Update()
    {
        Vector2 currentWeaponLocalPosition =
            new Vector2(weaponHolder.localPosition.x, weaponHolder.localPosition.z);
        Vector3 currentWeaponLocalRotation = weaponHolder.localEulerAngles;

        if (currentWeaponLocalPosition == lastWeaponLocalPosition &&
            currentWeaponLocalRotation == lastWeaponLocalRotation)
        {
            Debug.Log("SyncWeaponPosition: no change");
        }
        else
        {
            Debug.Log("change!");
        }
    }
}