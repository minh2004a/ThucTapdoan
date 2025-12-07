using UnityEngine;

// Class cha của mọi trạng thái (Idle, Walk, Attack, Die, v.v...) của Slime
// Khung sườn cho các trạng thái cụ thể kế thừa từ lớp này
// các phương thức Enter, Update, Exit để quản lý hành vi của Slime trong từng trạng thái
// Tương tác với SlimesController để thay đổi trạng thái và hành vi của Slime
// tất cả state được tạo từ SlimeState
public abstract class SlimeState
{
    protected SlimesController controller;

    // Constructor nhận vào SlimesController để tương tác với Slime
    public SlimeState(SlimesController slimesController)
    {
        this.controller = slimesController;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
