﻿namespace MaxyGames.uNode.Transition {
	[UnityEngine.AddComponentMenu("")]
	[TransitionMenu("OnCollisionStay", "OnCollisionStay")]
	public class OnCollisionStay : TransitionEvent {
		[Filter(typeof(UnityEngine.Collision), SetMember = true)]
		public MemberData storeCollision = new MemberData();

		public override void OnEnter() {
			UEvent.Register<UnityEngine.Collision>(UEventID.OnCollisionStay, owner, Execute);
		}

		public override void OnExit() {
			UEvent.Unregister<UnityEngine.Collision>(UEventID.OnCollisionStay, owner, Execute);
		}

		void Execute(UnityEngine.Collision collision) {
			if(storeCollision.isAssigned) {
				storeCollision.Set(collision);
			}
			Finish();
		}

		public override string GenerateOnEnterCode() {
			if(GetTargetNode() == null)
				return null;
			if(!CG.HasInitialized(this)) {
				CG.SetInitialized(this);
				var mData = CG.generatorData.GetMethodData("OnCollisionStay");
				if(mData == null) {
					mData = CG.generatorData.AddMethod(
						"OnCollisionStay",
						CG.Type(typeof(void)),
						CG.Type(typeof(UnityEngine.Collision)));
				}
				string set = null;
				if(storeCollision.isAssigned) {
					set = CG.Set(CG.Value((object)storeCollision), mData.parameters[0].name).AddLineInEnd();
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
