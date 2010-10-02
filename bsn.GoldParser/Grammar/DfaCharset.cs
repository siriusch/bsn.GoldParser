using System;
using System.Collections.Generic;
using System.Text;

namespace bsn.GoldParser.Grammar {
	public sealed class DfaCharset: GrammarObject<DfaCharset> {
		private readonly char[] charset;

		internal DfaCharset(CompiledGrammar owner, int index, string charset): base(owner, index) {
			this.charset = charset.ToCharArray();
		}

		public ICollection<char> Characters {
			get {
				return charset;
			}
		}
	}
}
