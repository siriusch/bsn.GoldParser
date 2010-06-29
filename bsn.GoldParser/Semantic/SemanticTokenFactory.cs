using System;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	public abstract class SemanticTokenFactory {
		public bool IsStaticOutputType {
			get {
				return RedirectForOutputType == null;
			}
		}

		public abstract Type OutputType {
			get;
		}

		protected internal virtual Symbol RedirectForOutputType {
			get {
				return null;
			}
		}
	}
}
