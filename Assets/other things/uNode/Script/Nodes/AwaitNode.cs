using System;
using UnityEngine;

namespace MaxyGames.uNode.Nodes {
	[AddComponentMenu("")]
	public class AwaitNode : ValueNode {
		[Hide, ValueIn("Value")]
		public MemberData target = MemberData.none;
		[Hide, FlowOut("", true, hideOnNotFlowNode = true)]
		public MemberData onFinished = new MemberData();

		protected override object Value() => throw new NotImplementedException();

		public override void OnExecute() {
			throw new NotImplementedException();
		}

		public override System.Type ReturnType() {
			if(target.isAssigned) {
				try {
					return uNodeUtility.GetAsyncReturnType(target.type) ?? typeof(object);
				}
				catch { }
			}
			return typeof(object);
		}

		public override string GenerateCode() {
			return CG.Flow(
				("await " + CG.Value(target)).AddSemicolon(),
				CG.FlowFinish(this, true, onFinished)
			);
		}

		public override string GenerateValueCode() {
			return "(await " + CG.Value(target) + ")";
		}

		public override Type GetNodeIcon() {
			return typeof(TypeIcons.ValueIcon);
		}

		public override string GetNodeName() => "Await";

		public override string GetRichName() {
			return $"Await: {target.GetNicelyDisplayName(richName:true)}";
		}

		public override bool IsFlowNode() => true;

		public override bool IsCoroutine() {
			return HasCoroutineInFlow(onFinished);
		}

		public override void CheckError() {
			uNodeUtility.CheckError(target, this, nameof(target));
			if(target.isAssigned) {
				var type = target.type;
				if(type != null && type != typeof(void)) {
					var awaiterMethod = type.GetMemberCached(nameof(System.Threading.Tasks.Task.GetAwaiter));
					if(awaiterMethod != null && awaiterMethod is System.Reflection.MethodInfo methodInfo) {
						var returnType = methodInfo.ReturnType;
						if(returnType.HasImplementInterface(typeof(System.Runtime.CompilerServices.INotifyCompletion))) {
							var resultMethod = returnType.GetMemberCached("GetResult") as System.Reflection.MethodInfo;
							if(resultMethod == null) {
								uNodeUtility.RegisterEditorError(this, $"Invalid await type `{returnType.PrettyName()}` the type doesn't implement `GetResult` method.");
							}
						} else {
							uNodeUtility.RegisterEditorError(this, $"Invalid await type `{returnType.PrettyName()}` the type doesn't implement `{typeof(System.Runtime.CompilerServices.INotifyCompletion)}` interface.");
						}
					} else {
						uNodeUtility.RegisterEditorError(this, $"Invalid target type `{target.type.PrettyName()}` the type doesn't implement `GetAwaiter` method.");
					}
				}
			}
		}
	}
}