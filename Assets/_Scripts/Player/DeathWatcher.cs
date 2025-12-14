// DeathWatcher.cs
using UnityEngine;

/// <summary>
/// Giám sát trạng thái HP của nhân vật để kích hoạt death.
/// Khi HP <= 0, player sẽ respawn về giường, bị trừ tiền và sang ngày mới.
/// </summary>
public class DeathWatcher : MonoBehaviour
{
    [SerializeField] PlayerHealth health;
    [SerializeField] SleepManager sleep;
    bool handled;

    void Update()
    {
        if (!handled && health.hp <= 0)
        {
            handled = true;
            sleep.DeathNow();
        }
    }

    public void ResetHandled() => handled = false;
}
