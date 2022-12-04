using UnityEngine;
using FSM;

    public class WallHang : StateBase
    {
        private PlayerFSM _fsm;
    
        public WallHang(PlayerFSM fsm) : base(needsExitTime: false)
        {
            this._fsm = fsm;
        }

        public override void OnEnter()
        {
            _fsm.animator.SetTrigger(_fsm.animIDHang);
            _fsm.transform.forward = _fsm.hangFwd;
            _fsm.transform.position = _fsm.hangPos + _fsm.transform.forward.normalized * -.25f + _fsm.transform.up * -.7f;
            base.OnEnter();
        }

        public override void OnLogic()
        {
            base.OnLogic();
        }

        public override void OnExit()
        {
            _fsm.hanging = false;
            base.OnExit();
        }
    }
