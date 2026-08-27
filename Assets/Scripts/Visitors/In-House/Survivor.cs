using Managers;

namespace Visitors.In_House
{
    public class Survivor : Character
    {
        public override void OnGrabbed()
        {
            base.OnGrabbed();
        }

        public override void OnStolen()
        {
            GameManager.Instance.RemoveGuard();
        }
    }
}