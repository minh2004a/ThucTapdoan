using UnityEngine;

[CreateAssetMenu(fileName = "StartingLoadout", menuName = "Game/Starting Loadout")]
public class StartingLoadout : ScriptableObject
{
    [System.Serializable]
    public struct Entry {
        public ItemSO item;       // để trống nếu dùng key
        public string itemKey;    // để trống nếu kéo thả item
        public int amount;        // số lượng
        public bool equip;        // có auto trang bị hay không
        public int hotbarSlot;    // -1 nếu không gán hotbar
    }
    [Tooltip("Số tiền người chơi nhận khi bắt đầu trò chơi mới.")]
    public int startingMoney = 0;
    public Entry[] items;
}