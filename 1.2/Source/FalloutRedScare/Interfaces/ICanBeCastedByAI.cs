using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RedScare
{
	public interface ICanBeCastedByAI
	{
		bool CanBeUsed();
		float GetWeight();
		void UseDecideTarget(Thing target);
		void TryUseDecideTarget();
	}
}
