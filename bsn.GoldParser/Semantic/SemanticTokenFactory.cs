using System;

namespace bsn.GoldParser.Semantic {
	public abstract class SemanticTokenFactory {
		public abstract Type OutputType {
			get;
		}

		public virtual bool IsStaticOutputType {
			get {
				return false;
			}
		}
	}
}