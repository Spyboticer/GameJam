﻿namespace MaxyGames.uNode.Transition {
	[UnityEngine.AddComponentMenu("")]
	[TransitionMenu("OnTriggerStay", "OnTriggerStay")]
	public class OnTriggerStay : TransitionEvent {
		[Filter(typeof(UnityEngine.Collider), SetMember = true)]
		public MemberData storeCollider = new MemberData();

		public override void OnEnter() {
			UEvent.Register<UnityEngine.Collider>(UEventID.OnTriggerStay, owner, Execute);
		}

		public override void OnExit() {
			UEvent.Unregister<UnityEngine.Collider>(UEventID.OnTriggerStay, owner, Execute);
		}

		void Execute(UnityEngine.Collider collider) {
			if(storeCollider.isAssigned) {
				storeCollider.Set(collider);
			}
			Finish();
		}

		public override string GenerateOnEnterCode() {
			if(GetTargetNode() == null)
				return null;
			if(!CG.HasInitialized(this)) {
				CG.SetInitialized(this);
				var mData = CG.generatorData.GetMethodData("OnTriggerStay");
				if(mData == null) {
					mData = CG.generatorData.AddMethod(
						"OnTriggerStay",
						CG.Type(typeof(void)),
						CG.Type(typeof(UnityEngine.Collider)));
				}
				string set = null;
				if(storeCollider.isAssigned) {
					set = CG.Set(CG.Value((object)storeCollider), mData.parameters[0].name).AddLineInEnd();
				}
				mData.AddCode(
					CG.Condition(
						"if",
						CG.CompareNodeState(node, null),
						set + CG.FlowFinish(this)
					)
				);
			}
			return null;
		}
	}
}
