using System;
using System.Collections.Generic;
using System.Xml;

using bsn.GoldParser.Parser;

namespace bsn.GoldParser {
	public class TokenXmlReader: XmlReader {
		public const string NonterminalNS = "urn:nonterminal";
		public const string TerminalNS = "urn:terminal";
		private readonly string columnAttribute;
		private readonly string lineAttribute;

		private readonly XmlNameTable nametable;
		private readonly string nonterminalNs;
		private readonly string nPrefix;
		private readonly Stack<KeyValuePair<Token, int>> stack = new Stack<KeyValuePair<Token, int>>();
		private readonly string terminalNs;
		private readonly string tPrefix;
		private ActiveAttribute activeAttribute;
		private Token current;
		private ElementPosition elementPosition;
		private bool onAttributeValue;
		private ReadState readState;

		public TokenXmlReader(XmlNameTable nametable, Token root) {
			this.nametable = nametable ?? new NameTable();
			terminalNs = this.nametable.Add(TerminalNS);
			nonterminalNs = this.nametable.Add(NonterminalNS);
			tPrefix = this.nametable.Add("t");
			nPrefix = this.nametable.Add("n");
			lineAttribute = this.nametable.Add("line");
			columnAttribute = this.nametable.Add("column");
			readState = ReadState.Initial;
			current = root;
		}

		public override XmlNodeType NodeType {
			get {
				if (onAttributeValue) {
					return XmlNodeType.Text;
				}
				if (activeAttribute != ActiveAttribute.None) {
					return XmlNodeType.Attribute;
				}
				if (current != null) {
					switch (elementPosition) {
					case ElementPosition.Start:
						return XmlNodeType.Element;
					case ElementPosition.Text:
						return XmlNodeType.Text;
					case ElementPosition.End:
						return XmlNodeType.EndElement;
					}
				}
				return XmlNodeType.None;
			}
		}

		public override string LocalName {
			get {
				switch (activeAttribute) {
				case ActiveAttribute.Line:
					if (!onAttributeValue) {
						return lineAttribute;
					}
					break;
				case ActiveAttribute.Column:
					if (!onAttributeValue) {
						return columnAttribute;
					}
					break;
				default:
					if ((current != null) && (elementPosition != ElementPosition.Text)) {
						return nametable.Add(current.ParentSymbol.XmlName);
					}
					break;
				}
				return string.Empty;
			}
		}

		public override string NamespaceURI {
			get {
				if ((activeAttribute == ActiveAttribute.None) && (current != null) && (elementPosition != ElementPosition.Text)) {
					return IsTerminal ? terminalNs : nonterminalNs;
				}
				return string.Empty;
			}
		}

		public override string Prefix {
			get {
				if ((activeAttribute == ActiveAttribute.None) && (current != null) && (elementPosition != ElementPosition.Text)) {
					return IsTerminal ? tPrefix : nPrefix;
				}
				return string.Empty;
			}
		}

		public override bool HasValue {
			get {
				return (activeAttribute != ActiveAttribute.None) || IsTerminal;
			}
		}

		public override string Value {
			get {
				switch (activeAttribute) {
				case ActiveAttribute.Line:
					return XmlConvert.ToString(current.Position.Line);
				case ActiveAttribute.Column:
					return XmlConvert.ToString(current.Position.Column);
				default:
					if (IsTerminal) {
						return current.Text;
					}
					break;
				}
				return string.Empty;
			}
		}

		public override int Depth {
			get {
				int result = stack.Count;
				if (current != null) {
					result++;
					if (activeAttribute != ActiveAttribute.None) {
						result++;
						if (onAttributeValue) {
							result++;
						}
					} else if (elementPosition == ElementPosition.Text) {
						result++;
					}
				}
				return result;
			}
		}

		public override string BaseURI {
			get {
				return string.Empty;
			}
		}

		public override bool IsEmptyElement {
			get {
				return false;
			}
		}

		private bool IsTerminal {
			get {
				return current is TextToken;
			}
		}

		public override int AttributeCount {
			get {
				return IsTerminal ? 2 : 0;
			}
		}

		public override bool EOF {
			get {
				return (ReadState == ReadState.EndOfFile);
			}
		}

		public override ReadState ReadState {
			get {
				return readState;
			}
		}

		public override XmlNameTable NameTable {
			get {
				return nametable;
			}
		}

		public override string GetAttribute(string name) {
			switch (name) {
			case "line":
				if (IsTerminal) {
					return XmlConvert.ToString(current.Position.Line);
				}
				break;
			case "column":
				if (IsTerminal) {
					return XmlConvert.ToString(current.Position.Column);
				}
				break;
			}
			return string.Empty;
		}

		public override string GetAttribute(string name, string namespaceURI) {
			if (string.IsNullOrEmpty(namespaceURI)) {
				return GetAttribute(name);
			}
			return string.Empty;
		}

		public override string GetAttribute(int i) {
			if (IsTerminal) {
				switch (i) {
				case 0:
					return XmlConvert.ToString(current.Position.Line);
				case 1:
					return XmlConvert.ToString(current.Position.Column);
				}
			}
			return string.Empty;
		}

		public override bool MoveToAttribute(string name) {
			if (IsTerminal) {
				switch (name) {
				case "line":
					activeAttribute = ActiveAttribute.Line;
					onAttributeValue = false;
					return true;
				case "column":
					activeAttribute = ActiveAttribute.Column;
					onAttributeValue = false;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToAttribute(string name, string ns) {
			if (string.IsNullOrEmpty(ns)) {
				return MoveToAttribute(name);
			}
			return false;
		}

		public override bool MoveToFirstAttribute() {
			if (IsTerminal) {
				activeAttribute = ActiveAttribute.Line;
				onAttributeValue = false;
				return true;
			}
			return false;
		}

		public override bool MoveToNextAttribute() {
			if (activeAttribute == ActiveAttribute.Line) {
				activeAttribute = ActiveAttribute.Column;
				onAttributeValue = false;
				return true;
			}
			return false;
		}

		public override bool MoveToElement() {
			if (activeAttribute != ActiveAttribute.None) {
				activeAttribute = ActiveAttribute.None;
				onAttributeValue = false;
				return true;
			}
			return false;
		}

		public override bool ReadAttributeValue() {
			if (onAttributeValue) {
				return false;
			}
			onAttributeValue = (activeAttribute != ActiveAttribute.None);
			return onAttributeValue;
		}

		public override bool Read() {
			switch (readState) {
			case ReadState.Initial:
				if (current != null) {
					readState = ReadState.Interactive;
					return true;
				}
				readState = ReadState.EndOfFile;
				return false;
			case ReadState.Interactive:
				MoveToElement();
				switch (elementPosition) {
				case ElementPosition.Start:
					if (IsTerminal) {
						elementPosition = ElementPosition.Text;
						return true;
					}
					if (current.Children.Length > 0) {
						stack.Push(new KeyValuePair<Token, int>(current, 0));
						current = current.Children[0];
						return true;
					}
					elementPosition = ElementPosition.End;
					return true;
				case ElementPosition.Text:
					elementPosition = ElementPosition.End;
					return true;
				case ElementPosition.End:
					while (stack.Count > 0) {
						KeyValuePair<Token, int> pair = stack.Pop();
						current = pair.Key;
						if (pair.Value < (current.Children.Length-1)) {
							current = current.Children[pair.Value+1];
							stack.Push(new KeyValuePair<Token, int>(pair.Key, pair.Value+1));
							elementPosition = ElementPosition.Start;
							return true;
						}
					}
					readState = ReadState.EndOfFile;
					break;
				}
				break;
			}
			return false;
		}

		public override void Close() {
			readState = ReadState.Closed;
		}

		public override string LookupNamespace(string prefix) {
			switch (prefix) {
			case "t":
				return terminalNs;
			case "n":
				return nonterminalNs;
			}
			return string.Empty;
		}

		public override void ResolveEntity() {}

		#region Nested type: ActiveAttribute
		private enum ActiveAttribute {
			None,
			Line,
			Column
		}
		#endregion

		#region Nested type: ElementPosition
		private enum ElementPosition {
			Start,
			Text,
			End
		}
		#endregion
	}
}