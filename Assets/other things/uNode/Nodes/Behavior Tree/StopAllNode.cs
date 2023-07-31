using UnityEngine;
using System.Collections.Generic;

namespace MaxyGames.uNode.Nodes {
	[NodeMenu("Other", "Stop All", HideOnFlow = true)]
	[AddComponentMenu("")]
	public class StopAllNode : Node {
		public override void OnExecute() {
			var transform = this.transform;
			foreach(Transform tr in transform.parent) {
				if(tr == transform)
					continue;
				var node = tr.GetComponent<Node>();
				if(node != null) {
					node.Stop();
				}
			}
			Finish();
		}

		public override string GenerateCode() {
			string data = null;
			foreach(Transform tr in transform.parent) {
				if(tr == transform)
					continue;
				var node = tr.GetComponent<Node>();
				if(node != null && CG.IsStateNode(node)) {
					data += CG.StopEvent(node, false).AddLineInFirst();
				}
			}
			return data + CG.FlowFinish(this, true, false).AddLineInFirst();
		}

		public override string GetNodeName() {
			return "Stop All";
		}

		public override bool IsCoroutine() {
			//return HasCoroutineInFlow(nextNode);
			return false;
		}
	}
}
